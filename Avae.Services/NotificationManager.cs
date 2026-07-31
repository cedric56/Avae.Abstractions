namespace Avae.Services
{
    public enum NotificationType
    {
        Information,
        Success,
        Warning,
        Error
    }

    public enum NotificationPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        TopCenter,
        BottomCenter
    }

    public interface INotificationManager
    {
        int MaxItems { get; }

        NotificationPosition Position { get; }

        void Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Action? onClick = null, Action? onClose = null);
    }

    //public static class NotificationManager
    //{
    //    static INotificationManager? _default;

    //    static INotificationManager Default { get { return _default ?? throw new InvalidOperationException(""); } }

    //    public static void SetDefaut(INotificationManager notificationManager)
    //    {
    //        _default = notificationManager;
    //    }

    //    public static void Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Action? onClick = null, Action? onClose = null)
    //    {
    //        Default.Show(title, message, type, expiration, onClick, onClose);
    //    }
    //}
}
