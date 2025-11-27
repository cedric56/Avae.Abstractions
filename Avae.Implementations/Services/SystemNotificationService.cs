using Avae.Abstractions.Interfaces;
using Avalonia.Labs.Notifications;
using Moq;

namespace Avae.Implementations.Services
{
    public class SystemNotificationService : ISystemNotificationService
    {
        public ISystemNotification CreateNotification(string action, string title, string message, SystemNotificationAction[] actions)
        {
            var mock = new Mock<ISystemNotification>();
            var _currentNotification = NativeNotificationManager.Current?.CreateNotification(action);
            if (_currentNotification is not null)
            {
                _currentNotification.Title = title;
                _currentNotification.Message = message;
                _currentNotification.SetActions(actions.Select(a => new NativeNotificationAction(a.caption, a.tag)).ToList());

                mock.Setup(m => m.Show()).Callback(() => _currentNotification.Show());
                mock.Setup(m => m.Close()).Callback(() => _currentNotification.Close());

                var manager = NativeNotificationManager.Current;
                if (manager != null)
                {
                    manager.NotificationCompleted += (sender, e) =>
                    {
                        mock.Raise(args => args.NotificationCompleted += null, new SystemNotificationEventArgs()
                        {
                            ActionTag = e.ActionTag,
                            IsActivated = e.IsActivated,
                            IsCancelled = e.IsCancelled,
                            NotificationId = e.NotificationId,
                            UserData = e.UserData,
                        });

                        //_currentNotification?.Close();
                    };
                }
            }
            return mock.Object;
        }
    }
}
