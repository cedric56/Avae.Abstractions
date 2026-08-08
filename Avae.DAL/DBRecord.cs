namespace Avae.DAL
{
    public enum ChangeType
    {
        None,
        Delete,
        Insert,
        Update
    }

    public class Record<T> where T : class, new()
    {
        public Record()
        {
            ChangeType = ChangeType.None;
            Connections = [];
        }

        public Record(long rowId, ChangeType changeType, List<string> connections)
        {
            RowId = rowId;
            ChangeType = changeType;
            Connections = connections;
        }

        public long RowId { get; set; }

        public ChangeType ChangeType { get; set; }

        public IList<string> Connections { get; set; }

        public override string ToString()
        {
            return $"{typeof(T).Name} : {RowId} {ChangeType} {string.Join(",", Connections)}";
        }
    }
}
