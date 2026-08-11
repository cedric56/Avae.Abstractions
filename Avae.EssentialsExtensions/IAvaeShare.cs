using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;

namespace Avae.Shared
{
    public interface IAvaeShare : IShare
    {
        Task RequestAsync(string title, IEnumerable<FileBase> files);
    }
}
