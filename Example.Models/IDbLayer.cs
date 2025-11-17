using Avae.Abstractions;
using Avae.DAL;
using MemoryPack;
using MessagePack;

namespace Example.Models
{
    public interface IDbLayer : IDataAccessLayer
    {
        Task<Result> DbTransSave(DbModelBase modelBase);
        Task<Result> DbTransRemove(DbModelBase modelBase);
    }

    [MemoryPackable]
    [MemoryPackUnion(0, typeof(Person))]
    //[MemoryPackUnion(1, typeof(Contact))]
    [MessagePack.MessagePackObject]
    [Union(0, typeof(Person))]
    public abstract partial class DbModelBase// : IModelBase
    {
        public abstract Task<Result> DbTransSave(IDataAccessLayer layer);
        public abstract Task<Result> DbTransRemove(IDataAccessLayer layer);
    }

    public class DBSqlLayer : SqlLayer, IDbLayer
    {
        public Task<Result> DbTransRemove(DbModelBase modelBase)
        {
            return modelBase.DbTransRemove(this);
        }

        public Task<Result> DbTransSave(DbModelBase modelBase)
        {
            return modelBase.DbTransSave(this);
        }
    }

    public class DBOnionLayer : OnionLayer, IDbLayer
    {
        public IDbService Service = SimpleProvider.GetService<IOnionService>() as IDbService;

        public async Task<Result> DbTransRemove(DbModelBase modelBase)
        {
            return await Service.DbTransRemove(modelBase);
        }

        public async Task<Result> DbTransSave(DbModelBase modelBase)
        {
            return await Service.DbTransSave(modelBase);
        }
    }
}
