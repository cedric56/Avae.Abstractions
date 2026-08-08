using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;

namespace Avae.Implementations.Services
{
    internal class NotificationService : Avae.Services.INotificationManager
    {
        static WindowNotificationManager? manager = null!;

        public int MaxItems { get => 5; }

        public Avae.Services.NotificationPosition Position => Avae.Services.NotificationPosition.TopCenter;

        public void Show(string title, string message, Avae.Services.NotificationType type = default, TimeSpan? expiration = null, Action? onClick = null, Action? onClose = null)
        {
            var topLevel = TopLevelStateManager.Default.GetActive();
            if (TopLevel.GetTopLevel(manager) != topLevel)
            {
                manager = new WindowNotificationManager(topLevel)
                {
                    Position = (Avalonia.Controls.Notifications.NotificationPosition)Position,
                    MaxItems = MaxItems
                };
            }

            Dispatcher.UIThread.Invoke(() =>
            {
                var notification = new Notification(title, message, (Avalonia.Controls.Notifications.NotificationType)type, expiration, onClick, onClose);
                manager?.Show(notification);
            });
        }
    }
}
