using Avalonia;
using Avalonia.Labs.Notifications;
using System;
namespace Example.Desktop;

class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
    {
        return App.Configure()
            .WithAppNotifications(new AppNotificationOptions()
            {
                AppIcon = "C:\\Users\\cedri\\source\\repos\\Avae.Abstractions\\Samples\\Example\\Assets\\avalonia-logo.ico",
                AppName = "Example"
            })
            .WithDataAnnotationsValidation()
            .UsePlatformDetect()
            .LogToTrace();
    }
}