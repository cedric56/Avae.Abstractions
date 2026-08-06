using Avae.DAL.Interfaces;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.Data.Common;

namespace Avae.DAL
{
    public class SqlFactory<TDbConnection>(string connectionString, bool isTransaction = false) : DbProviderFactory,
        IDbFactory
        where TDbConnection : DbConnection, new()
    {
        private class Update
        {
            public required int type { get; set; }
            public required string database { get; set; }
            public required string table { get; set; }
            public required long rowid { get; set; }
        }

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
                var currents = new List<Update>();

                //Sqlite only raise database changes on current connection
                raw.sqlite3_commit_hook(sqlite.Handle, (user_data) =>
                {
                    if (isTransaction)
                    {
                        _ = Task.Run(RaiseMonitors);
                    }
                    return 0;

                }, null);

                raw.sqlite3_update_hook(sqlite.Handle, (user_data, type, database, table, rowid) =>
                {
                    currents.Add(new Update()
                    {
                         database = database,
                         type = type,
                         rowid = rowid,
                         table = table                       
                    });

                    if (!isTransaction)
                    {
                        RaiseMonitors();
                    }

                }, null);

                void RaiseMonitors()
                {
                    foreach (var monitor in Monitors.OfType<SqlMonitor>())
                        foreach (var update in currents)
                            monitor.OnSqliteChanged(update.type switch
                            {
                                9 => ChangeType.Delete,
                                18 => ChangeType.Insert,
                                23 => ChangeType.Update,
                                _ => ChangeType.None

                            }, update.database, update.table, update.rowid);
                }
            }

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
