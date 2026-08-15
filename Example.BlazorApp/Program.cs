using Avae.Abstractions;
using Avae.BlazorEssentials;
using Example.BlazorApp.Components;
using Example.Razor;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Environment.WebRootPath) });
builder.Services.RegisterBlazorEssentials();
builder.Services.UseSharedLibrary(true);
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

ServiceLocator.SetDefault(app.Services);

app.UseBlazorFrameworkFiles();
app.UseRouting();
app.UseAntiforgery();
app.MapStaticAssets();
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(
                typeof(Avae.Razor.Layout.MainLayout).Assembly,
                typeof(Example.Razor.Components.Home).Assembly
            );
});

app.Run();