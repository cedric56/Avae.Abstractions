using Avae.DAL.Interfaces;

namespace Avae.DAL
{
    public abstract class SqlMonitor : ISqlMonitor
    {
        public abstract void OnSqliteChanged(ChangeType type, string database, string table, long rowid);
    }

    public class SqlMonitor<TObject> :
        SqlMonitor,
        ISqlMonitor<TObject>
        where TObject : class, new()
    {
        public event EventHandler<IRecord<TObject>>? OnChanged;

        public void Changed(IRecord<TObject> record)
        {
            OnChanged?.Invoke(this, record);
        }

        public override void OnSqliteChanged(ChangeType type, string database, string table, long rowid)
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
