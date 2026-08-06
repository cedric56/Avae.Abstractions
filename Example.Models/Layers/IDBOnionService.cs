using Avae.DAL;
using Avae.DAL.Interfaces;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;

namespace Example.Models
{
    //[MagicOnionClientGeneration(typeof(IDBOnionService))]
    //public partial class MagicOnionGeneratedClientInitializer { }

    public interface IDBOnionService : IService<IDBOnionService>, IOnionService
    {
        UnaryResult<Result> DbTransRemove(DBModelBase modelBase);

        UnaryResult<Result> DbTransSave(DBModelBase modelBase);
    }
}
