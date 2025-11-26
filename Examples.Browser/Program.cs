using Avae.DAL;
using Avae.DAL.Interfaces;
using Avae.Services;
using Avalonia;
using Avalonia.Browser;
using Example;
using Example.Models;
using Examples.Browser;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using MagicOnion;
using MagicOnion.Client;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Avalonia;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

internal sealed partial class Program
{
    private static Task Main(string[] args)=>
        BuildAvaloniaApp().WithInterFont().StartBrowserAppAsync("out");
    
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<BrowserApp>().UseServices().UseReactiveUI();


    public class BrowserApp : App
    {
        protected override string Logs => string.Empty;

        public override void Configure(IServiceCollection services)
        {
            base.Configure(services);

            var monitor = new SqlMonitor<Person>();
            monitor.AddSignalR("http://localhost:5001/PersonHub");
            services.AddSingleton<ISqlMonitor<Person>>(monitor);

            services.AddScoped<IDBOnionService>(_ => GetMagicOnion<IDBOnionService>());
            services.AddScoped<IOnionService>(provider => provider.GetRequiredService<IDBOnionService>());
            services.AddTransient<IXmlHttpRequest, XmlHttpRequest>();
            services.UseDbLayer<IDBLayer, DBOnionLayer>();
        }

        private static IGrpc GetMagicOnion<IGrpc>() where IGrpc : IService<IGrpc>
        {
            var client = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
            {
                DefaultRequestVersion = HttpVersion.Version20,
                Timeout = TimeSpan.FromSeconds(5)
            };
            var channel = GrpcChannel.ForAddress(
                "http://localhost:5001", new GrpcChannelOptions()
                {
                    HttpClient = client,
                });
            return MagicOnionClient.Create<IGrpc>(channel);
        }
    }
}
