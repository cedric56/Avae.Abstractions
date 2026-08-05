using Avae.DAL.Interfaces;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using MagicOnion;
using MagicOnion.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.Common;
using System.Net;

namespace Avae.DAL
{
    public static class Extensions
    {
        public static void  UseSqlMonitors<TDBConnection>(this IServiceCollection services,
            string connectionString, Action<SqlFactory<TDBConnection>>? action = null)
            where TDBConnection : DbConnection, new()
        {
            var factory = new SqlFactory<TDBConnection>(connectionString);

            action?.Invoke(factory);
            services.AddSingleton<IDbFactory>(sp => factory);
            services.AddTransient<IDbConnection>(_ => factory.CreateConnection()!);
        }

        public static void UseDbLayer<IDbLayer>(this IServiceCollection services, Func<IServiceProvider, IDbLayer> getLayer)
            where IDbLayer : class, IDataAccessLayer
        {
            services.AddSingleton<IDbLayer>(sp => getLayer(sp));
            services.AddSingleton<IDataAccessLayer>(sp => sp.GetRequiredService<IDbLayer>());
        }

        public static IGrpc GetMagicOnion<IGrpc>(this IServiceProvider provider, string url) where IGrpc : IService<IGrpc>
        {
            var client = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
            {
                DefaultRequestVersion = HttpVersion.Version20,
                Timeout = TimeSpan.FromSeconds(5)
            };
            var channel = GrpcChannel.ForAddress(
                url, new GrpcChannelOptions()
                {
                    HttpClient = client,
                });
            return MagicOnionClient.Create<IGrpc>(channel);
        }
    }
}
