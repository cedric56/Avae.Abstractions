using Avae.ViewModels;

namespace Avae.Maui;

public static class ModalService
{
    public static async Task<TResult?> ShowModalAsync<TViewModel, TResult>(
        object context, View view)
         where TViewModel : ICloseableViewModel<TResult>
    {
        var viewModel = (TViewModel)context;
        var taskCompletionSource = new TaskCompletionSource<TResult?>();
#if ANDROID
        viewModel.CloseRequested += CloseRequested;

        var alertBuilder = new Android.App.AlertDialog.Builder(Platform.CurrentActivity);

        alertBuilder.SetTitle(viewModel.Title);
        alertBuilder.SetView(Microsoft.Maui.Platform.ElementExtensions.ToPlatform((View)view, DialogService.Current?.Handler?.MauiContext ?? throw new InvalidOperationException("Unable to find MauiContext")));

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

        var dialog = alertBuilder.Create();
        DialogService.manager = new DialogService.AlertDialogManager(dialog);
        dialog?.Show();

        return await taskCompletionSource.Task;

        void CloseRequested(object? sender, TResult? result)
        {
            viewModel.CloseRequested -= CloseRequested;
            DialogService.manager?.IsClosed = true;
            DialogService.manager = null;
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
        stack.AddArrangedSubview(Microsoft.Maui.Platform.ElementExtensions.ToPlatform((View)view, DialogService.Current?.Handler?.MauiContext ?? throw new InvalidOperationException("Unable to find MauiContext")));

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
        return await DialogService.WithPreviousSuspendedAsync(async () =>
        {
            var current = DialogService.Current ?? throw new InvalidNavigationException($"{nameof(DialogService.Current)} can not be null");
            var xamlRoot = current.Handler?.PlatformView is Microsoft.UI.Xaml.UIElement page
                ? page.XamlRoot
                : null;

            var dialog = new DialogService.ContentDialogEx
            {
                RequestedTheme = Application.Current?.RequestedTheme == AppTheme.Dark
                    ? Microsoft.UI.Xaml.ElementTheme.Dark
                    : Application.Current?.RequestedTheme == AppTheme.Light
                        ? Microsoft.UI.Xaml.ElementTheme.Light
                        : Microsoft.UI.Xaml.ElementTheme.Default,
                Title = viewModel.Title,
                Content = Microsoft.Maui.Platform.ElementExtensions.ToPlatform(view, DialogService.Current?.Handler?.MauiContext ?? throw new InvalidOperationException("Unable to find MauiContext")),
                PrimaryButtonCommand = viewModel.Commands.ElementAtOrDefault(0)?.Command,
                PrimaryButtonText = viewModel.Commands.ElementAtOrDefault(0)?.Name,
                SecondaryButtonCommand = viewModel.Commands.ElementAtOrDefault(1)?.Command,
                SecondaryButtonText = viewModel.Commands.ElementAtOrDefault(1)?.Name,
                CloseButtonCommand = viewModel.Commands.ElementAtOrDefault(2)?.Command,
                CloseButtonText = viewModel.Commands.ElementAtOrDefault(2)?.Name,
                XamlRoot = xamlRoot
            };

            viewModel.CloseRequested += CloseRequested;
            dialog.Closed += Closed;
            await dialog.ShowAsync();
            return await taskCompletionSource.Task;

            void CloseRequested(object? sender, TResult? result)
            {
                viewModel.CloseRequested -= CloseRequested;
                dialog.IsSelfHiding = false;
                dialog.IsClosed = true;
                dialog.Closed -= Closed;
                taskCompletionSource.TrySetResult(result);
            }

            void Closed(Microsoft.UI.Xaml.Controls.ContentDialog sender, Microsoft.UI.Xaml.Controls.ContentDialogClosedEventArgs args)
            {
                if (dialog.IsSelfHiding)
                    return;
                viewModel.CloseRequested -= CloseRequested;
                dialog.IsClosed = true;
                dialog.Closed -= Closed;
                taskCompletionSource.TrySetResult(default);
            }
        });
#endif
        throw new NotImplementedException();
    }
}
