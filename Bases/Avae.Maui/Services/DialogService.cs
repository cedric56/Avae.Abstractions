using Avae.Services;
using Avae.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;
using System.Collections;
using System.Collections.ObjectModel;
using UXDivers.Popups;
using UXDivers.Popups.Maui;
using UXDivers.Popups.Services;

namespace Avae.Maui;

internal class DialogService(IServiceProvider provider, IIocConfiguration configuration) : IDialogService
{
    /// <summary>
    /// Gets the currently active MAUI page: the page of the activated window if one exists,
    /// otherwise the first available window's page, otherwise <see cref="Shell.Current"/>.
    /// </summary>
    public Page Current => Application.Current?.Windows.FirstOrDefault(w => w.IsActivated)?.Page ?? Application.Current?.Windows.FirstOrDefault()?.Page ?? Shell.Current;

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
    async Task<TResult?> IDialogService.ShowModalAsync<TViewModel, TResult>(NavigableContext? context)
        where TResult : default
    {
        //helper.Ensure();

        var viewModel = provider.GetViewModel<TViewModel>(context);
        var view = configuration.GetModalFor<TViewModel, TResult>(context ?? new NavigableContext()) ?? throw new InvalidOperationException($"Unable to create view for {typeof(TViewModel).Name}.  Ensure that it is registered in the container.");
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

    public async Task<T?> DisplayThreeButtons<T>(
         string? title, object? content,
         string? primaryButtonText, string? secondaryButtonText, string? closeButtonText,
         T primaryResult, T secondaryResult, T closeResult)
    {
        var taskCompletionSource = new TaskCompletionSource<T?>();

        if (content is Element e)
        {
            content = e.ToPlatform(Current?.Handler?.MauiContext ?? new MauiContext(provider));
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

    public class AvaePopupPage : PopupPage
    {
        public class CommandIndex : NamedCommand
        {
            public required int Index { get; set; }
        }

        public AvaePopupPage(
            string title,
            ObservableCollection<NamedCommand> commands)
        {
            Title = title;
            Buttons = commands;

            ControlTemplate = new ControlTemplate(() => DeviceInfo.Platform switch
            {
                var p when p == DevicePlatform.Android => BuildAndroidTemplate(Commands),
                var p when p == DevicePlatform.WinUI => BuildWindowsTemplate(title, Commands),
                _ => BuildIosTemplate(Commands) // iOS + MacCatalyst
            });

            if (Application.Current?.RequestedTheme == AppTheme.Light)
            {
                PopupBackground = Colors.White;
                Background = Color.FromArgb("#80B2B2B2");
            }
        }

        public string Title
        {
            get; set;
        }

        public ObservableCollection<NamedCommand> Buttons
        {
            get;
            set;
        }

        public ColumnDefinitionCollection Definitions
        {
            get
            {
                return [.. Buttons?.Select(b => new ColumnDefinition(GridLength.Star)).ToArray() ?? []];
            }
        }

        public ObservableCollection<CommandIndex> Commands
        {
            get
            {
                return new ObservableCollection<CommandIndex>(
                    Buttons?.Select(b => new CommandIndex()
                    {
                        Name = b.Name,
                        Command = b.Command,
                        Index = Buttons.IndexOf(b)

                    }) ?? []);
            }
        }

        private static object BuildIosTemplate(IEnumerable commands)
        {
            var grid = new Grid
            {
                WidthRequest = IdiomValue(270, 320, 320),
                VerticalOptions = LayoutOptions.Center,
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(1),
                    new RowDefinition(GridLength.Auto),
                }
            };

            var border = new Border
            {
                Stroke = Colors.Transparent,
                StrokeShape = new RoundRectangle { CornerRadius = 14 }
            };
            border.SetBinding(Border.BackgroundColorProperty,
                new Binding(nameof(AvaePopupPage.PopupBackground), source: RelativeBindingSource.TemplatedParent));
            Grid.SetRowSpan(border, 4);
            grid.Add(border);

            var titleLabel = new Label
            {
                FontSize = 17,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
            };
            titleLabel.SetBinding(Label.TextProperty,
                new Binding(nameof(AvaePopupPage.Title), source: RelativeBindingSource.TemplatedParent));
            titleLabel.SetDynamicResource(Label.TextColorProperty, "TextColor");

            var titleStack = new VerticalStackLayout { Spacing = 4, Padding = new Thickness(16, 20, 16, 4) };
            titleStack.Add(titleLabel);
            Grid.SetRow(titleStack, 0);
            grid.Add(titleStack);

            var contentPresenter = new ContentPresenter { Padding = new Thickness(16, 0, 16, 20) };
            Grid.SetRow(contentPresenter, 1);
            grid.Add(contentPresenter);

            var hairline = new BoxView { HeightRequest = 1 };
            hairline.SetDynamicResource(BoxView.ColorProperty, "PopupBorderColor");
            Grid.SetRow(hairline, 2);
            grid.Add(hairline);

            var buttonRow = new Grid
            {
                ColumnSpacing = 1,
                HeightRequest = 44
            };
            buttonRow.SetBinding(Grid.ColumnDefinitionsProperty,
                new Binding(nameof(AvaePopupPage.Definitions), source: RelativeBindingSource.TemplatedParent));
            buttonRow.SetDynamicResource(Grid.BackgroundColorProperty, "PopupBorderColor");

            BindableLayout.SetItemsSource(buttonRow, commands);
            BindableLayout.SetItemTemplate(buttonRow, new DataTemplate(() =>
            {
                var button = new Button
                {
                    TextColor = Color.FromArgb("#007AFF"),
                    CornerRadius = 0,
                    BorderWidth = 0,
                    HorizontalOptions = LayoutOptions.Fill
                };
                button.SetBinding(Button.TextProperty, "Name");
                button.SetBinding(Button.CommandProperty, "Command");
                button.SetBinding(Grid.ColumnProperty, "Index");
                button.SetDynamicResource(Button.BackgroundColorProperty, "BackgroundSecondaryColor");
                //button.SetBinding(Button.FontAttributesProperty, new Binding("IsPrimary", converter: new BoolToFontAttrConverter()));
                return button;
            }));

            Grid.SetRow(buttonRow, 3);
            grid.Add(buttonRow);

            return grid;
        }

        // ===================== Android — Material 3 dialog style =====================
        private static object BuildAndroidTemplate(IEnumerable commands)
        {
            var grid = new Grid
            {
                WidthRequest = IdiomValue(-1, 340, 340),
                VerticalOptions = LayoutOptions.Center,
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto),
                },
                Padding = new Thickness(24, 20, 24, 8)
            };
            grid.SetDynamicResource(View.MarginProperty, "AirSpacing");
            grid.SetDynamicResource(Grid.RowSpacingProperty, "SpacingSmall");

            var border = new Border
            {
                Margin = new Thickness(-24),
                Stroke = Colors.Transparent,
                StrokeShape = new RoundRectangle { CornerRadius = 28 },
                Shadow = new Shadow
                {
                    Brush = new SolidColorBrush(Color.FromArgb("#40000000")),
                    Offset = new Point(0, 2),
                    Radius = 12,
                    Opacity = 0.3f
                }
            };
            border.SetBinding(Border.BackgroundColorProperty,
                new Binding(nameof(AvaePopupPage.PopupBackground), source: RelativeBindingSource.TemplatedParent));
            Grid.SetRowSpan(border, 2);
            grid.Add(border);

            var titleLabel = new Label
            {
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Start
            };
            titleLabel.SetBinding(Label.TextProperty,
                new Binding(nameof(AvaePopupPage.Title), source: RelativeBindingSource.TemplatedParent));
            titleLabel.SetDynamicResource(Label.TextColorProperty, "TextColor");

            var contentPresenter = new ContentPresenter
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            var titleStack = new VerticalStackLayout { Spacing = 8 };
            titleStack.Add(titleLabel);
            titleStack.Add(contentPresenter);
            Grid.SetRow(titleStack, 0);
            grid.Add(titleStack);

            var buttonRow = new HorizontalStackLayout
            {
                HorizontalOptions = LayoutOptions.End,
                Spacing = 8,
                Margin = new Thickness(0, 4, 0, 4)
            };
            BindableLayout.SetItemsSource(buttonRow, commands);
            BindableLayout.SetItemTemplate(buttonRow, new DataTemplate(() =>
            {
                var button = new Button
                {
                    BackgroundColor = Colors.Transparent,
                    FontAttributes = FontAttributes.None,
                    CornerRadius = 20,
                    Padding = new Thickness(12, 0),
                    BorderWidth = 0
                };
                button.SetBinding(Button.TextProperty, "Name");
                button.SetBinding(Button.CommandProperty, "Command");
                button.SetDynamicResource(Button.TextColorProperty, "PrimaryColor");
                return button;
            }));

            Grid.SetRow(buttonRow, 1);
            grid.Add(buttonRow);

            return grid;
        }

        // ===================== Windows — WinUI ContentDialog style =====================
        private static object BuildWindowsTemplate(string title, IEnumerable commands)
        {
            var grid = new Grid
            {
                WidthRequest = IdiomValue(-1, 380, 380),
                VerticalOptions = LayoutOptions.Center,
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto),
                }
            };

