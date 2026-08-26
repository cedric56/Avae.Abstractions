using Avae.Core;

namespace Avae.DAL;

public class DBBase
{
    private static readonly object _lock = new();
    private static IDBLayer? _instance;
    public static IDBLayer Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= ServiceLocator.GetRequiredService<IDBLayer>();
                }
            }
            return _instance;
        }
    }


    //IEnumerable<dynamic> Query(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
    //dynamic QueryFirst(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    //dynamic QueryFirstOrDefault(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    //dynamic QuerySingle(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    //dynamic QuerySingleOrDefault(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    //IEnumerable<T> Query<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
    //T QueryFirst<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    //T QueryFirstOrDefault<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    //T QuerySingle<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    //T QuerySingleOrDefault<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    //IEnumerable<object> Query(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
    //object QueryFirst(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    //object QueryFirstOrDefault(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    //object QuerySingle(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    //object QuerySingleOrDefault(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    //IEnumerable<T> Query<T>(this IDbConnection cnn, CommandDefinition command); T QueryFirst<T>(this IDbConnection cnn, CommandDefinition command); T QueryFirstOrDefault<T>(this IDbConnection cnn, CommandDefinition command);
    //T QuerySingle<T>(this IDbConnection cnn, CommandDefinition command);
    //T QuerySingleOrDefault<T>(this IDbConnection cnn, CommandDefinition command);
    //GridReader QueryMultiple(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    //GridReader QueryMultiple(this IDbConnection cnn, CommandDefinition command);



}
