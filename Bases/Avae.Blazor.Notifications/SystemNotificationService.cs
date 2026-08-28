using Avae.Services;
using Avalonia.Labs.Notifications;
using Microsoft.JSInterop;
using System.Text;
using System.Text.Json;

namespace Avae.Blazor.Notifications;

internal class SystemNotificationService : ISystemNotificationService, IAsyncDisposable
{
    Dictionary<uint, ISystemNotification> currents = new();
    
    Dictionary<string, NotificationChannel>? channels = null;

    IJSRuntime jSRuntime;

    public SystemNotificationService(IJSRuntime jSRuntime, IEnumerable<NotificationChannel>? channels = null)
    {
        this.jSRuntime = jSRuntime;
        this.channels = channels?.ToDictionary(x => x.Id, x => x);
        _dotNetRef = DotNetObjectReference.Create(this);
    }

    private IJSObjectReference? _module;
    private IJSObjectReference? _innerModule;
    public event EventHandler<SystemNotificationEventArgs>? NotificationCompleted;

    public IReadOnlyDictionary<uint, ISystemNotification> ActiveNotifications() => currents;

    private async ValueTask<IJSObjectReference> GetModuleAsync()
    {
        if (_module == null)
        {
            _module = await jSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Avalonia.Labs.Notifications/notifications.js");
            _innerModule = await jSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Avae.Blazor.Notifications/notifications.js");
            await _module.InvokeVoidAsync("registerServiceWorker");
            await _innerModule.InvokeVoidAsync("registrations", _dotNetRef);
        }
        return _module;
    }
    private DotNetObjectReference<SystemNotificationService>? _dotNetRef;

    public async ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();
        if (_module is not null)
            await _module.DisposeAsync();
        if (_innerModule is not null)
            await _innerModule.DisposeAsync();        
    }

    [JSInvokable]
    public async void OnClose(string data)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        var obj = await JsonSerializer.DeserializeAsync(stream, NotificationJsonContext.Default.Data);
        if (obj != null && obj.data?.id is uint id && currents.TryGetValue(id, out var item))
        {
            NotificationCompleted?.Invoke(this, new SystemNotificationEventArgs()
            {
                IsCancelled = true,
                NotificationId = id
            });
        }
    }

    [JSInvokable]
    public async void OnClick(string data)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        var obj = await JsonSerializer.DeserializeAsync(stream, NotificationJsonContext.Default.Data);
        if (obj != null && obj.data?.id is uint id && currents.TryGetValue(id, out var item))
        {
            NotificationCompleted?.Invoke(this, new SystemNotificationEventArgs()
            {
                ActionTag = obj.action,
                IsActivated = string.IsNullOrWhiteSpace(obj.action),
                IsCancelled = false,
                NotificationId = id
            });
        }
    }

    [JSInvokable]
    public async void OnReply(string data, string reply)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        var obj = await JsonSerializer.DeserializeAsync(stream, NotificationJsonContext.Default.ReplyData);
        if (obj != null && obj.data?.id is uint id && currents.TryGetValue(id, out var item))
        {
            NotificationCompleted?.Invoke(this, new SystemNotificationEventArgs()
            {
                ActionTag = obj.action,
                IsActivated = string.IsNullOrWhiteSpace(obj.action),
                IsCancelled = false,
                NotificationId = id,
                UserData = reply
            });
        }
    }
    public const string DefaultChannel = "default";
    public const string DefaultChannelLabel = "Notifications";
    public async Task<ISystemNotification?> CreateNotification(string? category)
    {
        var module = await GetModuleAsync();
        if (module == null)
            return null;

        if (false == await module.InvokeAsync<bool>("isSupported"))
            return null;

        channels ??= [];
        if (!channels.TryGetValue(category ?? DefaultChannel, out var channel))
        {
            channels.Add(DefaultChannel, channel = new NotificationChannel(DefaultChannel, DefaultChannelLabel));
        }
        if (channel == null)
            return null;

        var item = new BlazorNotification(channel, this);
        currents.Add(item.Id, item);
        return item;
    }

    public async void CloseAll()
    {
        var module = await GetModuleAsync();
        if (module != null)
            await module.InvokeAsync<object>("closeAllNotifications");
    }

    public async Task Show(BlazorNotification notification, NotificationOptions options)
    {
        var module = await GetModuleAsync();
        if (module != null)
            await module.InvokeVoidAsync("create", notification.Title, JsonSerializer.Serialize(options, NotificationJsonContext.Default.NotificationOptions));
    }

    public async Task Close(uint id)
    {
        var module = await GetModuleAsync();
        if (module != null)
            await module.InvokeVoidAsync("close", id.ToString());
        currents.Remove(id);
    }

    public async Task InitializeAsync()
    {
        _module = await GetModuleAsync();
    }
}
