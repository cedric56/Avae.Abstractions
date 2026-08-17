using Microsoft.Maui.ApplicationModel;

namespace Avae.Essentials.Blazor;

internal class BlazorBrowser(BlazorLauncher launcher) : IBrowser
{
    public Task<bool> OpenAsync(Uri uri, BrowserLaunchOptions options)
    {
        return launcher.OpenAsync(uri);
    }
}
