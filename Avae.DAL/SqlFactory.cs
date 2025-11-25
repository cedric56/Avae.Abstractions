using Microsoft.Data.Sqlite;
using SQLitePCL;
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
            if (connection is SqliteConnection sqlite)
            {
                //Sqlite only raise database changes on current connection
                raw.sqlite3_update_hook(sqlite.Handle, (object user_data, int type, string database, string table, long rowid) =>
                {
                    foreach (var monitor in Monitors.OfType<SqlMonitor>())
                        monitor.OnSqliteChanged(type switch
                        {
                            9 => ChangeType.Delete,
                            18 => ChangeType.Insert,
                            23 => ChangeType.Update,
                            _ => ChangeType.None

                        }, database, table, rowid);

                }, null);
            }

            return connection;
        }

        public SqlMonitor<T> AddDbMonitor<T>() where T : class, new()
        {
            var monitor = new SqlMonitor<T>(connectionString, typeof(TDbConnection));
            Monitors.Add(monitor);
            return monitor;
        }
    }
}
