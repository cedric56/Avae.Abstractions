using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Storage;

namespace Example.Razor
{
    public interface IVideoCaptureHandles
    {
        RenderFragment VideoFragment { get; }
        event Func<TaskCompletionSource<FileResult?>, Task>? RequestCapture;
        void HandleCompleted(FileResult? result);
    }
}
