using Avae.Services;
using Avalonia.Labs.Notifications;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace Avae.Avalonia.Notifications;

public class SystemNotificationService : ISystemNotificationService
{
#if ANDROID
    public static Android.App.Activity? Activity { get; set; }
#endif

    INativeNotificationManager? manager;

    public event EventHandler<SystemNotificationEventArgs>? NotificationCompleted;

    public IReadOnlyDictionary<uint, ISystemNotification> ActiveNotifications()
    {
        return manager?.ActiveNotifications
           .ToDictionary(kvp => kvp.Key, kvp => new AvaloniaNotification(kvp.Value))
           .ToDictionary(kvp => kvp.Key, kvp => (ISystemNotification)kvp.Value) ?? [];
    }

    public class AvaloniaNotification(INativeNotification native) : ISystemNotification
    {
        public uint Id => native.Id;

        public string? Category => native.Category;

        public string? Title { get => native.Title; set => native.Title = value; }
        public string? Tag { get => native.Tag; set => native.Tag = value; }
        public string? Message { get => native.Message; set => native.Message = value; }
        public TimeSpan? Expiration { get => native.Expiration; set => native.Expiration = value; }
        public string? Icon { get => native.Icon?.ToString(); set => throw new NotImplementedException(); }
        public string? ReplyActionTag { get => native.ReplyActionTag; set => native.ReplyActionTag = value; }

        private IReadOnlyList<SystemNotificationAction>? actions;
        public IReadOnlyList<SystemNotificationAction>? Actions { get => actions; private set => SetActions(value); }
        public IEnumerable<int>? Vibrate { get; set; }

        public void Close()
        {
            native.Close();
        }

        public void SetActions(IReadOnlyList<SystemNotificationAction>? actions)
        {
            this.actions = actions;
            native.SetActions([.. actions?.Select(a => new NativeNotificationAction(a.caption, a.tag) { Icon = a.Icon == null ? null : new Bitmap(a.Icon) }) ?? []]);
        }

        public void Show()
        {
            native.Show();
        }
    }

    public Task<ISystemNotification?> CreateNotification(string? category)
    {
        if (manager == null)
        {
            manager = NativeNotificationManager.Current;
#if ANDROID
            manager?.SetPermissionActivity(Activity ?? throw new InvalidOperationException("Activity must be set on OnCreateBundle"));
#endif
            
        }

        if (manager != null)
        {
            
            var _currentNotification = manager.CreateNotification(category);
            if (_currentNotification is not null)
            {
                var current = new AvaloniaNotification(_currentNotification);

                manager.NotificationCompleted -= OnNotificationCompleted;
                manager.NotificationCompleted += OnNotificationCompleted;
                void OnNotificationCompleted(object? sender, NativeNotificationCompletedEventArgs args)
                {
                    
                    NotificationCompleted?.Invoke(this, new SystemNotificationEventArgs()
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

    public void CloseAll()
    {
        manager?.CloseAll();
    }
}
