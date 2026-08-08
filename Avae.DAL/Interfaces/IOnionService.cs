using MagicOnion;

namespace Avae.DAL.Interfaces
{
    public interface IOnionService : IService<IOnionService>
    {
        UnaryResult<Result> DbTransRemove(DBModelBase modelBase);

        UnaryResult<Result> DbTransSave(DBModelBase modelBase);

        UnaryResult<Result> FindByAnyAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null);
        
        UnaryResult<Result> GetAllAsync(string type, int? commandTimeout = null);
        
        UnaryResult<Result> GetAsync(string type, long id, int? commandTimeout = null);
        
        UnaryResult<Result> WhereAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null);        
    }
}