            var border = new Border
            {
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle { CornerRadius = 8 },
                // Original XAML had this binding commented out:
                // BackgroundColor = {Binding PopupBackground, Source={RelativeSource TemplatedParent}}
                //Shadow = new Shadow
                //{
                //    Brush = new SolidColorBrush(Color.FromArgb("#40000000")),
                //    Offset = new Point(0, 4),
                //    Radius = 16,
                //    Opacity = 0.2f
                //}
                //Shadow = new Shadow
                //{
                //    Brush = new SolidColorBrush(Colors.Black),
                //    Offset = new Point(0, 8),
                //    Radius = 32,
                //    Opacity = 0.4f
                //}
            };

            

            Color? foreground = null;
            Color? background = null;
            Color? borderbrush = null;
            Color? overlaybrush = null;
#if WINDOWS

            if (TryGetThemedBrush("ContentDialogBorderBrush", out var borderBrush) && borderBrush is not null)
            {
                var c = borderBrush.Color;
                borderbrush = Color.FromRgba(c.R, c.G, c.B, c.A);
            }

            if (TryGetThemedBrush("ContentDialogBackgroundThemeBrush", out var bgBrush) && bgBrush is not null)
            {
                var c = bgBrush.Color;
                background = Color.FromRgba(c.R, c.G, c.B, c.A);
            }

