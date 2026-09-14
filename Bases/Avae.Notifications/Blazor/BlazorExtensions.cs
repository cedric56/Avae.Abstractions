using Avae.Services;
using NotificationChannel = Avalonia.Labs.Notifications.NotificationChannel;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;

namespace Avae.Notifications
{
    public static class BlazorExtensions
    {
        public static void UseBlazorNotifications(this IServiceCollection services, IEnumerable<NotificationChannel>? channels = null)
        {
            services.AddScoped<ISystemNotificationService>(provider => new BlazorNotificationService(provider.GetRequiredService<IJSRuntime>(), channels));
        }
    }
}
