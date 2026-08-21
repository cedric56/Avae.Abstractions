using Avae.Services;

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
    private string? tag;
    public string? Tag { get => tag; set
        {
            if (uint.TryParse(value, out var id))
                tag = id.ToString();
            else
                throw new InvalidOperationException("Tag is reserved to identify notification");
        }
    }
    public string? Message { get; set; }
    public TimeSpan? Expiration { get; set; }
    public string? Icon { get; set; }

    public IReadOnlyList<SystemNotificationAction>? Actions { get; private set; }
    public IEnumerable<int>? Vibrate { get; set; }

    public void Close()
    {

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
