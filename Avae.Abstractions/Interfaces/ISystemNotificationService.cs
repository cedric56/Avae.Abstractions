namespace Avae.Abstractions.Interfaces
{
    public record SystemNotificationAction(string caption, string tag)
    {
        public string? Icon { get; init; }
    }

    public class SystemNotificationEventArgs : EventArgs
    {
        public string? ActionTag { get; init; }
        public uint? NotificationId { get; init; }
         
        // could be used for text input
        public object? UserData { get; init; }
        public bool IsCancelled { get; init; }
        public bool IsActivated { get; init; }
    }

    public interface ISystemNotification
    {
        event EventHandler<SystemNotificationEventArgs> NotificationCompleted;
        void Close();
        void Show();
    }

    public interface ISystemNotificationService
    {
        ISystemNotification CreateNotification(string action, string title, string message, SystemNotificationAction[] actions);
    }
}
