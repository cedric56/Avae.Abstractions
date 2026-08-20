using Avae.Services;

namespace Avae.Blazor.Notifications;

class BlazorNotification : ISystemNotification
{
    private static uint s_currentId = 0;
    Func<uint, string?, Task> show;

    public BlazorNotification(Func<uint, string?, Task> show)
    {
        this.show = show;

        Id = GetNextId();
    }

    public uint Id { get; }
    public string? ReplyActionTag {  get; set; }

    public event EventHandler<SystemNotificationEventArgs>? NotificationCompleted;

    public void Close()
    {
        throw new NotImplementedException();
    }

    public async void Show()
    {
        await show(Id, ReplyActionTag);
    }

    public void RaiseCompleted(SystemNotificationEventArgs e)
    {
        NotificationCompleted?.Invoke(this, e);
    }

    private static uint GetNextId()
    {
        return Interlocked.Increment(ref s_currentId);
    }
}
