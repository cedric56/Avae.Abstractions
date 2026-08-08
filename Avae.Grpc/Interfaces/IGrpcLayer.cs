using Avae.DAL;
using MagicOnion;

namespace Avae.Grpc
{
    public interface IGrpcLayer : IService<IGrpcLayer>
    {
        UnaryResult<DBResult> Remove(DBTransactional transactional);

        UnaryResult<DBResult> Save(DBTransactional transactional);

        UnaryResult<DBResult> FindByAnyAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null);
        
        UnaryResult<DBResult> GetAllAsync(string type, int? commandTimeout = null);
        
        UnaryResult<DBResult> GetAsync(string type, long id, int? commandTimeout = null);
        
        UnaryResult<DBResult> WhereAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null);        
    }
}
