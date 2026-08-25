using Avae.Core;
using Avae.Essentials;
using KristofferStrube.Blazor.FileSystemAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.Maui.Storage;

namespace Avae.Blazor.Essentials;

class BlazorFileResult : FileResult
{
    private readonly byte[]? _bytes;

    readonly KristofferStrube.Blazor.FileAPI.File? file;

    public BlazorFileResult(KristofferStrube.Blazor.FileAPI.File file, string name, string contentType)
        : base(name, contentType)
    {
        this.file = file;
        ContentType = contentType;
    }

    public BlazorFileResult(string fileName, string contentType, byte[] bytes)
        : base(fileName, contentType)
    {
        FileName = fileName;
        ContentType = contentType;
        _bytes = bytes;
    }

    //public Task<byte[]> GetBytes() => Task.FromResult(_bytes);

    public IJSObjectReference? JSReference => file?.JSReference;

    //public Task<Stream> OpenReadableAsync() => file.StreamAsync();

    

   
}

class BlazorFilePicker : IFilePicker
{
    public async Task<FileResult?> PickAsync(PickOptions? options = null)
    {
        try
        {
            OpenFilePickerOptions opt = new()
            {
                Multiple = false,
            };

            var fileSystemAccessService = ServiceLocator.GetScopedRequiredService<IFileSystemAccessService>();
            var fileHandles = await fileSystemAccessService.ShowOpenFilePickerAsync(opt);
            var task = fileHandles.Select(f => f.GetFileAsync()).Single();
            var file = await task;
            var name = await file.GetNameAsync();
            return new BlazorFileResult(file, name, EssentialsAccessors.ResolveContentType(name));
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
            var fileSystemAccessService = ServiceLocator.GetScopedRequiredService<IFileSystemAccessService>();
            var fileHandles = await fileSystemAccessService.ShowOpenFilePickerAsync(opt);
            var tasks = fileHandles.Select(f => f.GetFileAsync());
            var files = new List<KristofferStrube.Blazor.FileAPI.File>(await Task.WhenAll(tasks));
            return await Task.WhenAll(files.Select(async file =>
            {
                var name = await file.GetNameAsync();
                return new BlazorFileResult(file, name, EssentialsAccessors.ResolveContentType(name));

            }));
        }
        catch
        {
            return [];
        }
    }
}
