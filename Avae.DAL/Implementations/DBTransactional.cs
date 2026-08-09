using Avae.DAL.Interfaces;

namespace Avae.DAL;
public abstract partial class DBTransactional
{
    public abstract Task<DBResult> Save(IDBLayer layer);
    public abstract Task<DBResult> Remove(IDBLayer layer);
}
