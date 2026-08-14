namespace Avae.DAL
{
    public abstract class DBMonitor : IDBMonitor
    {
        public bool IsRunning { get; set; }

        public abstract void OnChanged(ChangeType type, string database, string table, long rowid, Guid? sessionID);

        public Func<Task> Restart { get; set; } = new Func<Task>(() => Task.CompletedTask);
    }

    public class DBMonitor<TObject> :
        DBMonitor,
        IDBMonitor<TObject>
        where TObject : class, new()
    {
        public event EventHandler<Record<TObject>>? OnRecordChanged;

        public void OnChanged(Record<TObject> record)
        {
            OnRecordChanged?.Invoke(this, record);
        }

        public override void OnChanged(ChangeType type, string database, string table, long rowid, Guid? sessionID)
        {
            if (table == typeof(TObject).Name)
            {
                var record = new Record<TObject>(rowid, type, sessionID.HasValue ? [sessionID.Value.ToString()] : []);
                OnChanged(record);
            }
        }
    }
}
