using Avae.DAL;
using Avae.DAL.Interfaces;
using Avae.SignalR;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Models
{
    public static class Extensions
    {
        public static void UseDBOnionLayer(this IServiceCollection services, out ISignalRService? signal, out Action? unsuscribe)
        {
            signal = null;
            unsuscribe = null;

            var monitor = new SqlMonitor<Person>();
            if (!OperatingSystem.IsBrowser())
            {
                signal = monitor.AddSignalR("http://localhost:5001/PersonHub", out unsuscribe);
            }

            services.AddTransient<IXmlHttpRequest>(sp => new Avae.DAL.XmlHttpRequest("http://localhost:5001/routes/IDBOnionService/"));
            services.UseSqlLayer<IDBLayer>(sp => new DBOnionLayer(sp));
            services.AddScoped<IDBOnionService>(sp =>
            {
                return sp.GetMagicOnion<IDBOnionService>("http://localhost:5000");
            });
            services.AddScoped<IOnionService>(provider => provider.GetRequiredService<IDBOnionService>());
            services.AddSingleton<ISqlMonitor<Person>>(monitor);
        }

        public static void UseDBSqlLayer(this IServiceCollection services, out ISignalRService? signal, out Action unsuscribe)
        {
            Action unsuscribeSignal = () => { };
            SignalRService? service = null;

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbPath = Path.Combine(folder, "database.db");
            var connectionString = $"Data Source={dbPath};Foreign Keys=True";

            services.UseSqlLayer<IDBLayer>(sp => new DBSqlLayer(sp));
            services.UseSqlMonitors<SqliteConnection>(connectionString, (factory) =>
            {
                var monitor = factory.AddDbMonitor<Person>();
                service = monitor.AddSignalR("http://localhost:5001/PersonHub", out unsuscribeSignal);
                services.AddSingleton<ISqlMonitor<Person>>(monitor);

            }, true);

            signal = service!;
            unsuscribe = unsuscribeSignal;
        }
    }
}
