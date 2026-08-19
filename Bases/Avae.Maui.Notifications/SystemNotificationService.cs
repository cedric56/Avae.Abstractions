using Avae.Services;
using Avalonia.Labs.Notifications;

namespace Avae.Maui.Notifications;

public class SystemNotificationService : ISystemNotificationService
{
#if ANDROID
    public static Android.App.Activity? Activity { get; set; }
#endif

    INativeNotificationManager? manager;

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
        if(manager == null)
        {
            manager = NativeNotificationManager.Current;
#if ANDROID
            manager?.SetPermissionActivity(Activity ?? throw new InvalidOperationException("Activity must be set on OnCreateBundle"));
#endif
        }

        if (manager != null)
        {
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
        }

        throw new InvalidOperationException("Notification is not defined");
    }
}
