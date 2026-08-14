using Avae.DAL;
using Avae.MagicServices;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Grpc.Net.Client.Web;
using MagicOnion;
using MagicOnion.Client;
using System.Diagnostics;
using System.Net;

namespace Avae.MagicClient
{
    public static class Extensions
    {
        public static async Task<Func<Task>> AddStreamingHub<TObject>(this IDBMonitor<TObject> monitor, GrpcChannel channel)
            where TObject : class, new()
        {
            try
            {
                if (OperatingSystem.IsBrowser())
                    throw new NotImplementedException("Use SignalR for WebAssembly");

                //using var cts = new CancellationTokenSource(
                //    TimeSpan.FromSeconds(2));

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
            var channel = provider.GetGrpcChannel(url);
            return MagicOnionClient.Create<IMagicService>(channel);
        }

        public static GrpcChannel GetGrpcChannel(this IServiceProvider provider, string url)
        {
            var client = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
            {
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact,
                DefaultRequestVersion = HttpVersion.Version20,
                Timeout = TimeSpan.FromSeconds(5)
            };
            return GrpcChannel.ForAddress(url, new GrpcChannelOptions()
            {
                HttpClient = client,
            });
        }
    }
}
