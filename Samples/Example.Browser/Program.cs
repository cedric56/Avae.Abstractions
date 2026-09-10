using Avalonia;
using Avalonia.Browser;
using Avalonia.Labs.Notifications;
using Example;
using Example.DAL;
using System.Threading.Tasks;

internal sealed partial class Program
{
    private static Task Main(string[] args)
    {
            return BuildAvaloniaApp().StartBrowserAppAsync("out");
    }
    public static AppBuilder BuildAvaloniaApp()
    {
        return App.CreateApp(
    services => services.UseDBOnionLayer())
        .WithAppNotifications();
    }
}

