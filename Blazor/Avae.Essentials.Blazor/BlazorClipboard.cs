using Microsoft.Maui.ApplicationModel.DataTransfer;
using PatrickJahr.Blazor.AsyncClipboard;

namespace Avae.Essentials.Blazor
{
    internal class BlazorClipboard(AsyncClipboardService service) : IClipboard
    {
        public bool HasText => throw new NotImplementedException();

        public event EventHandler<EventArgs>? ClipboardContentChanged;

        public async Task<string?> GetTextAsync()
        {
            return await service.ReadTextAsync();
        }

        public async Task SetTextAsync(string? text)
        {
            await service.WriteTextAsync(text ?? string.Empty);
        }
    }
}
