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
        ToastHelper.Show(title, message, type, expiration, onClick, onClose);
#elif ANDROID
        ToastHelper.Show(title, message, type, expiration, onClick, onClose);
#else
        throw new NotImplementedException();
#endif
    }
}
