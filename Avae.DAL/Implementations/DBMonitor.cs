namespace Avae.DAL
{
    public abstract class DBMonitor : IDBMonitor
    {
        public abstract void OnChanged(ChangeType type, string database, string table, long rowid);
    }

    public class DBMonitor<TObject> :
        DBMonitor,
        ISqlMonitor<TObject>
        where TObject : class, new()
    {
        public event EventHandler<Record<TObject>>? OnRecordChanged;

        public void OnChanged(Record<TObject> record)
        {
            OnRecordChanged?.Invoke(this, record);
        }

        public override void OnChanged(ChangeType type, string database, string table, long rowid)
        {
            if (table == typeof(TObject).Name)
            {
                var record = new Record<TObject>(rowid, type, []);
                Console.WriteLine(record);
                OnChanged(record);
            }
        }
    }
}
