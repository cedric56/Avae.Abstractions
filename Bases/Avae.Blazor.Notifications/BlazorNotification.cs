using Avae.Services;
using Avalonia.Labs.Notifications;

namespace Avae.Blazor.Notifications;

class BlazorNotification : ISystemNotification
{
    private static uint s_currentId = 0;
    SystemNotificationService manager;
    string? category;

    public BlazorNotification(NotificationChannel channel, SystemNotificationService manager)
    {
        this.category = channel.Id;
        this.manager = manager;
        this.Actions = [.. channel.Actions.Select(a => new SystemNotificationAction(a.Caption, a.Tag) { })];
        //this.ChannelIcon = channel.Icon;
        //this.Vibrations = channel.Vibrations ?? [];

        Id = GetNextId();
    }

    public uint Id { get; }
    public string? ReplyActionTag { get; set; }
    public int[] Vibrations { get; set; } = [];
    public string? Category => category;
    public string? ChannelIcon { get; set; }
    public string? Title { get; set; }
    public string? Tag { get; set; }
    public string? Message { get; set; }
    public TimeSpan? Expiration { get; set; }
    public IReadOnlyList<SystemNotificationAction>? Actions { get; private set; }
    public string? Icon { get; set; }

    public async void Close()
    {
        await manager.Close(Id);
    }

    public async void Show()
    {
        string? icon = null;
        //if (ChannelIcon == null && Icon != null)
        //{
        //    using var ms = new MemoryStream();
        //    Icon.Save(ms);
        //    var bytes = ms.ToArray();
        //    var base64 = Convert.ToBase64String(bytes);
        //    icon = $"data:image/png;base64,{base64}";
        //}

        await manager.Show(this, new NotificationOptions()
        {
            Actions = Actions?.Select(a => new NotificationAction { Action = a.tag, Icon = a.Icon, Title = a.caption, Type = a.tag == ReplyActionTag ? "text" : "button" }).ToArray() ?? [],
            Body = Message,
            Data = new NotificationData
            {
                Id = Id,
                ReplyActionTag = ReplyActionTag,
            },
            Icon = ChannelIcon ?? icon,
            Tag = Tag,
            Vibrations = Vibrations
        });
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