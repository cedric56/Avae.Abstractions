using Avalonia;
using Avalonia.Browser;
using Avalonia.Labs.Notifications;
using Example;
using Example.Models;
using System.Threading.Tasks;

internal sealed partial class Program
{
    private static Task Main(string[] args) =>
            BuildAvaloniaApp().StartBrowserAppAsync("out");
    public static AppBuilder BuildAvaloniaApp() =>
        App.CreateApp(services => services.AddPersonServiceRemote())
                  .WithAppNotifications();
}

