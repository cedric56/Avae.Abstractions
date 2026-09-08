using Microsoft.Maui.Platform;

namespace Avae.Maui.Services;

internal class Helper(IServiceProvider provider) : IDisposable
{
    /// <summary>
    /// Gets the currently active MAUI page: the page of the activated window if one exists,
    /// otherwise the first available window's page, otherwise <see cref="Shell.Current"/>.
    /// </summary>
    public Page Current => Application.Current?.Windows.FirstOrDefault(w => w.IsActivated)?.Page ?? Application.Current?.Windows.FirstOrDefault()?.Page ?? Shell.Current;

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
    public void Ensure()
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
        ThemeChanged(null, new AppThemeChangedEventArgs(Application.Current!.RequestedTheme));
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

    public void Dispose()
    {
        Application.Current?.RequestedThemeChanged -= ThemeChanged;
    }
}
