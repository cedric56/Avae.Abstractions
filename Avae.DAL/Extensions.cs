using Avae.DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace Avae.DAL
{
    public static class Extensions
    {
        public static void UseSqlMonitors<TDBConnection>(this IServiceCollection services,
            string connectionString, Action<SqlFactory<TDBConnection>>? action = null)
            where TDBConnection : DbConnection, new()
        {
            var factory = new SqlFactory<TDBConnection>(connectionString);
            action?.Invoke(factory);
            services.AddSingleton<IDbFactory>(factory);
            services.AddTransient(_ => factory.CreateConnection()!);
        }

        public static void UseDbLayer<IDbLayer>(this IServiceCollection services, Func<IServiceProvider, IDbLayer> getLayer)
            where IDbLayer : class, IDataAccessLayer
            //where TDbLayer : IDbLayer, IDataAccessLayer, new()
        {
            services.AddSingleton<IDbLayer>(sp => getLayer(sp));
            services.AddSingleton<IDataAccessLayer>(sp => sp.GetRequiredService<IDbLayer>());
        }
    }
}
