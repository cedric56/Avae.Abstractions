using Avae.DAL;
using Avae.DAL.gRPC;
using Avae.DAL.gRPC.Client;
using Avae.DAL.PostgreSQL;
using Avae.DAL.SignalR;
using Avae.DAL.Sqlite;
using Example.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data.Common;

namespace Example.DAL;
public static class Extensions
{
    static string ServerUrl = "https://88.165.230.223:17001";

    static string MagicHubUrl = $"{ServerUrl}/recordHubOfPerson";
    static string SignalHubUrl = $"{ServerUrl}/PersonHub";
    static string OnionUrl = $"{ServerUrl}/{typeof(IMagicOnionLayer).Name}/";

    public static Task<Func<Task>> AddStreamingHub<TObject>(
        this IServiceProvider provider,
        IDBMonitor<TObject> monitor,
        HttpMessageHandler? httpMessageHandler = null)
        where TObject : class, new()
    {
        IDBFactory.Monitors.Add(monitor);
        var channel = provider.GetGrpcHandlerChannel(MagicHubUrl, httpMessageHandler);
        return monitor.AddStreamingHub(channel);
    }

    public static Task<Func<Task>> AddSignalR<TObject>(
        this IServiceProvider provider,
        IDBMonitor<TObject> monitor,
         IRetryPolicy? retryPolicy = null,
        Func<HttpMessageHandler, HttpMessageHandler>? factory = null)
        where TObject : class, new()
    {
        IDBFactory.Monitors.Add(monitor);
        return monitor.AddSignalR(SignalHubUrl, retryPolicy, factory);
    }

    public static void UseDBOnionLayer(this IServiceCollection services)
    {
        services.AddSingleton<IDBMonitor<Person>>(new DBMonitor<Person>());
        services.AddSingleton<IXmlHttpRequest>(sp => new XmlHttpRequest(OnionUrl));
        services.AddSingleton(sp => sp.Create<IMagicOnionLayer>(ServerUrl));
        services.UseLayer(sp => new MagicOnionLayer(sp, sp.GetService<ILogger>()));
    }

    public static void UseDBSqlLayer<TDBConnection>(this IServiceCollection services)
        where TDBConnection : DbConnection, new()
    {
        var type = typeof(TDBConnection);

        services.AddSingleton<IDBMonitor<Person>>(new DBMonitor<Person>());

        if (type == typeof(SqliteConnection))
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbPath = Path.Combine(folder, "database.db");
            var connectionString = $"Data Source={dbPath};Foreign Keys=True";
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

        services.UseLayer(sp => new DBLayer(sp),
        () =>
        {
            if (type == typeof(SqliteConnection))
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
            else if (type == typeof(NpgsqlConnection))
            {
                return @"CREATE TABLE IF NOT EXISTS Person (
                            Id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                            FirstName VARCHAR(255) NULL,
                            LastName VARCHAR(255) NULL,
                            Photo BYTEA NULL
                        );

                        CREATE TABLE IF NOT EXISTS Contact (
                            Id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                            IdPerson INT NOT NULL,
                            IdContact INT NOT NULL,
                            CONSTRAINT FK_Contact_Person FOREIGN KEY (IdPerson) REFERENCES Person(Id),
                            CONSTRAINT FK_Contact_ContactPerson FOREIGN KEY (IdContact) REFERENCES Person(Id)
                        );";
            }
            else
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
