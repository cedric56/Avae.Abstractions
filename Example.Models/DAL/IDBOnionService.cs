using Avae.DAL;
using MagicOnion;

namespace Example.Models
{
    public interface IDBOnionService : IService<IDBOnionService>, IOnionService
    {
        UnaryResult<Result> DbTransRemove(DBModelBase modelBase);

        UnaryResult<Result> DbTransSave(DBModelBase modelBase);
    }
}
