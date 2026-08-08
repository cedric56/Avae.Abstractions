using Avae.DAL.Interfaces;

namespace Avae.DAL
{
    public abstract class DBMonitor : IDBMonitor
    {
        public abstract void Changed(ChangeType type, string database, string table, long rowid);
    }

    public class DBMonitor<TObject> :
        DBMonitor,
        ISqlMonitor<TObject>
        where TObject : class, new()
    {
        public event EventHandler<IRecord<TObject>>? OnChanged;

        public void Changed(IRecord<TObject> record)
        {
            OnChanged?.Invoke(this, record);
        }

        public override void Changed(ChangeType type, string database, string table, long rowid)
        {
            if (table == typeof(TObject).Name)
            {
                Changed(new Record<TObject>(rowid, type));
            }
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

        public List<string> ConnectionId { get; set; } = [];
    }
}
