using Avae.Services;
using Microsoft.JSInterop;
using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Avae.Blazor.Notifications;

internal class SystemNotificationService : ISystemNotificationService, IAsyncDisposable
{
    Dictionary<uint, BlazorNotification> dic = new();

    public enum PermissionType
    {
        Default = 0,
        Granted,
        Denied
    }

    private static readonly JsonSerializerOptions jsonSerializerOptionsForPropertyModel = new()
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
    };

    IJSRuntime jSRuntime;

    public SystemNotificationService(IJSRuntime jSRuntime)
    {
        this.jSRuntime = jSRuntime;

        _selfRef = DotNetObjectReference.Create(this);
    }

    
    private DotNetObjectReference<SystemNotificationService>? _selfRef;

    private IJSObjectReference? _module;
    private PermissionType? permissionType;

    private async ValueTask<IJSObjectReference> GetModuleAsync()
    {
        if (_module == null)
        {
            _module = await jSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Avae.Blazor.Notifications/notifications.js");
            await _module.InvokeVoidAsync("registrations.registerDotnet", _selfRef);
        }
        return _module;
    }

    [JSInvokable]
    public async Task HandleNotificationClose(object data)
    {
        var jsonString = JsonSerializer.Serialize(data);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
        var obj = await JsonSerializer.DeserializeAsync<NotificationData>(stream, jsonSerializerOptionsForPropertyModel);
        if (obj != null && obj.data?.id is uint id && dic.TryGetValue(id, out var item))
        {
            item.RaiseCompleted(new SystemNotificationEventArgs()
            {
                IsCancelled = true,
                NotificationId = id
            });
            dic.Remove(id);
        }
    }

    [JSInvokable]
    public async Task HandleNotificationClick(object data)
    {
        var jsonString = JsonSerializer.Serialize(data);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
        var obj = await JsonSerializer.DeserializeAsync<NotificationData>(stream, jsonSerializerOptionsForPropertyModel);
        if (obj != null && obj.data?.id is uint id && dic.TryGetValue(id, out var item))
        {
            item.RaiseCompleted(new SystemNotificationEventArgs()
            {
                ActionTag = obj.action,
                IsActivated = string.IsNullOrWhiteSpace(obj.action),
                IsCancelled = false,
                NotificationId = id
            });
            dic.Remove(id);
        }
    }

    [JSInvokable]
    public async Task HandleNotificationReply(object data)
    {
        var jsonString = JsonSerializer.Serialize(data);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
        var obj = await JsonSerializer.DeserializeAsync<NotificationReplyData>(stream, jsonSerializerOptionsForPropertyModel);
        if (obj != null && obj.data?.id is uint id && dic.TryGetValue(id, out var item))
        {
            item.RaiseCompleted(new SystemNotificationEventArgs()
            {
                ActionTag = obj.Reply,
                IsActivated = string.IsNullOrWhiteSpace(obj.action),
                IsCancelled = false,
                NotificationId = id,
                UserData = null
            });
            dic.Remove(id);
        }
    }

    public async Task<ISystemNotification?> CreateNotification(string action, string title, string message, SystemNotificationAction[] actions)
    {
        var module = await GetModuleAsync();
        if (module == null)
            return null;

        if (false == await module.InvokeAsync<bool>("isSupported"))
            return null;

        if (PermissionType.Granted != permissionType)
        {
            permissionType = await RequestPermission(module);
        }
        if (PermissionType.Granted == permissionType)
        {
            var item = new BlazorNotification(async (id, reply) =>
            {
                IList? replies = null;
                if (!string.IsNullOrWhiteSpace(reply))
                {
                    replies = new[] { new { action = "reply-action", title = "Reply", type = "text" } }.ToList();
                }
                else
                {
                    replies = actions.Select(a => new { Action = a.tag, Icon = a.Icon, Title = a.caption }).ToList();
                }

                var options = new
                {
                    data = new
                    {
                        id = id,
                        action = action,
                    },
                    action = action,
                    id = id,
                    body = message,
                    actions = replies
                };
                await module.InvokeAsync<object>("create", title, options);
            });
            dic.Add(item.Id, item);
            return item;
        }
        return null!;
    }

    private async ValueTask<PermissionType> RequestPermission(IJSObjectReference module)
    {
        string permission = await module.InvokeAsync<string>("requestPermission");

        if (permission.Equals("granted", StringComparison.InvariantCultureIgnoreCase))
            return PermissionType.Granted;

        if (permission.Equals("denied", StringComparison.InvariantCultureIgnoreCase))
            return PermissionType.Denied;

        return PermissionType.Default;
    }

    public async ValueTask DisposeAsync()
    {
        _selfRef?.Dispose();
    }

   
}
