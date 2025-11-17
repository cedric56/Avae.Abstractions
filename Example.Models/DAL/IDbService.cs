using Avae.DAL;
using MagicOnion;

namespace Example.Models
{
    public interface IDbService : IService<IDbService>, IOnionService
    {
        UnaryResult<Result> DbTransRemove(DbModelBase modelBase);

        UnaryResult<Result> DbTransSave(DbModelBase modelBase);
    }
}
