namespace Avae.Services;

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
    uint Id { get; }

    // categories are defined at launch. Defines which actions are set by default. on Android, will act as the notification channel
    string? Category { get; }
    string? Title { get; set; }

    string? Tag { get; set; }
    string? Message { get; set; }
    TimeSpan? Expiration { get; set; }

    /// <summary>
    /// Only supports on web
    /// </summary>
    IEnumerable<int>? Vibrate { get; set; }

    string? Icon { get; set; }

    // if set, enables text input in the notification and sets the specified action as the reply action
    string? ReplyActionTag { get; set; }

    // Defined by notification category
    IReadOnlyList<SystemNotificationAction>? Actions { get; }

    // no-op on ios
    void SetActions(IReadOnlyList<SystemNotificationAction>? actions);

    // can be called multiple times to update active notification
    void Show();
    void Close();

    
}

public interface ISystemNotificationService
{
    Task<IReadOnlyDictionary<uint, ISystemNotification>> ActiveNotifications();

    // if null, implementation will set a default category, otherwise category must be defined at launch
    Task<ISystemNotification?> CreateNotification(string? category);
    void CloseAll();

    event EventHandler<SystemNotificationEventArgs>? NotificationCompleted;
}
