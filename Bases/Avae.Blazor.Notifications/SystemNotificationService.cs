using Avae.Services;

namespace Avae.Blazor.Notifications;

internal class SystemNotificationService : ISystemNotificationService
{
    public Task<ISystemNotification?> CreateNotification(string action, string title, string message, SystemNotificationAction[] actions)
    {
        throw new NotImplementedException();
    }
}
