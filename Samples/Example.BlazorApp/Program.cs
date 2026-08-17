using Avae.Abstractions;
using Avae.Essentials.Blazor;
using Avae.Essentials.Blazor.Components;
using Example.BlazorApp.Components;
using Example.Razor;
using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Storage;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Environment.WebRootPath) });
builder.Services.RegisterBlazorEssentials();
builder.Services.UseSharedLibrary(true);
builder.Services.AddSingleton<IVideoCaptureHandles, VideoCapture>();
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();
    //.AddInteractiveWebAssemblyComponents();
var app = builder.Build();

ServiceLocator.SetDefault(app.Services);
//app.UseBlazorFrameworkFiles();
app.UseRouting();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    //.AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(Avae.Razor.Layout.MainLayout).Assembly,
        typeof(Example.Razor.Components.Home).Assembly
    );
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