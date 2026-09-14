using Avae.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Avae.Notifications;

public static class Extensions
{
    public static void UseNotifications(this IServiceCollection services)
    {
        services.AddSingleton<ISystemNotificationService, SystemNotificationService>();
    }
}
