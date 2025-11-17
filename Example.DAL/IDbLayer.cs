using Avae.Abstractions;
using Avae.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.DAL
{
    //public interface IDbLayer : IDataAccessLayer
    //{
    //    Task<Result> DbTransSave(IDbModelBase modelBase);
    //    Task<Result> DbTransRemove(IDbModelBase modelBase);
    //}

    //[MemoryPack.MemoryPackable]
    //public interface IDbModelBase : IModelBase
    //{
    //    Task<Result> DbTransSave(IDataAccessLayer layer);
    //    Task<Result> DbTransRemove(IDataAccessLayer layer);
    //}

    //public class DBSqlLayer : SqlLayer, IDbLayer
    //{
    //    public Task<Result> DbTransRemove(IDbModelBase modelBase)
    //    {
    //        return modelBase.DbTransRemove(this);
    //    }

    //    public Task<Result> DbTransSave(IDbModelBase modelBase)
    //    {
    //        return modelBase.DbTransSave(this);
    //    }
    //}

    //public class DBOnionLayer : OnionLayer, IDbLayer
    //{
    //    public IDbLayer Service = SimpleProvider.GetService<IDbLayer>();

    //    public Task<Result> DbTransRemove(IDbModelBase modelBase)
    //    {
    //        return Service.DbTransRemove(modelBase);
    //    }

    //    public Task<Result> DbTransSave(IDbModelBase modelBase)
    //    {
    //        return Service.DbTransSave(modelBase);
    //    }
    //}
}
