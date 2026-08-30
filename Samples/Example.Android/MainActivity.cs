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

    protected override void OnDestroy()
    {
        if (Avalonia.Application.Current is AndroidApp app)
            app.Dispose();

        base.OnDestroy();
    }
}
[Application]
public class MainApplication : AvaloniaAndroidApplication<AndroidApp>
{
    protected MainApplication(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        Microsoft.Maui.ApplicationModel.Platform.Init(this);
        return base.CustomizeAppBuilder(builder)
           .WithAppNotifications(ApplicationContext!)
           .UseAndroid();
    }
}

public class AndroidApp : App
{
    Func<Task>? unsuscribe = null;

    protected override async Task AfterCompletedAsync()
    {
        var monitor = Container.Provider.GetRequiredService<IDBMonitor<Example.Models.Person>>();
        //unsuscribe = await Container.Provider.AddSignalR(monitor, factory: _ => new Xamarin.Android.Net.AndroidMessageHandler
        //{
        //    ServerCertificateCustomValidationCallback = Avae.DAL.gRPC.Client.Extensions.ValidateCertificates2,
        //    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
        //});
        unsuscribe = await Container.Provider.AddStreamingHub(monitor, new SocketsHttpHandler()
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