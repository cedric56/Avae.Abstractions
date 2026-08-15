using Avae.DAL;
using Avae.MagicServices;
using MagicOnion.Server.Hubs;

namespace Avae.Server;

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
