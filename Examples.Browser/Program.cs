using Avae.DAL;
using Avae.DAL.Interfaces;
using Avae.SignalR;
using Avalonia;
using Avalonia.Browser;
using Example;
using Example.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

internal sealed partial class Program
{
    private static Task Main(string[] args)=>
        BuildAvaloniaApp().WithInterFont().StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<BrowserApp>();
        //.UseReactiveUI(() => { });


    public class BrowserApp : App
    {
        protected override string Logs => string.Empty;

        public override void Configure(IServiceCollection services)
        {
            base.Configure(services);

            services.AddTransient<IXmlHttpRequest>(sp => new Avae.DAL.XmlHttpRequest("http://localhost:5001/routes/IDBOnionService/"));            
            services.UseDbLayer<IDBLayer>(sp => new DBOnionLayer(sp));
            services.AddScoped<IDBOnionService>(sp =>
            {
                return sp.GetMagicOnion<IDBOnionService>("http://localhost:5000");
            });
            services.AddScoped<IOnionService>(provider => provider.GetRequiredService<IDBOnionService>());
            services.AddSingleton<ISqlMonitor<Person>>(provider =>
            {
                var monitor = new SqlMonitor<Person>();
                var e = monitor.AddSignalR("http://localhost:5001/PersonHub", out _);
                return monitor;
            });
        }
    }
}
