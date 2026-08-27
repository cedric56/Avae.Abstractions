using Avae.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Avae.Blazor.Notifications;

public static class Extensions
{
    public static void UseBlazorNotifications(this IServiceCollection services)
    {
        services.AddScoped<ISystemNotificationService, SystemNotificationService>();
    }
}
