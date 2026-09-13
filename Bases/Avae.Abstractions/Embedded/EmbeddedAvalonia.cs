using Avalonia.Browser;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Application = Avalonia.Application;

namespace Avae.Abstractions;

[SupportedOSPlatform("browser")]
class EmbeddedAvalonia : Application
{
    public static string AppDiv { get; set; } = "app";

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "View")]
    internal static extern ref AvaloniaView? GetView(
    [UnsafeAccessorType("Avalonia.BrowserSingleViewLifetime, Avalonia.Browser")] IApplicationLifetime lifetime);

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();

        GetView(ApplicationLifetime!) = new AvaloniaView(AppDiv)
        {
            Content = new Control(),
        };
    }
}
