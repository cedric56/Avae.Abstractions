using Avae.Services;
using Avalonia.Labs.Notifications;

namespace Avae.Implementations.Services
{
    public class SystemNotificationService : ISystemNotificationService
    {
        public class AvaloniaNotification(INativeNotification native) : ISystemNotification
        {
            public event EventHandler<SystemNotificationEventArgs>? NotificationCompleted;

            public void Close()
            {
                native.Close();
            }

            public void Show()
            {
                native.Show();
            }

            public void OnNativeCompleted(SystemNotificationEventArgs args)
            {
                NotificationCompleted?.Invoke(this, args);
            }
        }

        public ISystemNotification CreateNotification(string action, string title, string message, SystemNotificationAction[] actions)
        {
            var _currentNotification = NativeNotificationManager.Current?.CreateNotification(action);
            if (_currentNotification is not null)
            {
                var current = new AvaloniaNotification(_currentNotification);
                _currentNotification.Title = title;
                _currentNotification.Message = message;
                _currentNotification.SetActions(actions.Select(a => new NativeNotificationAction(a.caption, a.tag)).ToList());

                //mock.Setup(m => m.Show()).Callback(() => _currentNotification.Show());
                //mock.Setup(m => m.Close()).Callback(() => _currentNotification.Close());

                var manager = NativeNotificationManager.Current;
                if (manager != null)
                {
                    manager.NotificationCompleted += OnNotificationCompleted;
                    void OnNotificationCompleted(object? sender, NativeNotificationCompletedEventArgs args)
                    {
                        manager.NotificationCompleted -= OnNotificationCompleted;
                        current.OnNativeCompleted(new SystemNotificationEventArgs()
                        {
                            ActionTag = args.ActionTag,
                            IsActivated = args.IsActivated,
                            IsCancelled = args.IsCancelled,
                            NotificationId = args.NotificationId,
                            UserData = args.UserData,
                        });
                    }
                }
                return current;
            }

            throw new InvalidOperationException("Notification is not defined");
        }
    }
}
