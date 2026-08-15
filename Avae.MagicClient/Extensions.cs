using Avae.DAL;
using Avae.MagicServices;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Grpc.Net.Client.Web;
using GrpcWebSocketBridge.Client;
using MagicOnion;
using MagicOnion.Client;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.WebSockets;

namespace Avae.MagicClient
{
    public static class Extensions
    {
        public static async Task<Func<Task>> AddStreamingHub<TObject>(this IDBMonitor<TObject> monitor, GrpcChannel channel)
            where TObject : class, new()
        {
            try
            {
                var receiver = new RecordHubReceiver<TObject>(monitor);
                var hub = await StreamingHubClient.ConnectAsync<IRecordHub<TObject>, IRecordHubReceiver<TObject>>(channel, receiver);//, cancellationToken: cts.Token);
                var guid = await hub.AddReceiverAsync();
                IDBLayer.Sessions.Add(typeof(TObject), guid.ToString());
                monitor.OnRecordChanged += OnRecordChanged;
                return async () =>
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Console.WriteLine(ex);
                return () => Task.CompletedTask;
            }
        }

        public static IMagicService Create<IMagicService>(this IServiceProvider provider, string url) where IMagicService : IService<IMagicService>
        {
            //var channel = OperatingSystem.IsBrowser() ? provider.GetGrpcWebChannel(url) : provider.GetGrpcSocketChannel(url);
            var channel = provider.GetGrpcSocketChannel(url);
            return MagicOnionClient.Create<IMagicService>(channel);
        }

        private static GrpcChannel GetGrpcWebChannel(this IServiceProvider provider, string url)
        {
            var client = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
            {
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact,
                DefaultRequestVersion = HttpVersion.Version20,
                Timeout = TimeSpan.FromSeconds(5)
            };
            return GrpcChannel.ForAddress(url, new GrpcChannelOptions()
            {
                HttpClient = client
            });
        }

        public static GrpcChannel GetGrpcSocketChannel(this IServiceProvider provider, string url)
        {
            var client = new HttpClient(new GrpcWebSocketBridgeHandler());
            return GrpcChannel.ForAddress(url, new GrpcChannelOptions()
            {
                HttpClient = client
            });
        }

        public static GrpcChannel GetGrpcHandlerChannel(this IServiceProvider provider, string url)
        {
            return GrpcChannel.ForAddress(url, new GrpcChannelOptions()
            {
                HttpHandler = new GrpcWebSocketBridgeHandler()
            });
        }
    }
}