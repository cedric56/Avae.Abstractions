using Avalonia;
using Avalonia.Browser;
using Avalonia.Labs.Notifications;
using Avalonia.Media;
using Example;
using Example.DAL;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

internal sealed partial class Program
{
    private static Task Main(string[] args) => 
        BuildAvaloniaApp().StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<BrowserApp>()
                        .WithAppNotifications()
                        .WithInterFont();

    public class BrowserApp : App
    {
        public override async void Configure(IServiceCollection services)
        {
            base.Configure(services);

            services.UseDBOnionLayer();
        }
    }
}

