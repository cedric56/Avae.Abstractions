using Avae.DAL;
using Avae.MagicClient;
using Avae.MagicLayer;
using Avae.SignalR;
using Avae.Sqlite;
using Example.Models;
using GrpcWebSocketBridge.Client;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace Example.DAL;
public static class Extensions
{
    static string ServerUrl = "https://localhost:5001";

    static string MagicHubUrl = $"{ServerUrl}/OnionHub";
    static string SignalHubUrl = $"{ServerUrl}/PersonHub";
    static string OnionUrl = $"{ServerUrl}/{typeof(IMagicOnionLayer).Name}/";

    private static string ConnectionString
    {
        get
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbPath = Path.Combine(folder, "database.db");
            return $"Data Source={dbPath};Foreign Keys=True";
        }
    }

    public static Task<Func<Task>> AddStreamingHub<TObject>(
        this IServiceProvider provider,
        IDBMonitor<TObject> monitor)
        where TObject : class, new()
    {
        IDBFactory.Monitors.Add(monitor);
        var channel = provider.GetGrpcHandlerChannel(MagicHubUrl);
        return monitor.AddStreamingHub(channel);
    }

    public static Task<Func<Task>> AddSignalR<TObject>(
        this IServiceProvider provider,
        IDBMonitor<TObject> monitor)
        where TObject : class, new()
    {
        IDBFactory.Monitors.Add(monitor);
        return monitor.AddSignalR(SignalHubUrl);
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

    public static void UseDBOnionLayer(this IServiceCollection services)
    {
        services.AddSingleton<IDBMonitor<Person>>(new DBMonitor<Person>());
        services.AddSingleton<IXmlHttpRequest>(sp => new XmlHttpRequest(OnionUrl));
        services.AddSingleton(sp => sp.Create<IMagicOnionLayer>(ServerUrl));
        services.UseLayer(sp => new MagicOnionLayer(sp));        
    }

    public static void UseDBSqlLayer<TDBConnection>(this IServiceCollection services)
        where TDBConnection : DbConnection, new()
    {
        var type = GetConnectionType<TDBConnection>();

        services.AddSingleton<IDBMonitor<Person>>(new DBMonitor<Person>());

        if (type == DBConnectionType.Sqlite)
            services.UseSqliteFactory(ConnectionString);
        else
            services.UseFactory<TDBConnection>(ConnectionString);

        services.UseLayer(sp => new DBLayer(sp), type,
        () =>
        {
            if (type == DBConnectionType.Sqlite)
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
            else if (type == DBConnectionType.Microsoft)
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
        });
    }
}
