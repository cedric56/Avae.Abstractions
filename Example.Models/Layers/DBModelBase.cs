using Avae.DAL;
using Avae.DAL.Interfaces;
using MessagePack;

namespace Example.Models
{
    [MessagePackObject]
    [Union(0, typeof(Person))]
    public abstract partial class DBModelBase
    {
        public abstract Task<Result> DbTransSave(IDataAccessLayer layer);        
        public abstract Task<Result> DbTransRemove(IDataAccessLayer layer);
    }
}
