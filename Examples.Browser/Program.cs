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
using System;
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
        => AppBuilder.Configure<BrowserApp>().UseServices();


    public class BrowserApp : App
    {
        public BrowserApp()
            : base()
        {
            
        }

        public override void Configure(IServiceCollection services)
        {
            base.Configure(services);

            services.AddScoped<IOnionService>(_ => GetMagicOnion<IDbService>());
            services.AddSingleton<IDbLayer>(_ => new DBOnionLayer());
            services.AddTransient<IXmlHttpRequest, XmlHttpRequest>();
        }

        private static IGrpc GetMagicOnion<IGrpc>() where IGrpc : IService<IGrpc>
        {
            var client = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
            {
                DefaultRequestVersion = HttpVersion.Version20,
                Timeout = TimeSpan.FromSeconds(5)
            };
            var channel = GrpcChannel.ForAddress(
                OperatingSystem.IsBrowser() ? "http://localhost:5001" : "http://localhost:5000", new GrpcChannelOptions()
                {
                    HttpClient = client,
                });
            return MagicOnionClient.Create<IGrpc>(channel);
        }
    }
}

public partial class XmlHttpRequest : IXmlHttpRequest
{
    [JSImport("globalThis.eval")]
    public static partial string Invoke(string @params);

    public byte[] Send(string urlString, string data)
    {
        string escapedData = data.Replace("\\", "\\\\").Replace("'", "\\'");
        string escapedUrl = urlString.Replace("\\", "\\\\").Replace("'", "\\'");

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
