using Avae.DAL;
using Avae.DAL.Grpc;
using Avae.DAL.Interfaces;
using Avae.SignalR;
using Avae.Sqlite;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.Common;

namespace Example.Models;
public static class Extensions
{
    const string HubUrl = "http://localhost:5001/PersonHub";
    const string OnionUrl = "http://localhost:5001/IDBOnionService/";

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

    private static SqlConnectionType GetConnectionType<TDBConnection>()
    {
        var type = typeof(TDBConnection);
        if (type == typeof(SqlConnection))
            return SqlConnectionType.Microsoft;
        else if (type == typeof(SqliteConnection))
            return SqlConnectionType.Sqlite;
        return SqlConnectionType.Unspecified;
    }

    public static void UseDBOnionLayer(this IServiceCollection services, out ISignalRService? signal, out Action? unsuscribe)
    {
        signal = null;
        unsuscribe = null;

        var monitor = new SqlMonitor<Person>();
        //if (!OperatingSystem.IsBrowser())
        //{
            signal = monitor.AddSignalR(HubUrl, out unsuscribe);
        //}
        
        services.AddSingleton<IXmlHttpRequest>(sp => new XmlHttpRequest(OnionUrl));
        services.UseSqlLayer(sp => new OnionLayer(sp));
        services.AddSingleton(sp =>
        {
            return sp.GetMagicOnion<IOnionService>(
                OperatingSystem.IsBrowser() ?
                "http://localhost:5001" :
                "http://localhost:5000");
        });
        //services.AddSingleton<IOnionService>(provider => provider.GetRequiredService<IOnionService>());
        services.AddSingleton<ISqlMonitor<Person>>(monitor);
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
        Action<SqlMonitor<Person>>? action = null)
        where TDBConnection : DbConnection, new()
    {
        var type = GetConnectionType<TDBConnection>();
        services.UseSqlLayer(sp =>
        {
            //Create db
            using var connection = sp.GetService<IDbConnection>();
            if (connection is not null)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = GetCommandText(connection);
                cmd.ExecuteNonQuery();
            }

            return new SqlLayer(sp);

        }, type);
        if (type == SqlConnectionType.Sqlite)
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
            services.UseSqlFactory<TDBConnection>(ConnectionString, (factory) =>
            {
                var monitor = factory.AddDbMonitor<Person>();
                action?.Invoke(monitor);
                services.AddSingleton<ISqlMonitor<Person>>(monitor);
            });
        }
    }
}
