using Avae.Abstractions;
using MemoryPack;
using MessagePack;

namespace Example.Models
{
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(Person))]
    [MessagePackObject]
    [Union(0, typeof(Person))]
    public abstract partial class DbModelBase
    {
        public abstract Task<Result> DbTransSave(IDataAccessLayer layer);
        public abstract Task<Result> DbTransRemove(IDataAccessLayer layer);
    }
}
