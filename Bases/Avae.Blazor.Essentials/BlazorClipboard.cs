using Avae.Core;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using PatrickJahr.Blazor.AsyncClipboard;

namespace Avae.Blazor.Essentials;

internal class BlazorClipboard : IClipboard
{
    public bool HasText => throw new NotImplementedException();

    public event EventHandler<EventArgs>? ClipboardContentChanged;

    public async Task<string?> GetTextAsync()
    {
        var service = ServiceLocator.GetScopedRequiredService<AsyncClipboardService>();
        return await service.ReadTextAsync();
    }

    public async Task SetTextAsync(string? text)
    {
        var service = ServiceLocator.GetScopedRequiredService<AsyncClipboardService>();
        ClipboardContentChanged?.Invoke(this, EventArgs.Empty);
        await service.WriteTextAsync(text ?? string.Empty);
    }
}
