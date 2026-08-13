using Avae.DAL;
using Avae.MagicServices;
using Grpc.Net.Client;
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
                var sessionId = Guid.NewGuid();
                var receiver = new RecordHubReceiver<TObject>(sessionId, monitor);
                var hub = await StreamingHubClient.ConnectAsync<IRecordHub<TObject>, IRecordHubReceiver<TObject>>(channel, receiver);
                await hub.AddReceiverAsync();
                monitor.OnRecordChanged += OnRecordChanged;

                void OnRecordChanged(object? sender, Record<TObject> e)
                {
                    if (receiver.SessionId != null)
                        e.Connections.Add(receiver.SessionId?.ToString() ?? string.Empty);

                    hub.OnRecordChanged(e);
                }
                IDBFactory.Monitors.Add(monitor);
                return async () =>
                {
                    monitor.OnRecordChanged -= OnRecordChanged;
                    await hub.RemoveAsync();
                    await hub.WaitForDisconnectAsync();
                    await hub.DisposeAsync();
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return () => Task.CompletedTask;
            }
        }

        public static IMagicService Create<IMagicService>(this IServiceProvider provider, string url) where IMagicService : IService<IMagicService>
        {
            var channel = GetGrpcChannel(url);
            return MagicOnionClient.Create<IMagicService>(channel);
        }

        public static GrpcChannel GetGrpcChannel(string url)
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
