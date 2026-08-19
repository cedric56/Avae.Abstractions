using Append.Blazor.Notifications;
using Avae.Services;
using Microsoft.JSInterop;

namespace Avae.Blazor.Notifications;

internal class SystemNotificationService(IJSRuntime jSRuntime, Append.Blazor.Notifications.INotificationService service) : ISystemNotificationService
{
    private IJSObjectReference? _module;
    private PermissionType? permissionType;

    private async ValueTask<IJSObjectReference> GetModuleAsync()
    {
        return _module ??= await jSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Avae.Blazor.Notifications/notifications.js");
    }

    public class NotificationAction
    {
        public string? Action { get; set; }
        public string? Title { get; set; }
        public string? Icon { get; set; }
    }

    public class NotificationActionsOptions : Append.Blazor.Notifications.NotificationOptions
    {
        public List<Action> Actions { get; set; } = [];
    }

    public async Task<ISystemNotification?> CreateNotification(string action, string title, string message, SystemNotificationAction[] actions)
    {
        if (false == await service.IsSupportedByBrowserAsync())
            return null;

        if (PermissionType.Granted == (permissionType ??= await service.RequestPermissionAsync()))
        {
            if (actions.Length > 0)
            {
                var module = await GetModuleAsync();
                var options = new
                {
                    // Put other options here.
                    actions = new List<NotificationAction>(actions.Select(a => new NotificationAction() { Action = a.tag, Icon = a.Icon, Title = a.caption }))
                };
                await module.InvokeVoidAsync("create", title, options);
            }
            else
            {
                await service.CreateAsync(title, new Append.Blazor.Notifications.NotificationOptions()
                {
                    Body = message
                });
            }
        }
        return null!;
    }
}
