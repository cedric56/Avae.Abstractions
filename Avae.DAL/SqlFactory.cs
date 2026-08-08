using Avae.DAL.Interfaces;
using System.Data.Common;

namespace Avae.DAL
{
    public class SqlFactory<TDbConnection>(string connectionString) : DbProviderFactory,
        IDbFactory
        where TDbConnection : DbConnection, new()
    {
        public List<ISqlMonitor> Monitors { get; } = [];

        public override DbConnection? CreateConnection()
        {
            var connection = new TDbConnection()
            {
                ConnectionString = connectionString
            };
            connection.Open();
            return connection;
        }

        public SqlMonitor<T> AddDbMonitor<T>() where T : class, new()
        {
            var monitor = new SqlMonitor<T>();
            Monitors.Add(monitor);
            return monitor;
        }
    }
}