            if (TryGetThemedBrush("ContentDialogTopOverlay", out var bgOverlay) && bgBrush is not null)
            {
                var c = bgBrush.Color;
                overlaybrush = Color.FromRgba(c.R, c.G, c.B, c.A);
            }

            if (TryGetThemedBrush("ContentDialogForeground", out var fgBrush) && fgBrush is not null)
            {
                var c = fgBrush.Color;
                foreground = Color.FromRgba(c.R, c.G, c.B, c.A);
            }
#endif
            border.SetValue(Border.BackgroundColorProperty, background);// new SolidColorBrush(Color.FromArgb("#FF202020")));
            //border.Effects.Add(Effect = "drop-shadow(0 8 32 #66000000)")
            //border.SetValue(Border.BackgroundColorProperty, background);
            border.SetValue(Border.StrokeProperty, borderbrush);
            //border.SetDynamicResource(Border.BackgroundColorProperty, "ContentDialogBackground");
            //border.SetDynamicResource(Border.StrokeProperty, "ContentDialogBackground");
            Grid.SetRowSpan(border, 3);
            grid.Add(border);

            var titleLabel = new Label
            {
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                Padding = new Thickness(24, 24, 24, 0)
            };
            titleLabel.SetValue(Label.TextProperty, title);
            if (foreground != null)
                titleLabel.SetValue(Label.TextColorProperty, foreground);
            else
                titleLabel.SetDynamicResource(Label.TextColorProperty, "TextColor");
            Grid.SetRow(titleLabel, 0);
            grid.Add(titleLabel);

