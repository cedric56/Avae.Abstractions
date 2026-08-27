using Avae.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Avae.Avalonia.Notifications;

public static class Extensions
{
    public static void UseAvaeNotifications(this IServiceCollection services)
    {
        services.AddSingleton<ISystemNotificationService, SystemNotificationService>();
    }
}
