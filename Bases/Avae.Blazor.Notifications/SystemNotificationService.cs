using Avae.Services;
using Microsoft.JSInterop;
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

    public event EventHandler<SystemNotificationEventArgs>? NotificationCompleted;

    public IReadOnlyDictionary<uint, ISystemNotification> ActiveNotifications => throw new NotImplementedException();

    private async ValueTask<IJSObjectReference> GetModuleAsync()
    {
        if (_module == null)
        {
            _module = await jSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Avae.Blazor.Notifications/notifications.js");
            await _module.InvokeVoidAsync("registerServiceWorker");
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
            NotificationCompleted?.Invoke(this, new SystemNotificationEventArgs()
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
        using var stream = GetMemoryStream(data);
        var obj = await JsonSerializer.DeserializeAsync<NotificationData>(stream, jsonSerializerOptionsForPropertyModel);
        if (obj != null && obj.data?.id is uint id && dic.TryGetValue(id, out var item))
        {
            NotificationCompleted?.Invoke(this, new SystemNotificationEventArgs()
            {
                ActionTag = obj.action,
                IsActivated = string.IsNullOrWhiteSpace(obj.action),
                IsCancelled = false,
                NotificationId = id
            });
            dic.Remove(id);
        }
    }

    private MemoryStream GetMemoryStream(object obj)
    {
        var jsonString = JsonSerializer.Serialize(obj);
        return new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
    }

    [JSInvokable]
    public async Task HandleNotificationReply(object data, object user_data)
    {
        using var data_stream = GetMemoryStream(data);
        var obj = await JsonSerializer.DeserializeAsync<NotificationReplyData>(data_stream, jsonSerializerOptionsForPropertyModel);
        if (obj != null && obj.data?.id is uint id && dic.TryGetValue(id, out var item))
        {
            using var user_data_stream = GetMemoryStream(user_data);
            NotificationCompleted?.Invoke(this, new SystemNotificationEventArgs()
            {
                ActionTag = obj.action,
                IsActivated = false,
                IsCancelled = false,
                NotificationId = id,
                UserData = await JsonSerializer.DeserializeAsync<string>(user_data_stream, jsonSerializerOptionsForPropertyModel)
            });
            dic.Remove(id);
        }
    }

    public async Task<ISystemNotification?> CreateNotification(string? category)
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
            var item = new BlazorNotification(category, async (notification) =>
            {
                var options = new
                {
                    data = new
                    {
                        id = notification.Id,
                        replyActionTag = notification.ReplyActionTag,
                    },
                    body = notification.Message,
                    actions = notification.Actions?.Select(a => new { action = a.tag, icon = a.Icon, title = a.caption, type = a.tag == notification.ReplyActionTag ? "text" : "button" }),
                };
                await module.InvokeAsync<object>("create", notification.Title, options);
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

    public void CloseAll()
    {
        throw new NotImplementedException();
    }
}
