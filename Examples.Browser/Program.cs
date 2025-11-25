using Avae.DAL;
using Avae.Services;
using Avalonia;
using Avalonia.Browser;
using Example;
using Example.Models;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using MagicOnion;
using MagicOnion.Client;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Avalonia;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Threading.Tasks;

internal sealed partial class Program
{
    private static Task Main(string[] args)=>
        BuildAvaloniaApp().WithInterFont().StartBrowserAppAsync("out");
    
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<BrowserApp>().UseServices().UseReactiveUI();


    public class BrowserApp : App
    {
        public BrowserApp()
            : base()
        {
            
        }

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

public partial class XmlHttpRequest : IXmlHttpRequest
{
    const string URL = "http://localhost:5001/routes/IDBOnionService/";

    [JSImport("globalThis.eval")]
    public static partial string Invoke(string @params);

    public byte[] Send(string urlString, string data)
    {
        Debug.WriteLine(urlString);

        string escapedData = data.Replace("\\", "\\\\").Replace("'", "\\'");
        string escapedUrl = string.Concat(URL, urlString).Replace("\\", "\\\\").Replace("'", "\\'");

        var js = @$"const request = function (url, data) {{
        var xhr = new XMLHttpRequest();
        xhr.open('POST', url, false); 
        xhr.onload = function () {{
            //console.log(this.responseText);
        }};
        if (data)
            //Send the proper header information along with the request
            xhr.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
        xhr.send(data);

        if (xhr.status === 200) {{
            return xhr.responseText;
        }} else {{
            throw new Error(xhr.statusText);
        }}
    }}

    request('{escapedUrl}', '{escapedData}');
    ";

        var response = Invoke(js);
        return JsonSerializer.Deserialize<byte[]>(response);
    }
}
