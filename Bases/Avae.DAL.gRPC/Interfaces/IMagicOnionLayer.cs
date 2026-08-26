using MagicOnion;
using System.Data;

namespace Avae.DAL.gRPC;

public interface IMagicOnionLayer : IService<IMagicOnionLayer>
{
    UnaryResult<DBResult> Remove(DBTransactional transactional, string connectionId);

    UnaryResult<DBResult> Save(DBTransactional transactional, string connectionId);

    UnaryResult<DBResult> FindByAnyAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null);
    
    UnaryResult<DBResult> GetAllAsync(string type, int? commandTimeout = null);
    
    UnaryResult<DBResult> GetAsync(string type, long id, int? commandTimeout = null);
    
    UnaryResult<DBResult> WhereAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null);

    UnaryResult<DBResult> QueryAsync(string sql, object? param = null, int? commandTimeout = null, CommandType commandType = CommandType.Text);
}
