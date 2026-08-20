using System.Text.Json.Serialization;

namespace Avae.Blazor.Notifications;

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

[JsonSerializable(typeof(InnerData))]
[JsonSerializable(typeof(NotificationData))]
[JsonSerializable(typeof(NotificationReplyData))]
partial class NotificationJsonContext : JsonSerializerContext
{
}
