using Avae.DAL.Interfaces;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Avae.DAL
{
    public partial class XmlHttpRequest(string url) : IXmlHttpRequest
    {
        public bool IsConnected { get; set; }

        [JSImport("globalThis.eval")]
        public static partial string Invoke(string @params);

        [JsonSerializable(typeof(Result))]
        internal partial class SourceGenerationContext : JsonSerializerContext { }

        public Result Send(string urlString, string data)
        {
            Debug.WriteLine(urlString);

            string escapedData = data.Replace("\\", "\\\\").Replace("'", "\\'");
            string escapedUrl = string.Concat(url, urlString).Replace("\\", "\\\\").Replace("'", "\\'");

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
            return JsonSerializer.Deserialize<Result>(response, SourceGenerationContext.Default.Result) ?? new Result()
            {
                Successful = false,
                Exception = $"Unable to access webservice on {escapedUrl}?{escapedData}"
            };
        }
    }
}
