using Avae.Abstractions;
using Example.BlazorApp.Components;
using Example.Razor;
using MagicOnion;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Accessibility;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Authentication;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddRazorComponents()
//    //.AddInteractiveServerComponents()
//    .AddInteractiveWebAssemblyComponents();

//builder.Services.AddScoped(sp => new HttpClient
//{
//    BaseAddress = new Uri(builder.Environment.WebRootPath)
//});

//builder.Services.UseSharedLibrary();
//builder.Services.RegisterEssentials();

//var app = builder.Build();

//ServiceLocator.SetDefault(app.Services);

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error", createScopeForErrors: true);
//    app.UseHsts();
//}

//app.UseHttpsRedirection();

//// ✅ 2. Blazor framework files (for WASM)
//app.UseBlazorFrameworkFiles();

//// ✅ 1. Static files FIRST
//app.UseStaticFiles();

//// ✅ 3. Routing
//app.UseRouting();

//// ✅ 4. Anti-forgery MUST be after UseRouting and before endpoints
//app.UseAntiforgery();

//// ✅ 5. Status code pages
//app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

//// ✅ 6. Map endpoints
//app.MapRazorComponents<App>()
//    //.AddInteractiveServerRenderMode()
//    .AddInteractiveWebAssemblyRenderMode()
//    .AddAdditionalAssemblies(
//        typeof(Avae.Razor.Layout.MainLayout).Assembly,
//        typeof(Example.Razor.Components.Home).Assembly
//    );

//// ✅ 7. Map static assets (optional, if using .NET 9+)
//app.MapStaticAssets();

//app.Run();

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
    //.AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Environment.WebRootPath)
    //BaseAddress = new Uri(builder.Configuration["BaseAddress"] ?? "https://localhost:7129/")
    // or simply: new Uri(builder.HostEnvironment.BaseAddress) if you are in a pure client project
});

builder.Services.UseSharedLibrary();
builder.Services.TryAddSingleton<IAccelerometer>(Accelerometer.Default);
builder.Services.TryAddSingleton<IAppActions>(AppActions.Current);
builder.Services.TryAddSingleton<IAppInfo>(AppInfo.Current);
builder.Services.TryAddSingleton<IBarometer>(Barometer.Default);
builder.Services.TryAddSingleton<IBattery>(Battery.Default);
builder.Services.TryAddSingleton<IBrowser>(Browser.Default);
builder.Services.TryAddSingleton<IClipboard>(Clipboard.Default);
builder.Services.TryAddSingleton<ICompass>(Compass.Default);
builder.Services.TryAddSingleton<IConnectivity>(Connectivity.Current);
builder.Services.TryAddSingleton<IContacts>(Contacts.Default);
builder.Services.TryAddSingleton<IDeviceDisplay>(DeviceDisplay.Current);
builder.Services.TryAddSingleton<IDeviceInfo>(DeviceInfo.Current);
builder.Services.TryAddSingleton<IEmail>(Email.Default);
builder.Services.TryAddSingleton<IFilePicker>(FilePicker.Default);
builder.Services.TryAddSingleton<IFlashlight>(Flashlight.Default);
builder.Services.TryAddSingleton<IGeocoding>(Geocoding.Default);
builder.Services.TryAddSingleton<IGeolocation>(Geolocation.Default);
builder.Services.TryAddSingleton<IGyroscope>(Gyroscope.Default);
builder.Services.TryAddSingleton<IHapticFeedback>(HapticFeedback.Default);
builder.Services.TryAddSingleton<ILauncher>(Launcher.Default);
builder.Services.TryAddSingleton<IMagnetometer>(Magnetometer.Default);
builder.Services.TryAddSingleton<IMap>(Map.Default);
builder.Services.TryAddSingleton<IMediaPicker>(MediaPicker.Default);
builder.Services.TryAddSingleton<IOrientationSensor>(OrientationSensor.Default);
builder.Services.TryAddSingleton<IPhoneDialer>(PhoneDialer.Default);
builder.Services.TryAddSingleton<ISecureStorage>(SecureStorage.Default);
builder.Services.TryAddSingleton<ISemanticScreenReader>(SemanticScreenReader.Default);
builder.Services.TryAddSingleton<IShare>(Share.Default);
builder.Services.TryAddSingleton<ISms>(Sms.Default);
builder.Services.TryAddSingleton<ITextToSpeech>(TextToSpeech.Default);
builder.Services.TryAddSingleton<IVibration>(Vibration.Default);
builder.Services.TryAddSingleton<IWebAuthenticator>(WebAuthenticator.Default);

//builder.Services.RegisterEssentials();

var app = builder.Build();

ServiceLocator.SetDefault(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
//app.UseBlazorFrameworkFiles();
// Correct order for modern Blazor Web App
app.UseStaticFiles();                 // or better: app.MapStaticAssets(); in .NET 9/10
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    //.AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(Avae.Razor.Layout.MainLayout).Assembly,
        typeof(Example.Razor.Components.Home).Assembly
    );

// Prefer this in .NET 9 / 10
app.MapStaticAssets();

app.Run();