using Avae.Core;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;

namespace Avae.Blazor.Essentials;

internal class BlazorPhoneDialer : IPhoneDialer
{
    public bool IsSupported => true;

    public async void Open(string number)
    {
        var launcher = ServiceLocator.GetScopedRequiredService<ILauncher>();
        await launcher.OpenAsync(new Uri($"tel:{number}"));
    }
}
