using Avalonia;
using Avalonia.Labs.Notifications;

namespace Example.Windows;

class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .WithDataAnnotationsValidation()
            .WithAppNotifications(new AppNotificationOptions()
            {
                AppIcon = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets/avalonia-logo.ico"),
                AppName = "Example",
            })
            .UseHarfBuzz()
            .UseWin32()
            .UseSkia()
            .LogToTrace();
    }
}
