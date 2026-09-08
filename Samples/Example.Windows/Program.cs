using Avalonia;
using Avalonia.Labs.Notifications;
using Example.DAL;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Example.Windows;

class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
    {
        return App.CreateApp(services =>
            {
                services.UseDBSqlLayer<SqliteConnection>();
                //services.UseDBOnionLayer();
                services.AddSingleton<ILogger>(LoggerFactory.Create(b => b.AddDebug()).CreateLogger<App>());
            })
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
