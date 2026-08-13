using Avae.DAL;
using Avae.MagicServices;
using MessagePack;

namespace Avae.MagicClient
{
    [MessagePackObject]
    public partial class RecordHubReceiver<TObject> : IRecordHubReceiver<TObject> where TObject : class, new()
    {
        // ✅ Add a parameterless constructor for MessagePack
        public RecordHubReceiver() { }
        public RecordHubReceiver(Guid sessionId, IDBMonitor<TObject> monitor)
        {
            Monitor = monitor;
            SessionId = sessionId;
        }

        [Key(0)]
        public Guid? SessionId { get; private set; }

        [IgnoreMember]
        public IDBMonitor<TObject>? Monitor { get; set; }

        public void OnChanged(Record<TObject> record)
        {
            if (record.Connections.Contains(SessionId?.ToString() ?? string.Empty))
                return;

            Monitor?.OnChanged(record);
        }
    }
}
