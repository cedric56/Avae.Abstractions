using Avae.DAL.Interfaces;
//using MessagePack;

namespace Avae.DAL
{
    //Find a way to union
    //[MessagePackObject]
    //[Union(0, typeof(Person))]
    public abstract partial class DBModelBase
    {
        public abstract Task<Result> DbTransSave(IDBLayer layer);
        public abstract Task<Result> DbTransRemove(IDBLayer layer);
    }
}
