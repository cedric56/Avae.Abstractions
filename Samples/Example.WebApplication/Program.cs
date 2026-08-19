using Avae.Core;
using Avae.Blazor.Notifications;
using Avae.Blazor.Essentials;
using Example.Razor;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Environment.WebRootPath) });
builder.Services.RegisterBlazorEssentials();
builder.Services.UseBlazorNotifications();
builder.Services.UseSharedLibrary(true);
builder.Services.AddRazorPages();

var app = builder.Build();

ServiceLocator.SetDefault(app.Services);

app.UseBlazorFrameworkFiles();
app.UseRouting();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorPages();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapRazorPages();
//            //.AddAdditionalAssemblies(
//            //    typeof(Avae.Razor.Layout.MainLayout).Assembly,
//            //    typeof(Example.Razor.Components.Home).Assembly
//            //);
//});

app.Run();
