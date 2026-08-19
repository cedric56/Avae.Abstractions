using Avalonia;
using Avalonia.Labs.Notifications;

namespace Example.Windows;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .WithAppNotifications(new AppNotificationOptions()
            {
                AppIcon = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets/avalonia-logo.ico"),
                AppName = "Example",
            })
            .UseHarfBuzz()
            .UseWin32()
            .UseSkia()            
            .WithInterFont()
            .LogToTrace();
    }
}
