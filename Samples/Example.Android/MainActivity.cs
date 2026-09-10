using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Avae.Notifications;
using Avalonia;
using Avalonia.Android;
using Avalonia.Labs.Notifications;
using Example.DAL;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

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
public class MainApplication : AvaloniaAndroidApplication<Avalonia.Application>
{
    protected MainApplication(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    protected override AppBuilder CreateAppBuilder()
    {
        Microsoft.Maui.ApplicationModel.Platform.Init(this);
        return App.CreateApp(
             services =>
             {
                 services.UseDBOnionLayer();
                 services.AddSingleton<HttpMessageHandler>(_ =>
                 {
                     return new SocketsHttpHandler()
                     {
                         SslOptions =
                        {
                            RemoteCertificateValidationCallback = Avae.DAL.gRPC.Client.Extensions.ValidateCertificates
                        }
                     };

                //      new Xamarin.Android.Net.AndroidMessageHandler
                //    {
                //        ServerCertificateCustomValidationCallback = Avae.DAL.gRPC.Client.Extensions.ValidateCertificates2,
                //        AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
                //    }
                 });
             })
            .WithAppNotifications(ApplicationContext!)
            .UseAndroid();
    }
}