using Avalonia;
using Avalonia.Labs.Notifications;
using Example.DAL;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Example.Windows;

class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
    {
        return App.CreateApp(
            services =>
            {
                services.UseDBSqlLayer<SqliteConnection>();
                //services.UseDBOnionLayer();
                services.AddSingleton<ILogger>(LoggerFactory.Create(b => b.AddDebug()).CreateLogger<App>());
            })
            .WithAppNotifications(new AppNotificationOptions()
            {
                AppIcon = "C:\\Users\\cedri\\source\\repos\\Avae.Abstractions\\Samples\\Example\\Assets\\avalonia-logo.ico",
                AppName = "Example"
            })
            .WithDataAnnotationsValidation()
            .UseHarfBuzz()
            .UseWin32()
            .UseSkia()
            .LogToTrace();
    }
}
