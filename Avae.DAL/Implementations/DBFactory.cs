using Avae.DAL.Interfaces;
using System.Data.Common;

namespace Avae.DAL;

public class DBFactory<TDbConnection>(string connectionString) : DbProviderFactory,
    IDBFactory
    where TDbConnection : DbConnection, new()
{
    public List<IDBMonitor> Monitors { get; } = [];

    public override DbConnection? CreateConnection()
    {
        var connection = new TDbConnection()
        {
            ConnectionString = connectionString
        };
        connection.Open();
        return connection;
    }

    public DBMonitor<T> AddDbMonitor<T>() where T : class, new()
    {
        var monitor = new DBMonitor<T>();
        Monitors.Add(monitor);
        return monitor;
    }
}
