using Avae.Blazor.Essentials;
using Avae.Blazor.Essentials.Components;
using Avae.Blazor.Notifications;
using Avae.Core;
using Example.BlazorApp.Components;
using Example.Razor;
using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Storage;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri(builder.Environment.WebRootPath)
});
builder.Services.RegisterBlazorEssentials();
builder.Services.UseBlazorNotifications();
builder.Services.UseSharedLibrary(true);
 builder.Services.AddSingleton<IVideoCaptureHandles, VideoCapture>();
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });

var app = builder.Build();

ServiceLocator.SetDefault(app.Services);
app.UseRouting();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(Avae.Razor.Layout.MainLayout).Assembly,
        typeof(Example.Razor.Components.Home).Assembly
    );

app.UseServiceWorker();
app.Run();

class VideoCapture : VideoCaptureDialog, IVideoCaptureHandles
{
    VideoCaptureCoordinator coordinator;
    VideoCaptureDialog dialog = new VideoCaptureDialog();
    public VideoCapture(VideoCaptureCoordinator coordinator)
    {
        this.coordinator = coordinator;
        //this.dialog.OnCompleted += HandleCompleted;
    }

    public RenderFragment VideoFragment => (builder) => { builder.AddContent(0, null, value: dialog); };

    event Func<TaskCompletionSource<FileResult?>, Task>? IVideoCaptureHandles.RequestCapture
    {
        add { coordinator.RequestCapture += value; }
        remove { coordinator.RequestCapture -= value; }
    }

    public void HandleCompleted(FileResult? result)
    {
        throw new NotImplementedException();
    }
}