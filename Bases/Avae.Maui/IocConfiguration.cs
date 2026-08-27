using Avae.ViewModels;
using Avae.Services;
using Microsoft.Maui.Platform;
using UXDivers.Popups.Maui.Controls;
using UXDivers.Popups.Services;

namespace Avae.Maui;
internal class IocConfiguration(IServiceProvider serviceProvider, Func<IocContainer> getContainer, Action<IIocContainer>? configure = null) :
        IIocConfiguration, 
        ITaskDialogService, 
        IContentDialogService, 
        IDialogService,
        INotificationService,
        IRequestedThemeService
{
    IocContainer? _container = null;
    IocContainer Container { get => _container ??= getContainer(); }

    public Page Current => Application.Current?.Windows.FirstOrDefault(w => w.IsActivated)?.Page ?? Application.Current?.Windows.FirstOrDefault()?.Page ?? Shell.Current;

    public int MaxItems => 5;

    public NotificationPosition Position => NotificationPosition.TopCenter;

    public void Configure(IIocContainer container)
    {
        configure?.Invoke(container);
    }

    public object? GetView(string key, params object[] @params)
    {
        return Container.GetView(key, @params);
    }

    public IViewFor? GetContextFor(string key, NavigationContext context)
    {
        var view = Container.GetView(key, [context]);
        return view as IViewFor ?? throw new InvalidOperationException($"View must implement {nameof(IViewFor)}");
    }

    public IViewFor<TViewModel>? GetContextFor<TViewModel>(NavigationContext context) where TViewModel : IViewModelBase
    {
        return Container.GetView(typeof(TViewModel).Name, [context]) as IViewFor<TViewModel>;
    }

    public IModalFor<TViewModel, TResult>? GetModalFor<TViewModel, TResult>(NavigationContext context) where TViewModel : ICloseableViewModel<TResult>
    {
        var view = Container.GetView(typeof(TViewModel).Name, [context]);

        // Now verify the TResult type matches
        Type viewType = view.GetType();
        Type[] interfaces = viewType.GetInterfaces();

        // Find the IModalFor<,> interface implementation
        var modalInterface = interfaces.FirstOrDefault(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IModalFor<,>));

        if (modalInterface != null)
        {
            Type[] genericArgs = modalInterface.GetGenericArguments();
            Type viewModelType = genericArgs[0];
            Type resultType = genericArgs[1];

            // Check if the TResult matches
            if (resultType != typeof(TResult))
            {
                throw new InvalidOperationException(
                    $"The view associated with view model {typeof(TViewModel).Name} expects result type {resultType.Name}, " +
                    $"but {typeof(TResult).Name} was requested.");
            }
        }

        if (view is not IModalFor<TViewModel, TResult> modal)
            throw new InvalidOperationException($"The view associated with the view model {typeof(TViewModel).Name} is not a modal view.");

        return view as IModalFor<TViewModel, TResult>;
    }

    public async Task<TaskDialogStandardResult> ShowAsync(TaskDialogParams @params, params TaskDialogStandardResult[] results)
    {
        // Main Grid
        var mainGrid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
{
    new RowDefinition { Height = GridLength.Auto },
    new RowDefinition { Height = GridLength.Auto },
    new RowDefinition { Height = GridLength.Star },
    new RowDefinition { Height = GridLength.Auto }
}
        };

        // ============ HEADER HOST ============
        var headerHost = new Grid
        {
            IsVisible = @params.Header is not null ||@params.IconSource is not null
        };
        Grid.SetRow(headerHost, 0);

        // Icon Host (Viewbox)
        //var iconHost = new Viewbox
        //{
        //    WidthRequest = (double)Application.Current.Resources["TaskDialogIconSize"],
        //    HeightRequest = (double)Application.Current.Resources["TaskDialogIconSize"],
        //    VerticalOptions = LayoutOptions.Center,
        //    HorizontalOptions = LayoutOptions.Start,
        //    Margin = (Thickness)Application.Current.Resources["TaskDialogIconMargin"],
        //    IsVisible = false
        //};

        var iconElement = new Image
        {
            Source = @params.IconSource as ImageSource,
            AutomationId = "IconElement"
        };
        //iconHost.Child = iconElement;

        // Header Text
        var headerText = new Label
        {
            Text = @params.Header,
            LineBreakMode = LineBreakMode.WordWrap,
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
            IsVisible = @params.Header is not null
        };

        headerHost.Children.Add(iconElement);
        headerHost.Children.Add(headerText);
        mainGrid.Children.Add(headerHost);

        // ============ SUBHEADER TEXT ============
        var subHeaderText = new Label
        {
            Text = @params.SubHeader,
            FontSize = 9,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            LineBreakMode = LineBreakMode.WordWrap,
            IsVisible = @params.SubHeader is not null
        };
        Grid.SetRow(subHeaderText, 1);
        mainGrid.Children.Add(subHeaderText);

        // ============ CONTENT AREA ============
        var scrollView = new ScrollView
        {
            //Margin = (Thickness)Application.Current.Resources["TaskDialogContentMargin"],
            HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
            VerticalScrollBarVisibility = ScrollBarVisibility.Default
        };
        Grid.SetRow(scrollView, 2);

        var contentStack = new StackLayout
        {
            //Spacing = 18
        };

        // Content Presenter
        var contentPresenter = new Microsoft.Maui.Controls.ContentView
        {
            Content = @params.Content as Microsoft.Maui.Controls.View,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        contentStack.Children.Add(contentPresenter);

        // Progress Bar
        var progressBar = new ProgressBar
        {
            IsVisible = @params.ShowProgressBar
        };
        contentStack.Children.Add(progressBar);

        // More Details Panel
        var moreDetailsPanel = new StackLayout
        {
            IsVisible = false,
            Spacing = 0
        };

        var moreDetailsButton = new Button
        {
            Text = "Footer",// (string)Application.Current.Resources["TaskDialogFooterButtonNormalText"],
            IsVisible = @params.Footer is not null,
            //Style = (Style)Application.Current.Resources["TaskDialogMoreDetailsButton"]
        };
        moreDetailsPanel.Children.Add(moreDetailsButton);

        var footerHost = new Microsoft.Maui.Controls.ContentView
        {
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Fill,
            Content = @params.Footer as Microsoft.Maui.Controls.View,
            IsVisible = @params.Footer is not null
        };
        moreDetailsPanel.Children.Add(footerHost);

        contentStack.Children.Add(moreDetailsPanel);
        scrollView.Content = contentStack;
        mainGrid.Children.Add(scrollView);

        // Return or set the main grid as your content
        // this.Content = mainGrid;

        return await DisplayThreeButtons<TaskDialogStandardResult>(
            @params.Title,
            mainGrid,
            results.ElementAtOrDefault(0).ToString(),
            results.Length > 1 ? results.ElementAtOrDefault(1).ToString() : null,
            results.Length > 2 ? results.ElementAtOrDefault(2).ToString() : null,
            results.ElementAtOrDefault(0),
            results.ElementAtOrDefault(1),
            results.ElementAtOrDefault(2));
    }

    public async Task<ContentDialogResult> ShowAsync(ContentDialogParams @params)
    {
        return await DisplayThreeButtons<ContentDialogResult>(
            @params.Title,
            @params.Content,
            @params.PrimaryButtonText,
            @params.SecondaryButtonText,
            @params.CloseButtonText,
            ContentDialogResult.Primary,
            ContentDialogResult.Secondary,
            ContentDialogResult.None);
    }

    public Task ShowErrorAsync(Exception ex, string title = "Error")
    {
        return Current.DisplayAlertAsync(title, ex.Message, "Ok");
    }

    public Task ShowOkAsync(string message, string title = "Title")
    {
        return Current.DisplayAlertAsync(title, message, "Ok");
    }

    public Task<bool> ShowYesNoAsync(string message, string title = "Title")
    {
        return Current.DisplayAlertAsync(title, message, "Yes", "No");
    }

    public Task<bool> ShowOkCancelAsync(string message, string title = "Title")
    {
        return Current.DisplayAlertAsync(title, message, "Ok", "Cancel");
    }

    public Task<bool> ShowOkAbortAsync(string message, string title = "Title")
    {
        return Current.DisplayAlertAsync(title, message, "Ok", "Abort");
    }

    public Task<int> ShowYesNoCancelAsync(string message, string title = "Title")
    {
        return DisplayThreeButtons(title, message, "Yes", "No", "Cancel", 0, 1, 2);
    }

    public Task<int> ShowYesNoAbortAsync(string message, string title = "Title")
    {
        return DisplayThreeButtons(title, message, "Yes", "No", "Abort", 0, 1, 2);
    }

    async Task<TResult?> Services.IDialogService.ShowModalAsync<TViewModel, TResult>(NavigationContext? context)
        where TResult : default
    {
        Ensure();

        var viewModel = serviceProvider.GetViewModel<TViewModel>(context);
        var view = GetModalFor<TViewModel, TResult>(context ?? new NavigationContext()) ?? throw new InvalidOperationException($"Unable to create view for {typeof(TViewModel).Name}.  Ensure that it is registered in the container.");
        view.Context = viewModel;
        var modal = new AvaePopupPage<TResult>(viewModel.Title, viewModel.Commands)
        {
            Content = view as Microsoft.Maui.Controls.View
        };
        viewModel.CloseRequested += CloseRequestedHandler;
        return await IPopupService.Current.PushAsync(modal);

        async void CloseRequestedHandler(object? sender, TResult? e)
        {
            viewModel.CloseRequested -= CloseRequestedHandler;
            modal.SetResult(e);
            await IPopupService.Current.PopAsync(modal);
        }
    }

    async Task<T?> DisplayThreeButtons<T>(
         string? title, object? content,
         string? primaryButtonText, string? secondaryButtonText, string? closeButtonText,
         T primaryResult, T secondaryResult, T closeResult)
    {
        var taskCompletionSource = new TaskCompletionSource<T?>();

        if (content is Element e)
        {
            content = e.ToPlatform(Current?.Handler?.MauiContext ?? new MauiContext(serviceProvider));
        }
#if ANDROID
            var alertBuilder = new Android.App.AlertDialog.Builder(Platform.CurrentActivity);

            alertBuilder.SetTitle(title);
            if (content is string message)
                alertBuilder.SetMessage(message);
            else
                alertBuilder.SetView(content as Android.Views.View);

            if (!string.IsNullOrEmpty(primaryButtonText))
                alertBuilder.SetPositiveButton(primaryButtonText, (senderAlert, args) =>
                {
                    taskCompletionSource.SetResult(primaryResult);
                });
            if (!string.IsNullOrEmpty(secondaryButtonText))
                alertBuilder.SetNegativeButton(secondaryButtonText, (senderAlert, args) =>
                {
                    taskCompletionSource.SetResult(secondaryResult);
                });
            if (!string.IsNullOrEmpty(closeButtonText))
                alertBuilder.SetNeutralButton(closeButtonText, (senderAlery, args) =>
                {
                    taskCompletionSource.SetResult(closeResult);
                });

            var alertDialog = alertBuilder.Create();
            alertDialog?.Show();

            return await taskCompletionSource.Task;
#elif WINDOWS

        var current = Current;
        if (current == null)
            throw new InvalidNavigationException($"{nameof(Current)} can not be null");

        var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
        {
            RequestedTheme = Application.Current?.RequestedTheme ==
             AppTheme.Dark ? Microsoft.UI.Xaml.ElementTheme.Dark :
             Application.Current?.RequestedTheme == AppTheme.Light ?
             Microsoft.UI.Xaml.ElementTheme.Light : Microsoft.UI.Xaml.ElementTheme.Default,
            Title = title,
            Content = content,
            PrimaryButtonText = primaryButtonText,
            SecondaryButtonText = secondaryButtonText,
            CloseButtonText = closeButtonText,
            XamlRoot = current.Handler?.PlatformView is Microsoft.UI.Xaml.UIElement page ? page.XamlRoot : null
        };
        dialog.PrimaryButtonClick += (s, e) => taskCompletionSource.SetResult(primaryResult);
        dialog.SecondaryButtonClick += (s, e) => taskCompletionSource.SetResult(secondaryResult);
        dialog.CloseButtonClick += (s, e) => taskCompletionSource.SetResult(closeResult);

        await dialog.ShowAsync();
        return await taskCompletionSource.Task;
#elif MACCATALYST || IOS
        if (content is string message)
        {
            var alert = new UIKit.UIAlertController
            {
                Title = title,
                Message = message,
                //PreferredStyle = UIKit.UIAlertControllerStyle.Alert
            };
            if (!string.IsNullOrEmpty(primaryButtonText))
                alert.AddAction(UIKit.UIAlertAction.Create(primaryButtonText, UIKit.UIAlertActionStyle.Default, _ =>
                {
                    taskCompletionSource.SetResult(primaryResult);
                }));
            if (!string.IsNullOrEmpty(secondaryButtonText))
                alert.AddAction(UIKit.UIAlertAction.Create(secondaryButtonText, UIKit.UIAlertActionStyle.Default, _ =>
                {
                    taskCompletionSource.SetResult(secondaryResult);
                }));
            if (!string.IsNullOrEmpty(closeButtonText))
                alert.AddAction(UIKit.UIAlertAction.Create(closeButtonText, UIKit.UIAlertActionStyle.Default, _ =>
                {
                    taskCompletionSource.SetResult(closeResult);
                }));
            var keyWindow = UIKit.UIApplication.SharedApplication.ConnectedScenes
                                .OfType<UIKit.UIWindowScene>()
                                .SelectMany(s => s.Windows)
                                .FirstOrDefault(w => w.IsKeyWindow);
            var rootViewController = keyWindow?.RootViewController;
            rootViewController?.PresentViewController(alert, true, null);
            return await taskCompletionSource.Task;
        }
        else if(content is UIKit.UIView contentView)
        {
            var vc = new UIKit.UIViewController { ModalPresentationStyle = UIKit.UIModalPresentationStyle.FormSheet };
            vc.PreferredContentSize = new CoreGraphics.CGSize(320, 360);

            var stack = new UIKit.UIStackView { Axis = UIKit.UILayoutConstraintAxis.Vertical, Spacing = 12 };
            stack.TranslatesAutoresizingMaskIntoConstraints = false;

            if (!string.IsNullOrEmpty(title))
            {
                var label = new UIKit.UILabel
                {
                    Text = title
                };
                if(UIKit.UIFont.BoldSystemFontOfSize(17) is { } font)
                {
                    label.Font = font;
                }
                else if(UIKit.UIFont.SystemFontOfSize(17) is { } font2)
                {
                    label.Font = font2;
                }
                stack.AddArrangedSubview(label);
            }
            contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            stack.AddArrangedSubview(contentView);

            var buttons = new UIKit.UIStackView
            {
                Axis = UIKit.UILayoutConstraintAxis.Horizontal,
                Spacing = 8,
                Distribution = UIKit.UIStackViewDistribution.FillEqually
            };

            void AddButton(string? text, T result)
            {
                if (string.IsNullOrEmpty(text)) return;
                var button = UIKit.UIButton.FromType(UIKit.UIButtonType.System);
                button.SetTitle(text, UIKit.UIControlState.Normal);
                button.TouchUpInside += (_, _) => { taskCompletionSource.SetResult(result); vc.DismissViewController(true, null); };
                buttons.AddArrangedSubview(button);
            }
            AddButton(primaryButtonText, primaryResult);
            AddButton(secondaryButtonText, secondaryResult);
            AddButton(closeButtonText, closeResult);
            stack.AddArrangedSubview(buttons);

            vc.View!.AddSubview(stack);
            UIKit.NSLayoutConstraint.ActivateConstraints(new[]
            {
                    stack.LeadingAnchor.ConstraintEqualTo(vc.View.LeadingAnchor, 16),
                    stack.TrailingAnchor.ConstraintEqualTo(vc.View.TrailingAnchor, -16),
                    stack.TopAnchor.ConstraintEqualTo(vc.View.TopAnchor, 16),
                    stack.BottomAnchor.ConstraintEqualTo(vc.View.BottomAnchor, -16),
                });
            return await taskCompletionSource.Task;
        }
       
#endif
        throw new NotImplementedException();
    }

    bool _isLoad = false;

    ResourceDictionary colors = new ResourceDictionary();

    private void Ensure()
    {
        if (Application.Current == null)
            return;

        if (_isLoad) 
            return;

        _isLoad = true;
        colors.Add("TextColor", Colors.Black);
        colors.Add("PopupBackdropColor", Color.FromArgb("#80B2B2B2"));
        //Application.Current?.Resources.MergedDictionaries.Add(new PopupStyles());
        //Application.Current?.Resources.MergedDictionaries.Add(new DarkTheme());            
        Application.Current?.RequestedThemeChanged += ThemeChanged;
        ThemeChanged(this, new AppThemeChangedEventArgs(Application.Current!.RequestedTheme));
    }

    void ThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        if (Application.Current?.RequestedTheme == AppTheme.Light)
            Application.Current?.Resources.MergedDictionaries.Add(colors);
        else
            Application.Current?.Resources.MergedDictionaries.Remove(colors);
    }
    

    public async void Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Action? onClick = null, Action? onClose = null)
    {
        Ensure();

        var pop = new FloaterPopup()
        {
            Text = message,
            Title = title
        };

        if(Application.Current?.RequestedTheme == AppTheme.Light)
        {
            pop.PopupBackground = Colors.White;
            //pop.Background = Color.FromArgb("#ffb2b2b2");
        }
        
        pop.IconColor = type switch
        {
            NotificationType.Success => Colors.Green,
            NotificationType.Warning => Colors.Orange,
            NotificationType.Information => Colors.Blue,
            NotificationType.Error => Colors.Red,
            _ => Colors.Blue
        };

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += Tapped;
        pop.GestureRecognizers.Add(tapGesture);

        bool isClosed = false;
        if (expiration.HasValue)
        {
            _ = Task.Run(Delay);
        }

        await IPopupService.Current.PushAsync(pop);
        isClosed = true;
        onClose?.Invoke();

        async void Tapped(object? sender, TappedEventArgs e)
        {
            onClick?.Invoke();
            await IPopupService.Current.PopAsync(pop);
        }

        async Task Delay()
        {
            await Task.Delay((int)expiration.Value.TotalMilliseconds);
            if (!isClosed)
                await IPopupService.Current.PopAsync(pop);
        }
    }

    public void Request(RequestedTheme theme)
    {
        Application.Current?.UserAppTheme
            = theme switch
            {
                RequestedTheme.Light => AppTheme.Light,
                RequestedTheme.Dark => AppTheme.Dark,
                _ => AppTheme.Unspecified,
            };
    }
}
