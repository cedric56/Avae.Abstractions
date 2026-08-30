using Grpc.Net.Client;
using GrpcWebSocketBridge.Client;
using MagicOnion;
using MagicOnion.Client;
using System.Net.Security;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;

namespace Avae.DAL.gRPC.Client;

public static class Extensions
{
    private static Func<Task>? _dispose;

    public static async Task<Func<Task>> AddStreamingHub<TObject>(this IDBMonitor<TObject> monitor, GrpcChannel channel)
        where TObject : class, new()
    {
        try
        {
            if (IDBLayer.Sessions.TryGetValue(typeof(TObject), out _))
                return _dispose ?? (() => Task.CompletedTask);

            var receiver = new RecordHubReceiver<TObject>(monitor);
            var hub = await StreamingHubClient.ConnectAsync<IRecordHub<TObject>, IRecordHubReceiver<TObject>>(channel, receiver);//, cancellationToken: cts.Token);
            var guid = await hub.AddReceiverAsync();
            IDBLayer.Sessions.Add(typeof(TObject), guid.ToString());
            monitor.OnRecordChanged += OnRecordChanged;
            return _dispose = async () =>
            {
                try
                {
                    await hub.RemoveAsync();
                    await hub.WaitForDisconnectAsync();
                    await hub.DisposeAsync();
                }
                finally
                {
                    monitor.OnRecordChanged -= OnRecordChanged;
                }
            };

            void OnRecordChanged(object? sender, Record<TObject> e)
            {
                IDBLayer.Sessions.TryGetValue(typeof(TObject), out var sessionId);
                e.Add(sessionId);
                hub.OnRecordChanged(e);
            }
        }
        catch
        {
            return () => Task.CompletedTask;
        }
    }

    public static IMagicService Create<IMagicService>(this IServiceProvider provider, string url) where IMagicService : IService<IMagicService>
    {
        var handler = new GrpcWebSocketBridgeHandler();
        if (!OperatingSystem.IsBrowser() && handler.InnerHandler is HttpClientHandler httpHandler)
            httpHandler.ServerCertificateCustomValidationCallback = ValidateCertificates2;
        var client = new HttpClient(handler);
        var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions()
        {
            HttpClient = client
        });
        return MagicOnionClient.Create<IMagicService>(channel);
    }

    public static GrpcChannel GetGrpcHandlerChannel(this IServiceProvider provider, string url, HttpMessageHandler? httpMessageHandler = null)
    {
        if (httpMessageHandler != null)
            return GrpcChannel.ForAddress(url, new GrpcChannelOptions()
            {
                HttpHandler = httpMessageHandler
            });

        var handler = new GrpcWebSocketBridgeHandler();
        if (!OperatingSystem.IsBrowser() && handler.InnerHandler is HttpClientHandler httpHandler)
            httpHandler.ServerCertificateCustomValidationCallback = ValidateCertificates2;        
        return GrpcChannel.ForAddress(url, new GrpcChannelOptions()
        {
            HttpHandler = handler
        });
    }

    public static bool ValidateCertificates2(HttpRequestMessage message, X509Certificate2? x509Certificate, X509Chain? x509Chain, SslPolicyErrors errors)
    {
        if (x509Certificate == null) return false;
        return true;
    }

    public static bool ValidateCertificates(object sender, X509Certificate? x509Certificate, X509Chain? x509Chain, SslPolicyErrors errors)
    {
        if (x509Certificate == null) return false;
        return true;
    }
}