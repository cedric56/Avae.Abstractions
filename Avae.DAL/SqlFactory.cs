using Avae.Abstractions;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.Data.Common;

namespace Avae.DAL
{
    public class SqlFactory<TDbConnection>(string connectionString) : DbProviderFactory,
        IDbFactory
        where TDbConnection : DbConnection, new()
    {
        public List<IDbMonitor> Monitors { get; } = [];

        public override DbConnection? CreateConnection()
        {
            var connection = new TDbConnection()
            {
                ConnectionString = connectionString
            };
            connection.Open();
            if (connection is SqliteConnection sqlite)
            {
                raw.sqlite3_update_hook(sqlite.Handle, (object user_data, int type, string database, string table, long rowid) =>
                {
                    foreach (var monitor in Monitors.OfType<SqlMonitor>())
                        monitor.OnSqliteChanged(user_data, type, database, table, rowid);

                }, null);
            }

            return connection;
        }

        public void AddDbMonitor<T>() where T : class, new()
        {
            Monitors.Add(new SqlMonitor<T>(connectionString, typeof(TDbConnection)));
        }
    }
}
