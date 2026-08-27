using Avae.Services;
using Avalonia.Labs.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Avae.Blazor.Notifications;

public static class Extensions
{
    public static void UseBlazorNotifications(this IServiceCollection services, IEnumerable<NotificationChannel>? channels = null)
    {
        services.AddScoped<ISystemNotificationService>(provider => new SystemNotificationService(provider.GetRequiredService<IJSRuntime>(), channels));
    }
}
