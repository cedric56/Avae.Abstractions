using Avae.DAL;
using Avalonia;
using Avalonia.Browser;
using Example;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

internal sealed partial class Program
{
    private static Task Main(string[] args)=>
        BuildAvaloniaApp().WithInterFont().StartBrowserAppAsync("out");
    
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<BrowserApp>();


    public class BrowserApp : App
    {
        public BrowserApp()
            : base()
        {
            
        }

        public override void Configure(IServiceCollection services)
        {
            base.Configure(services);

            services.AddTransient<IXmlHttpRequest, XmlHttpRequest>();
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
