using System.Text.Json.Serialization;

namespace Avae.Blazor.Notifications;

public class WebNotification
{
    public string? Title { get; set; }
    public string? Body { get; set; }
    public string? Tag { get; set; }
    public string? Icon { get; set; }
    public string? Badge { get; set; }
    public string? Image { get; set; }
    public string? Lang { get; set; }
    public string? Dir { get; set; }
    public bool RequireInteraction { get; set; }
    public bool? Silent { get; set; }
    public long Timestamp { get; set; }          // or DateTimeOffset if you convert
    public object? Data { get; set; }
    public WebNotificationAction[]? Actions { get; set; }
}

public class WebNotificationAction
{
    public string? Action { get; set; }
    public string? Title { get; set; }
    public string? Icon { get; set; }
    public string? Type { get; set; }   // "button" | "text"
}

class NotificationData
{
    public string? action { get; set; }
    public InnerData? data { get; set; }
}

class InnerData
{
    public uint? id { get; set; }
}

class NotificationReplyData : NotificationData
{
    public string? Reply { get; set; }
}

[JsonSerializable(typeof(WebNotification))]
[JsonSerializable(typeof(WebNotificationAction))]
[JsonSerializable(typeof(InnerData))]
[JsonSerializable(typeof(NotificationData))]
[JsonSerializable(typeof(NotificationReplyData))]
partial class NotificationJsonContext : JsonSerializerContext
{
}
