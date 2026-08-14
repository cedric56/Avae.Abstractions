using Avae.DAL;
using Avae.MagicServices;
using MagicOnion.Server.Hubs;

namespace Avae.Server
{
    public class RecordHubRepository<TObject> where TObject : class, new()
    {
        readonly Dictionary<Guid, byte> customerIds = new(); // just tracking membership, group itself is shared
        IGroup<IRecordHubReceiver<TObject>>? group;
        readonly object gate = new();

        public RecordHubRepository(IDBMonitor<TObject> monitor)
        {
            IDBFactory.Monitors.Add(monitor);
            monitor.OnRecordChanged += (_, e) => Raise(e); // subscribed exactly ONCE, for the app lifetime
        }

        // Called by each connecting hub instance; captures the shared group once.
        public void RegisterGroup(IGroup<IRecordHubReceiver<TObject>> g, Guid contextId)
        {
            lock (gate)
            {
                group ??= g; // same underlying group object every time, but only need to capture it once
                customerIds[contextId] = 0;
            }
        }

        public void Unregister(Guid contextId)
        {
            lock (gate) customerIds.Remove(contextId);
        }

        public void Raise(Record<TObject> e)
        {
            var ids = (e.Connections ?? []).Select(id => new Guid(id)).ToList();
            group?.Except(ids).OnChanged(e);
        }
    }

    public class MagicOnionHub<TObject> :
     StreamingHubBase<IRecordHub<TObject>, IRecordHubReceiver<TObject>>,
     IRecordHub<TObject>, IDisposable where TObject : class, new()
    {
        readonly RecordHubRepository<TObject> repository;

        public MagicOnionHub(RecordHubRepository<TObject> repository)
        {
            this.repository = repository; // no monitor subscription here anymore
        }

        public async Task<Guid> AddReceiverAsync()
        {
            var group = await Group.AddAsync("customers");
            repository.RegisterGroup(group, this.Context.ContextId);
            return this.Context.ContextId;
        }

        public Task RemoveAsync()
        {
            repository.Unregister(this.Context.ContextId);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // nothing to unsubscribe here anymore — repository owns the monitor subscription for the app's lifetime
        }

        public void OnRecordChanged(Record<TObject> e)
        {
            repository.Raise(e);
        }
    }
}
