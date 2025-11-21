using Avae.Abstractions;
using Microsoft.Data.SqlClient;

namespace Avae.DAL
{
    public abstract class SqlMonitor : IDbMonitor
    {
        public abstract void OnSqliteChanged(object user_data, int type, string database, string table, long rowid);
    }

    public class SqlMonitor<TObject> :
        SqlMonitor,
        ISqlMonitorService<TObject>,
        IDisposable
        where TObject : class, new()
    {
        private IDataAccessLayer Layer = SimpleProvider.GetService<IDataAccessLayer>();
        private SignalRService? signalRService;
        private SqlDependencyExService<TObject>? sqlDependencyExService;

        public async Task AddSignalR(string url)
        {
            signalRService = new SignalRService(url);
            signalRService.On<Record<TObject>>(SqlHub<TObject>.Message, record =>
            {
                OnChanged?.Invoke(this, record);
            });
            await signalRService.StartAsync();
        }

        public SqlMonitor(string connectionString, Type connectionType)
        {
            if (connectionType == typeof(SqlConnection))
            {
                sqlDependencyExService = new SqlDependencyExService<TObject>(connectionString);
                sqlDependencyExService.OnChanged += (sender, e) =>
                {
                    var record = new Record<TObject>(e.Entity, e.ChangeType, e.EntityOldValues);
                    OnChanged?.Invoke(this, record);
                    //Inside client, we notify multiprocess
                    signalRService?.SendAsync(SqlHub<TObject>.Message, record);
                    //Inside server, we notify clients
                    OnDbChanged(record);
                };
                sqlDependencyExService.Start();
            }
        }

        public event EventHandler<IRecord<TObject>>? OnChanged;

        public override void OnSqliteChanged(object user_data, int type, string database, string table, long rowid)
        {
            if (table == typeof(TObject).Name)
            {
                var obj = Layer.Get<TObject>(rowid);
                var record = new Record<TObject>(obj, type switch
                {
                    9 => ChangeType.Delete,
                    18 => ChangeType.Insert,
                    23 => ChangeType.Update,
                    _ => ChangeType.None
                });
                OnChanged?.Invoke(this, record);
                //Inside client, we notify multiprocess
                signalRService?.SendAsync(SqlHub<TObject>.Message, record);
                //Inside server, we notify clients
                OnDbChanged(record);
            }
        }

        private void OnDbChanged(Record<TObject> record)
        {
            Task.Run(async () =>
            {
                var hub = SimpleProvider.GetService<SqlHub<TObject>>();
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
            sqlDependencyExService?.Stop();
            sqlDependencyExService?.Dispose();
        }
    }

    public class Record<T> : IRecord<T> where T : class, new()
    {
        public Record()
        {
            Entity = null!;
            ChangeType = ChangeType.None;
        }

        public Record(T entity, ChangeType changeType, T? oldEntity = null)
        {
            Entity = entity;
            ChangeType = changeType;
            EntityOldValues = oldEntity;
        }

        public T Entity { get; set; }
        public ChangeType ChangeType { get; set; }
        public T? EntityOldValues { get; set; }
    }
}
