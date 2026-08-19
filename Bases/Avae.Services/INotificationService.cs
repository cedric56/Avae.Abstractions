namespace Avae.Services;

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

public interface INotificationService
{
    void Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Action? onClick = null, Action? onClose = null);
}
