using Microsoft.Maui.ApplicationModel.Communication;

namespace Avae.Essentials.Blazor;

internal class BlazorPhoneDialer(BlazorLauncher launcher) : IPhoneDialer
{
    public bool IsSupported => true;

    public async void Open(string number)
    {
       await launcher.OpenAsync(new Uri(number));
    }
}
