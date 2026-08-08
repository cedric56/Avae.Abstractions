using Avalonia;
using Avalonia.Browser;
using Example;
using Example.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

internal sealed partial class Program
{
    private static Task Main(string[] args)=>BuildAvaloniaApp()
                .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<BrowserApp>();
        //.UseReactiveUI(() => { });


    public class BrowserApp : App
    {
        protected override string Logs => string.Empty;

        public override void Configure(IServiceCollection services)
        {
            base.Configure(services);

            services.UseDBOnionLayer(out _, out _);
        }
    }
}
