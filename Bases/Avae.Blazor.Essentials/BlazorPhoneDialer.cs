using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;

namespace Avae.Blazor.Essentials;

internal class BlazorPhoneDialer(ILauncher launcher) : IPhoneDialer
{
    public bool IsSupported => true;

    public async void Open(string number)
    {
       await launcher.OpenAsync(new Uri(number));
    }
}
