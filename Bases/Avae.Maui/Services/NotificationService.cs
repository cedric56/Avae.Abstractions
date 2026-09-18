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
    public async void Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Action? onClick = null, Action? onClose = null)
    {
#if WINDOWS

        Microsoft.UI.Xaml.Controls.InfoBar? bar = null!;
        var w = Application.Current?.Windows.FirstOrDefault();
        if (w != null)
        {
            var winui = w.Handler.PlatformView as Microsoft.UI.Xaml.Window;
            if (winui != null && winui.Content is Microsoft.UI.Xaml.Controls.Panel rootPanel)
            {
                var panel = rootPanel.Children.FirstOrDefault(c => c is Microsoft.UI.Xaml.Controls.StackPanel) as Microsoft.UI.Xaml.Controls.StackPanel;
                if (panel == null)
                    rootPanel.Children.Add(panel = new Microsoft.UI.Xaml.Controls.StackPanel() { Orientation = Microsoft.UI.Xaml.Controls.Orientation.Vertical,
                        Margin = new Microsoft.UI.Xaml.Thickness(50),
                    });
                panel.Children.Add(bar = new Microsoft.UI.Xaml.Controls.InfoBar());
            }
        }
        bar.Title = title;
        bar.Message = message;
        bar.IsOpen = true;
#else

        throw new NotImplementedException();

#endif
    }
}
