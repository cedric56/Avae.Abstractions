using Avae.Services;
using System.Formats.Asn1;

namespace Avae.Blazor.Notifications;

class BlazorNotification : ISystemNotification
{
    private static uint s_currentId = 0;
    Func<BlazorNotification, Task> show;
    string? category;

    public BlazorNotification(string? category, Func<BlazorNotification, Task> show)
    {
        this.category = category;
        this.show = show;

        Id = GetNextId();
    }

    internal BlazorNotification(uint id)
    {
        Id = id;
        show = _ => Task.CompletedTask;
    }

    public uint Id { get; }
    public string? ReplyActionTag {  get; set; }

    public string? Category => category;

    public string? Title { get; set; }
    public string? Tag { get; set; }
    public string? Message { get; set; }
    public TimeSpan? Expiration { get; set; }
    public string? Icon { get; set; }

    public IReadOnlyList<SystemNotificationAction>? Actions { get; private set; }
    public IEnumerable<int>? Vibrate { get; set; }

    public void Close()
    {
        //throw new NotImplementedException();
    }

    public async void Show()
    {
        await show(this);
    }

    private static uint GetNextId()
    {
        return Interlocked.Increment(ref s_currentId);
    }

    public void SetActions(IReadOnlyList<SystemNotificationAction>? actions)
    {
        Actions = actions;
    }
}
