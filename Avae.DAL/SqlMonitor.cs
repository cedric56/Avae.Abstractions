using Avae.Abstractions;
using Avae.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using TableDependencyCore.SqlClient;
using TableDependencyCore.SqlClient.Base.EventArgs;

namespace Avae.DAL
{
    public abstract class SqlMonitor : ISqlMonitor
    {
        public abstract void OnSqliteChanged(ChangeType type, string database, string table, long rowid);
    }

    public class SqlMonitor<TObject> :
        SqlMonitor,
        ISqlMonitor<TObject>,
        IDisposable
        where TObject : class, new()
    {
        IServiceProvider provider;
        private SignalRService? signalRService;
        private readonly SqlTableDependencyCore<TObject>? sqlDependency;

        public void AddSignalR(string url)
        {
            signalRService = new SignalRService(url);
            signalRService.On<Record<TObject>>(SqlHub<TObject>.Message, record =>
            {
                OnChanged?.Invoke(this, record);
            });
            Task.Run(async () =>
            {
                try
                {
                    await signalRService.StartAsync();
                }
                catch (Exception ex) {
                    Debug.WriteLine(ex);
                }
            });
        }

        public SqlMonitor(IServiceProvider provider)
        {
            this.provider = provider;
        }

        internal SqlMonitor(IServiceProvider provider, string connectionString, Type connectionType)
        {
            this.provider = provider;
            if (connectionType == typeof(SqlConnection))
            {
                sqlDependency = new SqlTableDependencyCore<TObject>(connectionString);
                sqlDependency.OnChanged += SqlDependencyExService_OnChanged;
                sqlDependency.Start();
            }
        }

        private void SqlDependencyExService_OnChanged(object? sender, RecordChangedEventArgs<TObject> e)
        {
            if (e.Entity is IModelBase model)
            {
                var record = new Record<TObject>(model.Id, Enum.Parse<ChangeType>(e.ChangeType.ToString()));
                OnChanged?.Invoke(this, record);
                //Inside client, we notify multiprocess
                signalRService?.SendAsync(SqlHub<TObject>.Message, record);
                //Inside server, we notify clients
                RaiseHub(record);
            }
        }

        public event EventHandler<IRecord<TObject>>? OnChanged;

        public override void OnSqliteChanged(ChangeType type, string database, string table, long rowid)
        {
            if (table == typeof(TObject).Name)
            {
                var record = new Record<TObject>(rowid, type);
                OnChanged?.Invoke(this, record);
                //Inside client, we notify multiprocess
                signalRService?.SendAsync(SqlHub<TObject>.Message, record);
                //Inside server, we notify clients
                RaiseHub(record);
            }
        }

        private void RaiseHub(Record<TObject> record)
        {
            Task.Run(async () =>
            {
                var hub = provider.GetService<SqlHub<TObject>>();
                if (hub is not null)
                    await hub.SendMessage(record);
            });
        }

        public void Dispose()
        {
            if (signalRService is not null)
            {
                Task.Run(async () =>
                {
                    await signalRService.StopAsync();
                    await signalRService.DisposeAsync();
                });
            }
            if (sqlDependency is not null)
            {
                sqlDependency.OnChanged -= SqlDependencyExService_OnChanged;
                sqlDependency.Stop();
                sqlDependency.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }

    public class Record<T> : IRecord<T> where T : class, new()
    {
        public Record()
        {
            ChangeType = ChangeType.None;
        }

        public Record(long rowId, ChangeType changeType)
        {
            RowId = rowId;
            ChangeType = changeType;
        }

        public long RowId { get; set; }
        public ChangeType ChangeType { get; set; }
    }
}
