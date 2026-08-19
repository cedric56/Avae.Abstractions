using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace BlazorWebAssemblyExtension;

public class BlazorWebAssemblyExtensionStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        // Return a registration action that adds middleware to the pipeline
        return app =>
        {
            app.Use(async (ctx, next) =>
            {
                var path = ctx.Request.Path.Value ?? "";
                if (path.EndsWith("_content/Avae.Blazor.Notifications/service-worker.js", StringComparison.OrdinalIgnoreCase))
                {
                    ctx.Response.ContentType = "application/javascript; charset=utf-8";
                    ctx.Response.Headers["Service-Worker-Allowed"] = "/";
                }
                await next();
            });

            // Call the next startup filter in the chain
            next(app);
        };
    }
}