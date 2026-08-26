using Dapper;
using Dapper.Contrib.Extensions;
using MessagePack;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Avae.DAL;

public class DBLayer(IServiceProvider provider) : IDBLayer
{
    public Task<DBResult> Remove(DBTransactional transactional)
    {
        return transactional.Remove(this);
    }

    public Task<DBResult> Save(DBTransactional transactional)
    {
        return transactional.Save(this);
    }

    public T? Get<T>(long id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
    {
        using var db = new DBLogConnection(provider);
        return db.Get<T>(id, transaction, commandTimeout);
    }

    public IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
    {
        using var db = new DBLogConnection(provider);
        return db.GetAll<T>(transaction, commandTimeout);
    }

    public Task<IEnumerable<T>> GetAllAsync<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
    {
        using var db = new DBLogConnection(provider);
        return db.GetAllAsync<T>(transaction, commandTimeout);
    }

    public async Task<T?> GetAsync<T>(long id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
    {
        using var db = new DBLogConnection(provider);
        return await db.GetAsync<T>(id, transaction, commandTimeout);
    }

    private static readonly ConcurrentDictionary<Type, string> _columnCache = new();


    private static List<PropertyInfo> GetMappedProperties<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>()
    {
        return typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite)
            .Where(p => p.GetCustomAttribute<ComputedAttribute>() == null)
            .Where(p => p.GetCustomAttribute<IgnoreMemberAttribute>() == null)
            .ToList();
    }

    private static string GetColumns<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>()
    {
        return _columnCache.GetOrAdd(typeof(T), _ =>
            string.Join(", ", GetMappedProperties<T>().Select(p => p.Name)));
    }

    private static string Create<T>(Dictionary<string, object> filters, string condition, out DynamicParameters parameters)
    {
        var conditions = new List<string>();
        parameters = new DynamicParameters();

        foreach (var pair in filters)
        {
            parameters.Add(pair.Key, pair.Value);
            conditions.Add($"(@{pair.Key} IS NOT NULL AND {pair.Key} = @{pair.Key})");
        }

        string where = string.Join(condition, conditions);
        string columns = GetColumns<T>();

        return $"SELECT {columns} FROM {typeof(T).Name} WHERE {where}";
    }

    public Task<IEnumerable<T>> FindByAnyAsync<T>(Dictionary<string, object> filters) where T : class, new()
    {
        var sql = Create<T>(filters, " OR ", out var parameters);
        using var db = new DBLogConnection(provider);
        return db.QueryAsync<T>(sql, parameters);
    }

    public IEnumerable<T> FindByAny<T>(Dictionary<string, object> filters) where T : class, new()
    {
        var sql = Create<T>(filters, " OR ", out var parameters);
        using var db = new DBLogConnection(provider);
        return db.Query<T>(sql, parameters);
    }

    public Task<IEnumerable<T>> WhereAsync<T>(Dictionary<string, object> filters) where T : class, new()
    {
        var sql = Create<T>(filters, " AND ", out var parameters);
        using var db = new DBLogConnection(provider);
        return db.QueryAsync<T>(sql, parameters);
    }

    public IEnumerable<T> Where<T>(Dictionary<string, object> filters) where T : class, new()
    {
        var sql = Create<T>(filters, " AND ", out var parameters);
        using var db = new DBLogConnection(provider);
        return db.Query<T>(sql, parameters);
    }

    public IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TReturn> map, string splitOn = "Id")
    {
        using var db = new DBLogConnection(provider);
        return db.QueryAsync(command, map, splitOn);
    }

    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.QueryAsync(command, map, splitOn);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.QueryAsync(command, map, splitOn);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.QueryAsync(command, map, splitOn);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.QueryAsync(command, map, splitOn);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.QueryAsync(command, map, splitOn);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null)
    {
        using var db = new DBLogConnection(provider);
        return db.QueryAsync(command, map, splitOn);
    }
}
