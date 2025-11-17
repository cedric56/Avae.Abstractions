using Android.App;
using Android.Content.PM;
using Avae.Implementations;
using Avalonia;
using Avalonia.Android;
using Example;

namespace Examples.Android;

[Activity(
    Label = "Examples.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (Avalonia.Application.Current is AvaeApplication app)
            app.Dispose();
    }
}
