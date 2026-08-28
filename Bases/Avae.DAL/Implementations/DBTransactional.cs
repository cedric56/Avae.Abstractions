namespace Avae.DAL;

public abstract partial class DBTransactional
{
    public abstract Task<DBResult> Save(IDBLayer layer, int? commandTimeout = null);
    public abstract Task<DBResult> Remove(IDBLayer layer, int? commandTimeout = null);
}
