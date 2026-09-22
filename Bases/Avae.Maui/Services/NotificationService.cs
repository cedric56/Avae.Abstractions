using Avae.Services;

namespace Avae.Maui;

internal class NotificationService : INotificationService
{
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
    public void Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Action? onClick = null, Action? onClose = null)
    {
#if WINDOWS
        InfoBarHelper.Show(
        title,
        message,
        type switch
        {
            NotificationType.Error => Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error,
            NotificationType.Success => Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success,
            NotificationType.Warning => Microsoft.UI.Xaml.Controls.InfoBarSeverity.Warning,
            _ => Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational
        },
        expiration,
        onClick,
        onClose);
#else
        throw new NotImplementedException();
#endif
    }

#if WINDOWS

    public static class InfoBarHelper
    {
        const string HostTag = "Avae.InfoBarHost";

        public static Microsoft.UI.Xaml.Controls.InfoBar Show(
    string title,
    string message,
    Microsoft.UI.Xaml.Controls.InfoBarSeverity severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational,
    TimeSpan? expiration = null,
    Action? onClick = null,
    Action? onClose = null,
    bool isClosable = true)
        {
            var host = GetOrCreateHost()
                ?? throw new InvalidOperationException("No WinUI window/content to attach InfoBar.");

            var bar = new Microsoft.UI.Xaml.Controls.InfoBar
            {
                Title = title,
                Message = message,
                Severity = severity,
                IsClosable = isClosable,
                IsOpen = true,
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
                Margin = new Microsoft.UI.Xaml.Thickness(12, 8, 12, 0),
            };

            var closed = false;
            Microsoft.UI.Dispatching.DispatcherQueueTimer? timer = null;

            void Dismiss()
            {
                if (closed) return;
                closed = true;

                if (timer is not null)
                {
                    timer.Stop();
                    timer.Tick -= OnTick;
                    timer = null;
                }

                bar.CloseButtonClick -= OnClose;
                bar.PointerPressed -= OnPointerPressed;

                bar.IsOpen = false;
                host.Children.Remove(bar);
                onClose?.Invoke();
            }

            void OnClose(Microsoft.UI.Xaml.Controls.InfoBar sender, object args) => Dismiss();

            void OnPointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
            {
                onClick?.Invoke();
                Dismiss();
                e.Handled = true;
            }

            void OnTick(Microsoft.UI.Dispatching.DispatcherQueueTimer t, object args) => Dismiss();

            bar.CloseButtonClick += OnClose;

            if (onClick is not null)
            {
                bar.IsHitTestVisible = true;
                bar.PointerPressed += OnPointerPressed;
            }

            if (expiration is { } delay && delay > TimeSpan.Zero)
            {
                timer = bar.DispatcherQueue.CreateTimer();
                timer.Interval = delay;
                timer.IsRepeating = false;
                timer.Tick += OnTick;
                timer.Start();
            }

            host.Children.Add(bar);
            return bar;
        }

        static Microsoft.UI.Xaml.Controls.StackPanel? GetOrCreateHost()
        {
            var existing = FindHost();
            if (existing is not null)
                return existing;

            if (GetRootPanel() is not { } root)
                return null;

            var host = new Microsoft.UI.Xaml.Controls.StackPanel
            {
                Tag = HostTag,
                Orientation = Microsoft.UI.Xaml.Controls.Orientation.Vertical,
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
                VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Top,
            };

            if (root is Microsoft.UI.Xaml.Controls.Grid)
                Microsoft.UI.Xaml.Controls.Grid.SetRowSpan(host, 99);

            root.Children.Add(host);
            return host;
        }

        static Microsoft.UI.Xaml.Controls.StackPanel? FindHost()
        {
            if (GetRootPanel() is not { } root)
                return null;

            return root.Children
                .OfType<Microsoft.UI.Xaml.Controls.StackPanel>()
                .FirstOrDefault(p => Equals(p.Tag, HostTag));
        }

        static Microsoft.UI.Xaml.Controls.Panel? GetRootPanel()
        {
            var window = Application.Current?.Windows?.FirstOrDefault();
            var winui = window?.Handler.PlatformView as Microsoft.UI.Xaml.Window;
            return winui?.Content as Microsoft.UI.Xaml.Controls.Panel;
        }
    }

#endif
}
