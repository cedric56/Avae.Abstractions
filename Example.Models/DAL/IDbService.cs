using Avae.DAL;
using MagicOnion;

namespace Example.Models
{
    public interface IDbService : IService<IDbService>, 
        IOnionService
    {
        UnaryResult<Result> DbTransRemove(DBModelBase modelBase);

        UnaryResult<Result> DbTransSave(DBModelBase modelBase);
    }
}
