using Avae.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Avae.Blazor.Notifications;

public static class Extensions
{
    public static void UseBlazorNotifications(this IServiceCollection services)
    {
        services.AddScoped<ISystemNotificationService, SystemNotificationService>();
    }

    public static void UseServiceWorker(this IApplicationBuilder builder)
    {
        builder.Use(async (ctx, next) =>
        {
            var path = ctx.Request.Path.Value ?? "";
            if (path.EndsWith("_content/Avae.Blazor.Notifications/service-worker.js", StringComparison.OrdinalIgnoreCase))
            {
                ctx.Response.ContentType = "application/javascript; charset=utf-8";
                ctx.Response.Headers["Service-Worker-Allowed"] = "/";
            }
            await next();
        });
    }
}
