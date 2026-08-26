using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Avae.Avalonia.Notifications;
using Avalonia;
using Avalonia.Android;
using Avalonia.Controls;
using Avalonia.Labs.Notifications;

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
        if (Avalonia.Application.Current is App app)
            app.Dispose();

        base.OnDestroy();
    }
}
[Application]
public class MainApplication : AvaloniaAndroidApplication<App>
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
