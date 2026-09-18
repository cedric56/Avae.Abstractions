using Avae.Services;
using Avae.ViewModels;

namespace Avae.Maui;

internal class DialogService(IServiceProvider provider, IIocConfiguration configuration) : IDialogService
{
#if WINDOWS

    class ContentDialogEx : Microsoft.UI.Xaml.Controls.ContentDialog
    {
        public bool IsClosed { get; set; }        
    }

    public Microsoft.UI.Xaml.Controls.ContentDialog? GetPrevious()
    {
        var current = Current;
        if (current == null)
            throw new InvalidNavigationException($"{nameof(Current)} can not be null");

        var xamlRoot = current.Handler?.PlatformView is Microsoft.UI.Xaml.UIElement page
            ? page.XamlRoot
            : null;
        if (xamlRoot == null) return null;

        var popups = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetOpenPopupsForXamlRoot(xamlRoot);
        return popups.Where(p => p.Child is Microsoft.UI.Xaml.Controls.ContentDialog)
            .Select(c => c.Child as Microsoft.UI.Xaml.Controls.ContentDialog)
            .FirstOrDefault();
    }

    /// <summary>
    /// Hides any currently-open ContentDialog, runs <paramref name="action"/>, then restores
    /// the previous dialog afterward (unless it was itself explicitly closed in the meantime).
    /// </summary>
    private async Task<TResult> WithPreviousSuspendedAsync<TResult>(Func<Task<TResult>> action)
    {
        var previous = GetPrevious();
        if (previous != null)
        {
            previous.Hide();
            previous.Closed += Closed;
        }

        try
        {
            return await action();
        }
        finally
        {
            if (previous != null)
                await previous.ShowAsync();
        }

        void Closed(
            Microsoft.UI.Xaml.Controls.ContentDialog sender,
            Microsoft.UI.Xaml.Controls.ContentDialogClosedEventArgs e)
        {
            previous!.Closed -= Closed;
            if (sender is ContentDialogEx ex && ex.IsClosed)
                previous = null;
        }
    }
#else
// No-op passthrough on non-Windows platforms — nothing to suspend/restore.
private Task<TResult> WithPreviousSuspendedAsync<TResult>(Func<Task<TResult>> action) => action();
#endif


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
    public Task ShowErrorAsync(Exception ex, string title = "Error") =>
        WithPreviousSuspendedAsync(async () =>
        {
            await Current.DisplayAlertAsync(title, ex.Message, "Ok");
            return true;
        });

    /// <summary>
    /// Displays an alert with a single "Ok" button.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns>A task representing the asynchronous display operation.</returns>
    public Task ShowOkAsync(string message, string title = "Title") =>
        WithPreviousSuspendedAsync(async () =>
        {
            await Current.DisplayAlertAsync(title, message, "Ok");
            return true;
        });

    /// <summary>
    /// Displays an alert with "Yes" and "No" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns><see langword="true"/> if the user selected "Yes"; otherwise <see langword="false"/>.</returns>
    public Task<bool> ShowYesNoAsync(string message, string title = "Title") =>
        WithPreviousSuspendedAsync(() => Current.DisplayAlertAsync(title, message, "Yes", "No"));

    /// <summary>
    /// Displays an alert with "Ok" and "Cancel" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns><see langword="true"/> if the user selected "Ok"; otherwise <see langword="false"/>.</returns>
    public Task<bool> ShowOkCancelAsync(string message, string title = "Title") =>
        WithPreviousSuspendedAsync(() => Current.DisplayAlertAsync(title, message, "Ok", "Cancel"));

    /// <summary>
    /// Displays an alert with "Ok" and "Abort" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns><see langword="true"/> if the user selected "Ok"; otherwise <see langword="false"/>.</returns>
    public Task<bool> ShowOkAbortAsync(string message, string title = "Title") =>
        WithPreviousSuspendedAsync(() => Current.DisplayAlertAsync(title, message, "Ok", "Abort"));

    /// <summary>
    /// Displays an alert with "Yes", "No", and "Cancel" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns>0 if "Yes" was selected, 1 if "No" was selected, or 2 if "Cancel" was selected.</returns>
    public Task<int> ShowYesNoCancelAsync(string message, string title = "Title") =>
        DisplayThreeButtons(title, message, "Yes", "No", "Cancel", 0, 1, 2);

    /// <summary>
    /// Displays an alert with "Yes", "No", and "Abort" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns>0 if "Yes" was selected, 1 if "No" was selected, or 2 if "Abort" was selected.</returns>
    public Task<int> ShowYesNoAbortAsync(string message, string title = "Title") =>
        DisplayThreeButtons(title, message, "Yes", "No", "Abort", 0, 1, 2);

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

        var viewModel = provider.GetViewModel<TViewModel>(context);
        var view = configuration.GetModalFor<TViewModel, TResult>(context ?? new NavigableContext()) ?? throw new InvalidOperationException($"Unable to create view for {typeof(TViewModel).Name}.  Ensure that it is registered in the container.");
        view.Context = viewModel;

        var taskCompletionSource = new TaskCompletionSource<TResult?>();
#if ANDROID
        viewModel.CloseRequested += CloseRequested;

        var alertBuilder = new Android.App.AlertDialog.Builder(Platform.CurrentActivity);

        alertBuilder.SetTitle(viewModel.Title);
        alertBuilder.SetView(Microsoft.Maui.Platform.ElementExtensions.ToPlatform((View)view, Current?.Handler?.MauiContext ?? new MauiContext(provider)));

