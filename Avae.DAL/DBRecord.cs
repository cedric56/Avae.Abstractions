using Avae.DAL.Interfaces;

namespace Avae.DAL
{
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
