using Avae.DAL;
using Avae.MagicOnion;
using Avae.SignalR;
using Avae.Sqlite;
using Example.Models;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using MagicOnion.Client;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.Common;
using System.Net;

namespace Example.DAL;
public static class Extensions
{
    static string HubUrl = "http://localhost:5001/PersonHub";
    static string OnionUrl = $"http://localhost:5001/{typeof(IMagicOnionLayer).Name}/";

    private static string GetCommandText(IDbConnection connection)
    {
        if (connection is SqliteConnection)
        {
            return @"
            CREATE TABLE IF NOT EXISTS Person(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FirstName TEXT,
                LastName TEXT
            );

            CREATE TABLE IF NOT EXISTS Contact(
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            IdPerson INTEGER  NOT NULL,
                            IdContact INTEGER  NOT NULL,
                            CONSTRAINT FK_Contact_Person FOREIGN KEY(IdPerson) REFERENCES Person(Id),
                            CONSTRAINT FK_Contact_ContactPerson FOREIGN KEY(IdContact) REFERENCES Person(Id)
                        );
            ";
        }
        else if (connection is SqlConnection)
        {
            return @"CREATE TABLE IF NOT EXISTS Person (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        FirstName NVARCHAR(255) NULL,
                        LastName NVARCHAR(255) NULL,
                        Photo VARBINARY(MAX) NULL
                    );

                    CREATE TABLE IF NOT EXISTS Contact (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        IdPerson INT NOT NULL,
                        IdContact INT NOT NULL,
                        CONSTRAINT FK_Contact_Person FOREIGN KEY (IdPerson) REFERENCES Person(Id),
                        CONSTRAINT FK_Contact_ContactPerson FOREIGN KEY (IdContact) REFERENCES Person(Id)
                    );";
        }

        throw new NotImplementedException();
    }

    private static string ConnectionString
    {
        get
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbPath = Path.Combine(folder, "database.db");
            return $"Data Source={dbPath};Foreign Keys=True";
        }
    }

    private static DBConnectionType GetConnectionType<TDBConnection>()
    {
        var type = typeof(TDBConnection);
        if (type == typeof(SqlConnection))
            return DBConnectionType.Microsoft;
        else if (type == typeof(SqliteConnection))
            return DBConnectionType.Sqlite;
        return DBConnectionType.Unspecified;
    }

    public static void UseDBOnionLayer(this IServiceCollection services, out ISignalRService? signal, out Action? unsuscribe)
    {
        signal = null;
        unsuscribe = null;

        var monitor = new DBMonitor<Person>();
        signal = monitor.AddSignalR(HubUrl, out unsuscribe);
        services.AddSingleton<ISqlMonitor<Person>>(monitor);
        services.AddSingleton<IXmlHttpRequest>(sp => new XmlHttpRequest(OnionUrl));
        services.AddSingleton(sp =>
        {
            var channel = sp.GetGrpcChannel(OperatingSystem.IsBrowser() ? "http://localhost:5001" : "http://localhost:5000");
            return MagicOnionClient.Create<IMagicOnionLayer>(channel);
        });
        services.UseLayer(sp => new MagicOnionLayer(sp));        
    }

    public static void UseDBSqlLayer<TDBConnection>(this IServiceCollection services, out ISignalRService? signal, out Action unsuscribe)
        where TDBConnection : DbConnection, new()
    {
        Action unsuscribeSignal = () => { };
        SignalRService? service = null;
        services.UseDBSqlLayer<TDBConnection>(monitor =>
        {
            service = monitor.AddSignalR(HubUrl, out unsuscribeSignal);
        });
        signal = service!;
        unsuscribe = unsuscribeSignal;
    }

    public static void UseDBSqlLayer<TDBConnection>(this IServiceCollection services)
        where TDBConnection : DbConnection, new()
    {
        services.UseDBSqlLayer<TDBConnection>(null);
    }

    public static void UseDBSqlLayer<TDBConnection>(this IServiceCollection services,
        Action<DBMonitor<Person>>? action = null)
        where TDBConnection : DbConnection, new()
    {
        var type = GetConnectionType<TDBConnection>();
        services.UseLayer(sp =>
        {
            CreateDB(sp);
            return new DBLayer(sp);

        }, type);

        if (type == DBConnectionType.Sqlite)
        {
            services.UseSqliteFactory(ConnectionString, (factory) =>
            {
                var monitor = factory.AddDbMonitor<Person>();
                action?.Invoke(monitor);
                services.AddSingleton<ISqlMonitor<Person>>(monitor);
            });
        }
        else
        {
            services.UseFactory<TDBConnection>(ConnectionString, (factory) =>
            {
                var monitor = factory.AddDbMonitor<Person>();
                action?.Invoke(monitor);
                services.AddSingleton<ISqlMonitor<Person>>(monitor);
            });
        }

        void CreateDB(IServiceProvider provider)
        {
            using var connection = provider.GetService<IDbConnection>();
            if (connection is not null)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = GetCommandText(connection);
                cmd.ExecuteNonQuery();
            }
        }
    }

    private static GrpcChannel GetGrpcChannel(this IServiceProvider provider, string url)
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
