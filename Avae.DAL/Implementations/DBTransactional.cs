using Avae.DAL.Interfaces;
//using MessagePack;

namespace Avae.DAL
{
    //Find a way to union
    //[MessagePackObject]
    //[Union(0, typeof(Person))]
    public abstract partial class DBTransactional
    {
        public abstract Task<DBResult> Save(IDBLayer layer);
        public abstract Task<DBResult> Remove(IDBLayer layer);
    }
}
