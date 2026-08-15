using KristofferStrube.Blazor.FileSystemAccess;
using Microsoft.Maui.Storage;

namespace Avae.BlazorEssentials;

class BlazorFileResult(KristofferStrube.Blazor.FileAPI.File file, string name, string contentType = "todo") : FileResult(name, contentType)
{
    public async new Task<Stream> OpenReadAsync() => await file.StreamAsync();
}

class BlazorFilePicker(IFileSystemAccessService fileSystemAccessService) : IFilePicker
{
    public async Task<FileResult?> PickAsync(PickOptions? options = null)
    {
        try
        {
            OpenFilePickerOptions opt = new()
            {
                Multiple = false,
            };
            var fileHandles = await fileSystemAccessService.ShowOpenFilePickerAsync(opt);
            var task = fileHandles.Select(f => f.GetFileAsync()).Single();
            var file = await task;
            return new BlazorFileResult(file, await file.GetNameAsync());
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<FileResult>?> PickMultipleAsync(PickOptions? options = null)
    {
        try
        {
            OpenFilePickerOptions opt = new()
            {
                Multiple = true,
            };
            var fileHandles = await fileSystemAccessService.ShowOpenFilePickerAsync(opt);
            var tasks = fileHandles.Select(f => f.GetFileAsync());
            var files = new List<KristofferStrube.Blazor.FileAPI.File>(await Task.WhenAll(tasks));
            return await Task.WhenAll(files.Select(async f =>
            {
                return new BlazorFileResult(f, await f.GetNameAsync());

            }));
        }
        catch
        {
            return [];
        }
    }
}
