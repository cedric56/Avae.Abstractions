namespace Avae.DAL;

public abstract partial class DBTransactional
{
    //public Guid SessionId { get; } = IDBLayer.SessionId;

    public abstract Task<DBResult> Save(IDBLayer layer);
    public abstract Task<DBResult> Remove(IDBLayer layer);
}