        if (viewModel.Commands.Count > 0)
            alertBuilder.SetPositiveButton(viewModel.Commands[0].Name, (senderAlert, args) =>
            {
                if (viewModel.Commands[0].Command.CanExecute(null))
                    viewModel.Commands[0].Command.Execute(null);
            });
        if (viewModel.Commands.Count > 1)
            alertBuilder.SetNegativeButton(viewModel.Commands[1].Name, (senderAlert, args) =>
            {
                if (viewModel.Commands[1].Command.CanExecute(null))
                    viewModel.Commands[1].Command.Execute(null);
            });
        if (viewModel.Commands.Count > 2)
            alertBuilder.SetNeutralButton(viewModel.Commands[2].Name, (senderAlery, args) =>
            {
                if (viewModel.Commands[2].Command.CanExecute(null))
                    viewModel.Commands[2].Command.Execute(null);
            });

        var alertDialog = alertBuilder.Create();
        alertDialog?.Show();

        return await taskCompletionSource.Task;

        void CloseRequested(object? sender, TResult? result)
        {
            viewModel.CloseRequested -= CloseRequested;
            taskCompletionSource.SetResult(result);
        }
#elif MACCATALYST || IOS
        var vc = new UIKit.UIViewController { ModalPresentationStyle = UIKit.UIModalPresentationStyle.FormSheet };
        vc.PreferredContentSize = new CoreGraphics.CGSize(320, 360);
        viewModel.CloseRequested += CloseRequested;
        var stack = new UIKit.UIStackView { Axis = UIKit.UILayoutConstraintAxis.Vertical, Spacing = 12 };
        stack.TranslatesAutoresizingMaskIntoConstraints = false;

        if (!string.IsNullOrEmpty(viewModel.Title))
        {
            var label = new UIKit.UILabel
            {
                Text = viewModel.Title
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
        //contentView.TranslatesAutoresizingMaskIntoConstraints = false;
        stack.AddArrangedSubview(Microsoft.Maui.Platform.ElementExtensions.ToPlatform((View)view, Current?.Handler?.MauiContext ?? new MauiContext(provider)));

        var buttons = new UIKit.UIStackView
        {
            Axis = UIKit.UILayoutConstraintAxis.Horizontal,
            Spacing = 8,
            Distribution = UIKit.UIStackViewDistribution.FillEqually
        };

        void AddButton(NamedCommand? command)
        {
            if (string.IsNullOrEmpty(command?.Name)) return;
            var button = UIKit.UIButton.FromType(UIKit.UIButtonType.System);
            button.SetTitle(command.Name, UIKit.UIControlState.Normal);
            button.TouchUpInside += (_, _) =>
            {
                if (command.Command.CanExecute(null))
                    command.Command.Execute(null);
            };
            buttons.AddArrangedSubview(button);
        }
        AddButton(viewModel.Commands.ElementAtOrDefault(0));
        AddButton(viewModel.Commands.ElementAtOrDefault(1));
        AddButton(viewModel.Commands.ElementAtOrDefault(2));
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

        void CloseRequested(object? sender, TResult? result)
        {
            viewModel.CloseRequested -= CloseRequested;
            taskCompletionSource.SetResult(result);
            vc.DismissViewController(true, null);
        }
#elif WINDOWS
        return await WithPreviousSuspendedAsync(async () =>
        {
            var current = Current;
            if (current == null)
                throw new InvalidNavigationException($"{nameof(Current)} can not be null");

            var xamlRoot = current.Handler?.PlatformView is Microsoft.UI.Xaml.UIElement page
                ? page.XamlRoot
                : null;

            var dialog = new ContentDialogEx
            {
                RequestedTheme = Application.Current?.RequestedTheme == AppTheme.Dark
                    ? Microsoft.UI.Xaml.ElementTheme.Dark
                    : Application.Current?.RequestedTheme == AppTheme.Light
                        ? Microsoft.UI.Xaml.ElementTheme.Light
                        : Microsoft.UI.Xaml.ElementTheme.Default,
                Title = viewModel.Title,
                Content = Microsoft.Maui.Platform.ElementExtensions.ToPlatform((View)view, Current?.Handler?.MauiContext ?? new MauiContext(provider)),
                PrimaryButtonCommand = viewModel.Commands.ElementAtOrDefault(0)?.Command,
                PrimaryButtonText = viewModel.Commands.ElementAtOrDefault(0)?.Name,
                SecondaryButtonCommand = viewModel.Commands.ElementAtOrDefault(1)?.Command,
                SecondaryButtonText = viewModel.Commands.ElementAtOrDefault(1)?.Name,
                CloseButtonCommand = viewModel.Commands.ElementAtOrDefault(2)?.Command,
                CloseButtonText = viewModel.Commands.ElementAtOrDefault(2)?.Name,
                XamlRoot = xamlRoot
            };

            viewModel.CloseRequested += CloseRequested;
            await dialog.ShowAsync();
            return await taskCompletionSource.Task;

            void CloseRequested(object? sender, TResult? result)
            {
                viewModel.CloseRequested -= CloseRequested;
                dialog.IsClosed = true;
                taskCompletionSource.SetResult(result);
            }
        });        
#endif
        throw new NotImplementedException();
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
            content = Microsoft.Maui.Platform.ElementExtensions.ToPlatform(e, Current?.Handler?.MauiContext ?? new MauiContext(provider));
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
        return await WithPreviousSuspendedAsync(async () =>
        {
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
        });
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
}
