using Avalonia;
using Avalonia.Browser;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.Versioning;

namespace Avae.Browser;

public static class Extensions
{
    [SupportedOSPlatform("browser")]
    public static async Task UseEmbeddedAvaloniaApp(this IServiceCollection services, string appDiv = "app", Func<AppBuilder, AppBuilder>? action = null)
    {
        EmbeddedAvalonia.AppDiv = appDiv;
        var builder = AppBuilder.Configure<EmbeddedAvalonia>();
        if (action != null) builder = action(builder);
        await builder.SetupBrowserAppAsync();
    }
}
