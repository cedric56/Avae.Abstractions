using Dapper;
using System.Data;

namespace Avae.DAL;

public record DBAlias(string alias, string columnName);

public interface IDBLayer
{
    public static Dictionary<Type, string> Sessions { get; set; } = new();

    T? Get<T>(long id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new();

    Task<T?> GetAsync<T>(long id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new();

    IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new();

    Task<IEnumerable<T>> GetAllAsync<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new();

    Task<IEnumerable<T>> FindByAnyAsync<T>(Dictionary<string, object> filters) where T : class, new();

    Task<IEnumerable<T>> FindByAnyAsync<T>(params (string key, object value)[] filters) where T : class, new()
    {
        return FindByAnyAsync<T>(filters.ToDictionary(x => x.key, y => y.value));
    }

    IEnumerable<T> FindByAny<T>(Dictionary<string, object> filters) where T : class, new();

    IEnumerable<T> FindByAny<T>(params (string key, object value)[] filters) where T : class, new()
    {
        return FindByAny<T>(filters.ToDictionary(x => x.key, y => y.value));
    }

    Task<IEnumerable<T>> WhereAsync<T>(Dictionary<string, object> filters) where T : class, new();

    Task<IEnumerable<T>> WhereAsync<T>(params (string key, object value)[] filters) where T : class, new()
    {
        return WhereAsync<T>(filters.ToDictionary(x => x.key, y => y.value));
    }

    IEnumerable<T> Where<T>(Dictionary<string, object> filters) where T : class, new();

    IEnumerable<T> Where<T>(params (string key, object value)[] filters) where T : class, new()
    {
        return Where<T>(filters.ToDictionary(x => x.key, y => y.value));
    }

    IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null);
    IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null);
    IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null);
    IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null);
    IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null);
    IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null);

    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null);
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null);
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null);
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null);
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null);
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null);
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null);
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null);
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null);
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null);
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null);
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null);

    Task<DBResult> Save(DBTransactional transactional);
    Task<DBResult> Remove(DBTransactional transactional);
}
