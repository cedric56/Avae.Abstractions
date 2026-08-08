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
        public class SqliteFactory : DBFactory<SqliteConnection>
        {
            private readonly string connectionString;
            private readonly bool isTransaction;
            public SqliteFactory(string connectionString, bool isTransaction = true)
                : base(connectionString)
            {
                this.connectionString = connectionString;
                this.isTransaction = isTransaction;
            }

            public override DbConnection? CreateConnection()
            {
                var connection = new SqliteConnection()
                {
                    ConnectionString = connectionString
                };
                connection.Open();

                var records = new List<Record>();

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
                    records.Add(new Record()
                    {
                        database = database,
                        type = type switch
                        {
                            9 => ChangeType.Delete,
                            18 => ChangeType.Insert,
                            23 => ChangeType.Update,
                            _ => ChangeType.None
                        },
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
                    if (isTransaction) Console.WriteLine("Transaction");
                    foreach (var monitor in Monitors.OfType<DBMonitor>())
                        foreach (var record in records.DistinctBy(r => r.rowid))
                        {                            
                            monitor.OnChanged(record.type, record.database, record.table, record.rowid);
                        }
                }

                return connection;
            }
        }

        public static void UseSqliteFactory(this IServiceCollection services,
           string connectionString, Action<SqliteFactory>? action = null, bool isTransaction = true)
        {
            var factory = new SqliteFactory(connectionString, isTransaction);
            action?.Invoke(factory);
            services.AddSingleton<IDBFactory>(sp => factory);
            services.AddTransient<IDbConnection>(_ => factory.CreateConnection()!);
        }
    }
}
