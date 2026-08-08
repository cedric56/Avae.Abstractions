using Avae.DAL.Interfaces;

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
        public event EventHandler<IRecord<TObject>>? OnRecordChanged;

        public void OnChanged(IRecord<TObject> record)
        {
            OnRecordChanged?.Invoke(this, record);
        }

        public override void OnChanged(ChangeType type, string database, string table, long rowid)
        {
            if (table == typeof(TObject).Name)
            {
                OnChanged(new Record<TObject>(rowid, type));
            }
        }
    }
}
