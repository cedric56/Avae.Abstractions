using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Avae.Avalonia.Notifications;
using Avae.DAL;
using Avalonia;
using Avalonia.Android;
using Avalonia.Labs.Notifications;
using Example.DAL;
using Example.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Example.Android;

[Activity(
    Label = "Examples.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        SystemNotificationService.Activity = this;

        base.OnCreate(savedInstanceState);
    }
}

[Application]
public class MainApplication : AvaloniaAndroidApplication<AndroidApp>
{
    protected MainApplication(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    protected override AppBuilder CreateAppBuilder()
    {
        Microsoft.Maui.ApplicationModel.Platform.Init(this);
        return App.CreateApp<AndroidApp>(
             configure: services => services.UseDBOnionLayer())
            .WithAppNotifications(ApplicationContext!)
            .UseAndroid();
    }
}

public class AndroidApp : App
{
    public AndroidApp(IServiceProvider provider)
        : base(provider)
    {

    }

    Func<Task>? unsuscribe = null;

    protected override async Task AfterCompletedAsync()
    {
        var monitor = provider.GetRequiredService<IDBMonitor<Example.Models.Person>>();
        //unsuscribe = await Container.Provider.AddSignalR(monitor, factory: _ => new Xamarin.Android.Net.AndroidMessageHandler
        //{
        //    ServerCertificateCustomValidationCallback = Avae.DAL.gRPC.Client.Extensions.ValidateCertificates2,
        //    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
        //});
        Repository.Initialize(monitor);
        
        unsuscribe = await monitor.AddStreamingHub(new SocketsHttpHandler()
        {
            SslOptions =
            {
                RemoteCertificateValidationCallback = Avae.DAL.gRPC.Client.Extensions.ValidateCertificates
            }
        });
    }

    public override void Dispose()
    {
        unsuscribe?.Invoke();

        base.Dispose();
    }
}