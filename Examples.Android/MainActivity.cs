using Android.App;
using Android.Content.PM;
using Avae.Services;
using Avalonia;
using Avalonia.Android;
using Example;
using ReactiveUI.Avalonia;

namespace Examples.Android;

[Activity(
    Label = "Examples.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<AndroidApp>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .UseServices()
            .UseReactiveUI();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (Avalonia.Application.Current is AndroidApp app)
            app.Dispose();
    }
}

public class AndroidApp : App
{
    protected override string Logs => string.Empty;
}
