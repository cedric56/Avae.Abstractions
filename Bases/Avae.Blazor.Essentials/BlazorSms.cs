using Microsoft.Maui.ApplicationModel.Communication;

namespace Avae.Blazor.Essentials;

internal class BlazorSms(BlazorLauncher launcher) : ISms
{
    public bool IsComposeSupported => true;

    public Task ComposeAsync(SmsMessage? message)
    {
        var recipients = string.Join(",", message?.Recipients.Select(Uri.EscapeDataString) ?? []);
        var uri = $"sms:{recipients}";
        if (!string.IsNullOrEmpty(message?.Body))
            uri += "?&body=" + Uri.EscapeDataString(message.Body);

        return launcher.OpenAsync(new Uri(uri));
    }
}
