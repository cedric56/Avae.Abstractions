using Avalonia;
using Avalonia.Labs.Notifications;
using Example.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Example.Windows;

class Program
{
    [STAThread]
    public static void Main(string[] args) =>
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp() =>
        App.CreateApp(
            services =>
            {
                services.AddPersonServiceLocal<SqliteConnection>();
                services.AddSingleton<ILogger>(LoggerFactory.Create(b => b.AddDebug()).CreateLogger<App>());
            })
            .WithAppNotifications(new AppNotificationOptions()
            {
                AppIcon = Path.Combine(AppContext.BaseDirectory, "avalonia-logo.ico"),
                AppName = "Example"
            })
            .WithDataAnnotationsValidation()
            .UseHarfBuzz()
            .UseWin32()
            .UseSkia()
            .LogToTrace();
}
