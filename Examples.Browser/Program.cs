using Avae.Essentials;
using Avalonia;
using Avalonia.Browser;
using Example;
using Example.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

internal sealed partial class Program
{
    private static Task Main(string[] args)=>BuildAvaloniaApp()
                .StartBrowserAppAsync("out")
                .ContinueWith(async t =>
                {
                    await JSHost.ImportAsync("essentials", $"/essentials.js");
                });

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<BrowserApp>();
        //.UseReactiveUI(() => { });


    public class BrowserApp : Example.App
    {
        protected override string Logs => string.Empty;

        public override async void Configure(IServiceCollection services)
        {
            base.Configure(services);

            services.UseDBOnionLayer(out _, out _);
        }
    }
}
