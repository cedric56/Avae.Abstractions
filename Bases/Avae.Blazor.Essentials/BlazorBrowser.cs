using Avae.Core;
using Microsoft.Maui.ApplicationModel;

namespace Avae.Blazor.Essentials;

internal class BlazorBrowser : IBrowser
{
    public Task<bool> OpenAsync(Uri uri, BrowserLaunchOptions options)
    {
        var launcher = ServiceLocator.GetScopedRequiredService<ILauncher>();
        return launcher.OpenAsync(uri);
    }
}
