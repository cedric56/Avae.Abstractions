using Avae.DAL;
using Avae.DAL.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using SQLitePCL;
using System.Data;
using System.Data.Common;

namespace Avae.Sqlite
{
    public static class Extensions
    {
        private class Update
        {
            public required int type { get; set; }
            public required string database { get; set; }
            public required string table { get; set; }
            public required long rowid { get; set; }
        }

        public class SqliteFactory(string connectionString, bool isTransaction = true) : SqlFactory<SqliteConnection>(connectionString)
        {
            public override DbConnection? CreateConnection()
            {
                var connection = new SqliteConnection()
                {
                    ConnectionString = connectionString
                };
                connection.Open();

                var currents = new List<Update>();

                //Sqlite only raise database changes on current connection
                raw.sqlite3_commit_hook(connection.Handle, (user_data) =>
                {
                    if (isTransaction)
                    {
                        _ = Task.Run(RaiseMonitors);
                    }
                    return 0;

                }, null);

                raw.sqlite3_update_hook(connection.Handle, (user_data, type, database, table, rowid) =>
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

                return connection;
            }
        }

        public static void UseSqliteFactory(this IServiceCollection services,
           string connectionString, Action<SqliteFactory>? action = null, bool isTransaction = false)
        {
            var factory = new SqliteFactory(connectionString, isTransaction);

            action?.Invoke(factory);
            services.AddSingleton<IDBFactory>(sp => factory);
            services.AddTransient<IDbConnection>(_ => factory.CreateConnection()!);
        }
    }
}
