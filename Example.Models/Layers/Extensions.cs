using Avae.DAL;
using Avae.DAL.Interfaces;
using Avae.SignalR;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace Example.Models;
public static class Extensions
{
    const string HubUrl = "http://localhost:5001/PersonHub";

    private static string ConnectionString
    {
        get
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbPath = Path.Combine(folder, "database.db");
            return $"Data Source={dbPath};Foreign Keys=True";
        }
    }

    public static void UseDBOnionLayer(this IServiceCollection services, out ISignalRService? signal, out Action? unsuscribe)
    {
        signal = null;
        unsuscribe = null;

        var monitor = new SqlMonitor<Person>();
        if (!OperatingSystem.IsBrowser())
        {
            signal = monitor.AddSignalR(HubUrl, out unsuscribe);
        }

        services.AddTransient<IXmlHttpRequest>(sp => new XmlHttpRequest("http://localhost:5001/routes/IDBOnionService/"));
        services.UseSqlLayer<IDBLayer>(sp => new DBOnionLayer(sp));
        services.AddScoped(sp =>
        {
            return sp.GetMagicOnion<IDBOnionService>(
                OperatingSystem.IsBrowser() ?
                "http://localhost:5001" :
                "http://localhost:5000");
        });
        services.AddScoped<IOnionService>(provider => provider.GetRequiredService<IDBOnionService>());
        services.AddSingleton<ISqlMonitor<Person>>(monitor);
    }

    public static void UseDBSqlLayer<TDBConnection>(this IServiceCollection services, out ISignalRService? signal, out Action unsuscribe)
        where TDBConnection : DbConnection, new()
    {
        Action unsuscribeSignal = () => { };
        SignalRService? service = null;

        services.UseSqlLayer<IDBLayer>(sp => new DBSqlLayer(sp));
        services.UseSqlMonitors<TDBConnection>(ConnectionString, (factory) =>
        {
            var monitor = factory.AddDbMonitor<Person>();
            service = monitor.AddSignalR(HubUrl, out unsuscribeSignal);
            services.AddSingleton<ISqlMonitor<Person>>(monitor);

        }, true);

        signal = service!;
        unsuscribe = unsuscribeSignal;
    }

    public static void UseDBSqlLayer<TDBConnection>(this IServiceCollection services)
        where TDBConnection : DbConnection, new()
    {
        services.UseSqlLayer<IDBLayer>(sp => new DBSqlLayer(sp));
        services.UseSqlMonitors<TDBConnection>(ConnectionString, (factory) =>
        {
            var monitor = factory.AddDbMonitor<Person>();
            services.AddSingleton<ISqlMonitor<Person>>(monitor);

        }, true);
    }
}