            var inner = new Border()
            {
                BackgroundColor = overlaybrush
            };
            var contentPresenter = new ContentPresenter { Padding = new Thickness(24, 12, 24, 24) };
            inner.Content = contentPresenter;
            Grid.SetRow(inner, 1);
            grid.Add(inner);

            var buttonRow = new Grid
            {
                ColumnSpacing = 2,
                HeightRequest = 44
            };
            buttonRow.SetBinding(Grid.ColumnDefinitionsProperty,
                new Binding(nameof(Definitions), source: RelativeBindingSource.TemplatedParent));

            BindableLayout.SetItemsSource(buttonRow, commands);
            BindableLayout.SetItemTemplate(buttonRow, new DataTemplate(() =>
            {
                var button = new Button
                {
                    CornerRadius = 4,
                    BorderWidth = 0,
                    Padding = 0,
                    FontSize = 14,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };
                button.SetBinding(Button.TextProperty, "Name");
                button.SetBinding(Button.CommandProperty, "Command");
                button.SetBinding(Grid.ColumnProperty, "Index");
                //button.SetBinding(Button.BackgroundColorProperty,
                //    new Binding("IsPrimary", converter: new BoolToAccentOrNeutralConverter()));
                //button.SetBinding(Button.TextColorProperty,
                //    new Binding("IsPrimary", converter: new BoolToWhiteOrTextColorConverter()));
                if (foreground != null)
                    button.SetValue(Button.TextColorProperty, foreground);
                return button;
            }));

            Grid.SetRow(buttonRow, 2);
            grid.Add(buttonRow);

            return grid;
        }

        // ===================== Helpers =====================

        /// <summary>
        /// C# equivalent of the XAML OnIdiom markup extension for double values.
        /// </summary>
        private static double IdiomValue(double defaultValue, double tablet, double desktop)
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
                return tablet;
            if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
                return desktop;
            return defaultValue;
        }

#if WINDOWS
        public static bool TryGetThemedBrush(string key, out Microsoft.UI.Xaml.Media.SolidColorBrush? brush)
        {
            brush = null;
            var app = Microsoft.UI.Xaml.Application.Current;
            var themeKey = app.RequestedTheme == Microsoft.UI.Xaml.ApplicationTheme.Dark ? "Dark" : "Light";

            foreach (var merged in app.Resources.MergedDictionaries)
            {
                if (merged.ThemeDictionaries.TryGetValue(themeKey, out var themeObj) &&
                    themeObj is Microsoft.UI.Xaml.ResourceDictionary themeDict &&
                    themeDict.TryGetValue(key, out var value) &&
                    value is Microsoft.UI.Xaml.Media.SolidColorBrush b)
                {
                    brush = b;
                    return true;
                }
            }

            // Fallback: some entries live at the top level, not nested under ThemeDictionaries
            if (app.Resources.TryGetValue(key, out var flatValue) && flatValue is Microsoft.UI.Xaml.Media.SolidColorBrush flatBrush)
            {
                brush = flatBrush;
                return true;
            }

            if (app.Resources.TryGetValue(key, out var e))
            {
                //brush = b;
                return true;
            }

            return false;
        }
#endif
    }


    public partial class AvaePopupPage<TResult>(string title, ObservableCollection<NamedCommand> commands) : AvaePopupPage(title, commands), IPopupResultPage<TResult?>
    {
        public TResult? Result { get; set; }

        public void SetResult(TResult? result)
        {
            Result = result;
        }
    }
}
