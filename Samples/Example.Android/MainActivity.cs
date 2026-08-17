using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
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
public class MainActivity : AvaloniaMainActivity
{

    // Override OnCreate if you need additional initialization
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        try
        {
            base.OnCreate(savedInstanceState);

            

            // Optional: Additional Android-specific initialization
            // For example, setting up services, notifications, etc.
            Log.Info("Avalonia", "✅ OnCreate completed");
        }
        catch (System.Exception ex)
        {
            Log.Error("Avalonia", $"❌ OnCreate failed: {ex}");
            throw;
        }
    }

    // Override OnDestroy if you need cleanup
    protected override void OnDestroy()
    {
        try
        {
            Log.Info("Avalonia", "🧹 OnDestroy starting");

            if (Avalonia.Application.Current is AndroidApp app)
                app.Dispose();

            base.OnDestroy();
            Log.Info("Avalonia", "✅ OnDestroy completed");
        }
        catch (System.Exception ex)
        {
            Log.Error("Avalonia", $"❌ OnDestroy failed: {ex}");
            throw;
        }
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
           .UseAndroid()
           .WithInterFont();
    }
}

public class AndroidApp : App
{

}
