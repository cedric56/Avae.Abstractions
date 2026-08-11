using Avalonia.Platform.Storage;
using Microsoft.Maui.ApplicationModel.Communication;

namespace Avalonia.Controls.Maui.Essentials;

class AvaeEmailAttachment : EmailAttachment
{
    private IStorageFile storageFile;
    public AvaeEmailAttachment(AvaloniaFileResult file)
        : base(file.FullPath, file.ContentType)
    {
        FileName = file.FileName;
        storageFile = file.StorageFile;
    }

    public new Task<Stream> OpenReadAsync()
    {
        return storageFile.OpenReadAsync();
    }
}
