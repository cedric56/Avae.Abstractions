using Avae.Blazor.Essentials;
using Avae.Blazor.Essentials.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Storage;
using MudBlazor;

namespace Example.BlazorApp;

class VideoCapture : ComponentBase, IDisposable
{
    private readonly VideoCaptureCoordinator coordinator;
    private readonly IDialogService dialogService;
    private TaskCompletionSource<FileResult?>? tcs;
    private IDialogReference? reference;

    public VideoCapture(VideoCaptureCoordinator coordinator, IDialogService dialogService)
    {
        this.coordinator = coordinator;
        this.dialogService = dialogService;
        this.coordinator.RequestCapture += Coordinator_RequestCapture;
    }

    private async Task Coordinator_RequestCapture(TaskCompletionSource<FileResult?> arg)
    {
        tcs = arg;

        var parameters = new DialogParameters
        {
            { "OnCompleted", EventCallback.Factory.Create<FileResult?>(this, HandleCompleted) }
        };

        reference = await dialogService.ShowAsync<VideoCaptureDialog>("Camera", parameters);
        await tcs.Task;
    }

    public void HandleCompleted(FileResult? result)
    {
        tcs?.SetResult(result);
        reference?.Close();
    }

    public void Dispose()
    {
        this.coordinator.RequestCapture -= Coordinator_RequestCapture;
    }
}
