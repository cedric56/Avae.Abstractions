using Avae.Services;
using Avae.ViewModels;
using Microsoft.Maui.Platform;
using UXDivers.Popups.Maui.Controls;
using UXDivers.Popups.Services;

namespace Avae.Maui;

/// <summary>
/// MAUI implementation of view/modal resolution and the various dialog, notification, and theme
/// services, backed by an <see cref="IocContainer"/> and platform-specific dialog APIs.
/// </summary>
/// <param name="serviceProvider">The service provider used to resolve view models.</param>
/// <param name="getContainer">A factory that lazily supplies the <see cref="IocContainer"/> used for view resolution.</param>
/// <param name="configure">Optional callback invoked from <see cref="Configure(IIocContainer)"/> to register additional views/components.</param>
internal class IocConfiguration(IServiceProvider serviceProvider, Func<IocContainer> getContainer, Action<IIocContainer>? configure = null) :
        IIocConfiguration,
        ITaskDialogService,
        IContentDialogService,
        IDialogService,
        INotificationService,
        IRequestedThemeService
{
    IocContainer? _container = null;

    /// <summary>
    /// Gets the IoC container used for view resolution, lazily created via <c>getContainer</c> on first access.
    /// </summary>
    IocContainer Container { get => _container ??= getContainer(); }

    /// <summary>
    /// Gets the currently active MAUI page: the page of the activated window if one exists,
    /// otherwise the first available window's page, otherwise <see cref="Shell.Current"/>.
    /// </summary>
    public Page Current => Application.Current?.Windows.FirstOrDefault(w => w.IsActivated)?.Page ?? Application.Current?.Windows.FirstOrDefault()?.Page ?? Shell.Current;

    /// <summary>
    /// Invokes the configured <c>configure</c> callback, if any, to register additional views/components with the container.
    /// </summary>
    /// <param name="container">The container to configure.</param>
    public void Configure(IIocContainer container)
    {
        configure?.Invoke(container);
    }

    /// <summary>
    /// Resolves and creates the view registered under the specified key.
    /// </summary>
    /// <param name="key">The key the view was registered under.</param>
    /// <param name="params">Additional arguments passed through to the view's registered factory.</param>
    /// <returns>The created view instance.</returns>
    public object? GetView(string key, params object[] @params)
    {
        return Container.GetView(key, @params);
    }

    /// <summary>
    /// Resolves the view registered under the specified key, using the supplied navigation context.
    /// </summary>
    /// <param name="key">The key the view was registered under.</param>
    /// <param name="context">The navigation context to pass to the view's factory.</param>
    /// <returns>The resolved view as an <see cref="IViewFor"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the resolved view does not implement <see cref="IViewFor"/>.</exception>
    public IViewFor? GetContextFor(string key, NavigableContext context)
    {
        var view = Container.GetView(key, [context]);
        return view as IViewFor ?? throw new InvalidOperationException($"View must implement {nameof(IViewFor)}");
    }

    /// <summary>
    /// Resolves the strongly typed view for the specified view model type, using the supplied navigation context.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type whose view should be resolved.</typeparam>
    /// <param name="context">The navigation context to pass to the view's factory.</param>
    /// <returns>The resolved view as an <see cref="IViewFor{TViewModel}"/>, or <see langword="null"/> if resolution fails or the result does not match.</returns>
    public IViewFor<TViewModel>? GetContextFor<TViewModel>(NavigableContext context) where TViewModel : IViewModelBase
    {
        return Container.GetView(typeof(TViewModel).Name, [context]) as IViewFor<TViewModel>;
    }

    /// <summary>
    /// Resolves the modal view associated with the specified closeable view model type.
    /// </summary>
    /// <typeparam name="TViewModel">The closeable view model type whose modal view should be resolved.</typeparam>
    /// <typeparam name="TResult">The result type produced when the modal is closed.</typeparam>
    /// <param name="context">The navigation context to pass to the view's factory.</param>
    /// <returns>The resolved modal view, or <see langword="null"/> if resolution fails.</returns>
    public IModalFor<TViewModel, TResult>? GetModalFor<TViewModel, TResult>(NavigableContext context) where TViewModel : ICloseableViewModel<TResult>
    {
        return Container.GetModal<TViewModel, TResult>(context);
    }

    /// <summary>
    /// Displays a task dialog assembled from header, subheader, content, progress bar, and footer
    /// sections, and awaits the user's button selection.
    /// </summary>
    /// <param name="params">The task dialog's configuration, including header, content, and footer.</param>
    /// <param name="results">Up to three result values corresponding to the primary, secondary, and close buttons.</param>
    /// <returns>The result value associated with the button the user selected.</returns>
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
            IsVisible = @params.Header is not null || @params.IconSource is not null
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

    /// <summary>
    /// Displays a content dialog with up to three buttons (primary, secondary, close) and awaits the user's selection.
    /// </summary>
    /// <param name="params">The content dialog's configuration, including title, content, and button text.</param>
    /// <returns>The <see cref="ContentDialogResult"/> corresponding to the button the user selected.</returns>
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

    /// <summary>
    /// Displays an alert showing the specified exception's message.
    /// </summary>
    /// <param name="ex">The exception whose message should be displayed.</param>
    /// <param name="title">The dialog title. Defaults to "Error".</param>
    /// <returns>A task representing the asynchronous display operation.</returns>
    public Task ShowErrorAsync(Exception ex, string title = "Error")
    {
        return Current.DisplayAlertAsync(title, ex.Message, "Ok");
    }

    /// <summary>
    /// Displays an alert with a single "Ok" button.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns>A task representing the asynchronous display operation.</returns>
    public Task ShowOkAsync(string message, string title = "Title")
    {
        return Current.DisplayAlertAsync(title, message, "Ok");
    }

    /// <summary>
    /// Displays an alert with "Yes" and "No" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns><see langword="true"/> if the user selected "Yes"; otherwise <see langword="false"/>.</returns>
    public Task<bool> ShowYesNoAsync(string message, string title = "Title")
    {
        return Current.DisplayAlertAsync(title, message, "Yes", "No");
    }

    /// <summary>
    /// Displays an alert with "Ok" and "Cancel" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns><see langword="true"/> if the user selected "Ok"; otherwise <see langword="false"/>.</returns>
    public Task<bool> ShowOkCancelAsync(string message, string title = "Title")
    {
        return Current.DisplayAlertAsync(title, message, "Ok", "Cancel");
    }

    /// <summary>
    /// Displays an alert with "Ok" and "Abort" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns><see langword="true"/> if the user selected "Ok"; otherwise <see langword="false"/>.</returns>
    public Task<bool> ShowOkAbortAsync(string message, string title = "Title")
    {
        return Current.DisplayAlertAsync(title, message, "Ok", "Abort");
    }

    /// <summary>
    /// Displays an alert with "Yes", "No", and "Cancel" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns>0 if "Yes" was selected, 1 if "No" was selected, or 2 if "Cancel" was selected.</returns>
    public Task<int> ShowYesNoCancelAsync(string message, string title = "Title")
    {
        return DisplayThreeButtons(title, message, "Yes", "No", "Cancel", 0, 1, 2);
    }

    /// <summary>
    /// Displays an alert with "Yes", "No", and "Abort" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns>0 if "Yes" was selected, 1 if "No" was selected, or 2 if "Abort" was selected.</returns>
    public Task<int> ShowYesNoAbortAsync(string message, string title = "Title")
    {
        return DisplayThreeButtons(title, message, "Yes", "No", "Abort", 0, 1, 2);
    }

    /// <summary>
    /// Resolves the view model of type <typeparamref name="TViewModel"/>, wraps its associated modal
    /// view in a popup page, and pushes it, resolving the returned task when the view model requests a close.
    /// </summary>
    /// <typeparam name="TViewModel">The closeable view model type to show.</typeparam>
    /// <typeparam name="TResult">The result type produced when the modal is closed.</typeparam>
    /// <param name="context">Optional navigation context; an empty context is used if not supplied.</param>
    /// <returns>The result value supplied when the view model requested a close.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no modal view is registered for <typeparamref name="TViewModel"/>.</exception>
    async Task<TResult?> Services.IDialogService.ShowModalAsync<TViewModel, TResult>(NavigableContext? context)
        where TResult : default
    {
        Ensure();

        var viewModel = serviceProvider.GetViewModel<TViewModel>(context);
        var view = GetModalFor<TViewModel, TResult>(context ?? new NavigableContext()) ?? throw new InvalidOperationException($"Unable to create view for {typeof(TViewModel).Name}.  Ensure that it is registered in the container.");
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

    /// <summary>
    /// Displays a platform-native dialog with up to three buttons, using the appropriate native API
    /// for the current platform (Android alert dialog, WinUI content dialog, or an iOS/Mac Catalyst
    /// alert controller or form-sheet view controller), and awaits the user's selection.
    /// </summary>
    /// <typeparam name="T">The result type associated with each button.</typeparam>
    /// <param name="title">The dialog title.</param>
    /// <param name="content">The dialog content: a message string, or a view/element to embed.</param>
    /// <param name="primaryButtonText">The primary button's text, or <see langword="null"/> to omit it.</param>
    /// <param name="secondaryButtonText">The secondary button's text, or <see langword="null"/> to omit it.</param>
    /// <param name="closeButtonText">The close button's text, or <see langword="null"/> to omit it.</param>
    /// <param name="primaryResult">The result returned if the primary button is selected.</param>
    /// <param name="secondaryResult">The result returned if the secondary button is selected.</param>
    /// <param name="closeResult">The result returned if the close button is selected.</param>
    /// <returns>The result associated with the button the user selected.</returns>
    /// <exception cref="NotImplementedException">Thrown on platforms without a supported native dialog implementation, or for unsupported content types.</exception>
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
        else if (content is UIKit.UIView contentView)
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
                if (UIKit.UIFont.BoldSystemFontOfSize(17) is { } font)
                {
                    label.Font = font;
                }
                else if (UIKit.UIFont.SystemFontOfSize(17) is { } font2)
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

    /// <summary>
    /// Resource dictionary holding light-theme overrides (text color, popup backdrop color)
    /// added to or removed from the application's merged dictionaries based on the current theme.
    /// </summary>
    ResourceDictionary colors = new ResourceDictionary();

    /// <summary>
    /// One-time initialization: builds the light-theme color overrides and subscribes to theme
    /// change notifications so they're applied/removed as the app theme changes. Does nothing if
    /// called before <see cref="Application.Current"/> is available or after the first successful call.
    /// </summary>
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

    /// <summary>
    /// Adds or removes the light-theme color overrides from the application's merged resource
    /// dictionaries depending on whether the app's requested theme is light.
    /// </summary>
    /// <param name="sender">The event source (unused).</param>
    /// <param name="e">The theme-changed event arguments.</param>
    void ThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        if (Application.Current?.RequestedTheme == AppTheme.Light)
            Application.Current?.Resources.MergedDictionaries.Add(colors);
        else
            Application.Current?.Resources.MergedDictionaries.Remove(colors);
    }


    /// <summary>
    /// Displays a transient popup notification with a type-colored icon, optionally auto-dismissing
    /// after <paramref name="expiration"/> and invoking callbacks on tap or close.
    /// </summary>
    /// <param name="title">The notification title.</param>
    /// <param name="message">The notification message.</param>
    /// <param name="type">The notification type, used to color the icon. Defaults to <see cref="NotificationType.Information"/>.</param>
    /// <param name="expiration">Optional duration after which the notification is automatically dismissed if not already closed.</param>
    /// <param name="onClick">Optional callback invoked when the notification is tapped.</param>
    /// <param name="onClose">Optional callback invoked once the notification has been dismissed.</param>
    public async void Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Action? onClick = null, Action? onClose = null)
    {
        Ensure();

        var pop = new FloaterPopup()
        {
            Text = message,
            Title = title
        };

        if (Application.Current?.RequestedTheme == AppTheme.Light)
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

    /// <summary>
    /// Applies the specified theme to the application by setting <see cref="Application.UserAppTheme"/>.
    /// </summary>
    /// <param name="theme">The requested theme.</param>
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