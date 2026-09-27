using Avae.DAL;
using Dommel;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data.Common;

namespace Example.Models;

public static class Constants
{
    public static string ServerUrl = "https://88.165.230.223:17001";
    public static string MagicHubUrl = $"{ServerUrl}/recordHubOfPerson";
    public static string SignalHubUrl = $"{ServerUrl}/PersonHub";
    public static string OnionUrl = $"{ServerUrl}/{typeof(IMagicOnionLayer).Name}/";
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersonServiceLocal<TDBConnection>(this IServiceCollection services)// DbProviderFactory factory)
        where TDBConnection : DbConnection, new()
    {
        var type = typeof(TDBConnection);

        if (type == typeof(SqliteConnection))
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbPath = Path.Combine(folder, "database.db");
            var connectionString = $"Data Source={dbPath};Foreign Keys=True";
            DommelMapper.AddSqlBuilder(typeof(DBLogConnection), new SqliteSqlBuilder());
            services.UseSqliteFactory(connectionString);
        }
        else if (type == typeof(NpgsqlConnection))
        {
            var connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=Postgre";
            services.UseNpgsqlFactory(connectionString);
        }
        else
        {
            var connectionString = @"Server=Desktop\\SQLEXPRESS;Database=Kundalini;User ID=cedric;Password=Ex@duS56;TrustServerCertificate=True";
            services.UseFactory<TDBConnection>(connectionString);
        }

        return services
            .AddSingleton<IEntityMapper, DommelEntityMapper>()
            .AddSingleton<IEntityCache<Person>>(sp =>
            {
                return new InMemoryEntityCache<Person>(
                    p => p.Id,
                    sp.GetRequiredService<IEntityMapper>(),
                    sp.GetService<IDBMonitor<Person>>());
            })
            .AddSingleton<IPersonGraph, PersonGraph>()
            .AddSingleton<IPersonService, PersonServiceLocal>()
            .AddSingleton<IDBMonitor<Person>>(new DBMonitor<Person>());
    }

    public static IServiceCollection AddPersonServiceRemote(this IServiceCollection services)
    {
        return services
            .AddSingleton<IDBFactory, RemoveFactory>()
            .AddSingleton<IEntityCache<Person>>(sp =>
            {
                return new InMemoryRemoteEntityCache<Person>(
                    sp.GetRequiredService<MagicOnionLayer>(),
                    p => p.Id,
                    sp.GetService<IDBMonitor<Person>>());
            })
            .AddSingleton(sp => sp.Create<IMagicOnionLayer>(Constants.ServerUrl))
            .AddSingleton<IPersonGraph, PersonGraph>()
            .AddSingleton<IPersonService, PersonServiceRemote>()
            .AddSingleton<IDBMonitor<Person>>(new DBMonitor<Person>())
            .AddSingleton<MagicOnionLayer>(sp => new MagicOnionLayer(
                sp.GetRequiredService<IMagicOnionLayer>(),
                1000));
        //logger: sp.GetService<ILogger>()));
    }

    class RemoveFactory : IDBFactory
    {
        public List<IDBMonitor> Monitors { get; } = new();
        public Dictionary<Type, string> Sessions { get; } = new();

        public DbConnection? CreateConnection()
        {
            throw new NotImplementedException();
        }
    }
}