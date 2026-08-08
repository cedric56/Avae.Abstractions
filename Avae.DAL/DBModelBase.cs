using Avae.DAL.Interfaces;
//using MessagePack;

namespace Avae.DAL
{
    //Find a way to union
    //[MessagePackObject]
    //[Union(0, typeof(Person))]
    public abstract partial class DBModelBase
    {
        public abstract Task<Result> DbTransSave(IDataAccessLayer layer);
        public abstract Task<Result> DbTransRemove(IDataAccessLayer layer);
    }
}
