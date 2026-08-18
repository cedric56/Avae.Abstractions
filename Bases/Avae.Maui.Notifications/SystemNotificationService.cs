using Avae.Services;
using Avalonia.Labs.Notifications;
using Microsoft.Maui.Platform;

namespace Avae.Maui.Notifications
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

        public Task<ISystemNotification?> CreateNotification(string action, string title, string message, SystemNotificationAction[] actions)
        {
            var manager = NativeNotificationManager.Current;
            if (manager == null)
                throw new InvalidOperationException("Manager is not defined");

//#if ANDROID
//            manager.SetPermissionActivity(Android.App.Application.Context.GetActivity()!);
//#endif

            var _currentNotification = manager.CreateNotification(action);
            if (_currentNotification is not null)
            {
                var current = new AvaloniaNotification(_currentNotification);
                _currentNotification.Title = title;
                _currentNotification.Message = message;
                _currentNotification.SetActions(actions.Select(a => new NativeNotificationAction(a.caption, a.tag)).ToList());

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
                return Task.FromResult<ISystemNotification?>(current);
            }

            throw new InvalidOperationException("Notification is not defined");
        }
    }
}
