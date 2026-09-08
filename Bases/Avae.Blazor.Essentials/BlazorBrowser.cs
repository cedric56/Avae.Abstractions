using Avae.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;

namespace Avae.Blazor.Essentials;

internal class BlazorBrowser(CircuitServiceAccessor circuitServiceAccessor) : IBrowser
{
    public Task<bool> OpenAsync(Uri uri, BrowserLaunchOptions options)
    {
        var launcher = circuitServiceAccessor.GetRequiredService<ILauncher>();
        return launcher.OpenAsync(uri);
    }
}
