using Avae.DAL;
using Avae.DAL.gRPC;
using MagicOnion.Server.Hubs;

namespace Avae.Server;

public class RecordHub<TObject> :
 StreamingHubBase<IRecordHub<TObject>, IRecordHubReceiver<TObject>>,
 IRecordHub<TObject> where TObject : class, new()
{
    readonly RecordHubRepository<TObject> repository;

    public RecordHub(RecordHubRepository<TObject> repository)
    {
        this.repository = repository;
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

    public void OnRecordChanged(Record<TObject> e)
    {
        repository.Raise(e);
    }
}
