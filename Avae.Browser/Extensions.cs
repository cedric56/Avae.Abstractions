using Avalonia;
using Avalonia.Browser;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.Versioning;

namespace Avae.Browser;

public static class Extensions
{
    //[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    //[return: UnsafeAccessorType("Avalonia.BrowserSingleViewLifetime, Avalonia.Browser")]
    //internal extern static object CreateAppActions();

    //[UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "s_globalThis")]
    //public static extern ref JSObject GlobalThis(
    //    [UnsafeAccessorType("Avalonia.Browser.BrowserWindowingPlatform, Avalonia.Browser")] object? dummy);

    //[UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "GetGlobalThis")]
    //public static extern JSObject GetGlobalThis(
    // [UnsafeAccessorType("Avalonia.Browser.Interop.DomHelper, Avalonia.Browser")] object? dummy);


    //[SupportedOSPlatform("browser")]
    //public static async Task UseEmbeddedAvaloniaApp(string appDiv = "app")
    //{
    //    EmbeddedAvalonia.AppDiv = appDiv;
    //    var builder = AppBuilder.Configure<EmbeddedAvalonia>();
    //    builder.UseBrowser();
    //    GlobalThis(null) = GetGlobalThis(null);
    //    builder.SetupWithLifetime((IApplicationLifetime)CreateAppActions());
    //}

    [SupportedOSPlatform("browser")]
    public static async Task UseEmbeddedAvaloniaApp(this IServiceCollection services, string appDiv = "app")
    {
        EmbeddedAvalonia.AppDiv = appDiv;
        var builder = AppBuilder.Configure<EmbeddedAvalonia>();
        await builder.SetupBrowserAppAsync();
    }
}
