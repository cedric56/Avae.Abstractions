using Avae.Core;
using Dapper;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Avae.DAL.gRPC;

public partial class MagicOnionLayer(IServiceProvider provider, string url, int globalCommandTimeout, ILogger? logger = null) : IDBLayer
{
    public async Task<DBResult> Remove(DBTransactional transactional, int? commandTimeout = null)
    {
        try
        {
            using var tcs = new CancellationTokenSource(globalCommandTimeout);
            IDBLayer.Sessions.TryGetValue(transactional.GetType(), out var connectionId);
            var service = provider.GetRequiredService<IMagicOnionLayer>();
            return await service
                .WithCancellationToken(tcs.Token)
                .Remove(transactional, connectionId ?? string.Empty, commandTimeout)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return new DBResult()
            {
                Successful = false,
                Exception = ex.Message
            };
        }
    }

    public async Task<DBResult> Save(DBTransactional transactional, int? commandTimeout = null)
    {
        try
        {
            using var tcs = new CancellationTokenSource(globalCommandTimeout);
            IDBLayer.Sessions.TryGetValue(transactional.GetType(), out var connectionId);
            var service = provider.GetRequiredService<IMagicOnionLayer>();
            return await service
                .WithCancellationToken(tcs.Token)
                .Save(transactional, connectionId ?? string.Empty, commandTimeout)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return new DBResult()
            {
                Successful = false,
                Exception = ex.Message
            };
        }
    }

    public IEnumerable<T> FindByAny<T>(Dictionary<string, object> filters, int? commandTimeout = null) where T : class, new()
    {
        try
        {
            if (OperatingSystem.IsBrowser())
            {
                logger?.LogWarning("FindByAny<T> is not recommended for use in WebAssembly due to potential performance issues. Consider using FindByAnyAsync<T> instead.");
                var request = provider.GetRequiredService<IXmlHttpRequest>();
                var result = request.Send(url, nameof(FindByAnyAsync), MessagePackSerializer.Serialize(new object[] { typeof(T).Name, filters, commandTimeout ?? int.MaxValue }), globalCommandTimeout);
                if (result == Array.Empty<byte>()) return [];
                return MessagePackSerializer.Deserialize<IEnumerable<T>>(result) ?? [];
            }
            return AsyncHelper.RunSync(() => FindByAnyAsync<T>(filters, commandTimeout));
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return [];
        }
    }

    public async Task<IEnumerable<T>> FindByAnyAsync<T>(Dictionary<string, object> filters, int? commandTimeout = null) where T : class, new()
    {
        try
        {
            using var tcs = new CancellationTokenSource(globalCommandTimeout);
            var service = provider.GetRequiredService<IMagicOnionLayer>();
            var result = await service
                .WithCancellationToken(tcs.Token)
                .FindByAnyAsync(typeof(T).Name, filters, commandTimeout)
                .ConfigureAwait(false);
            if (!result.Successful) throw new Exception(result.Exception);
            if (result.Data == Array.Empty<byte>()) return [];
            return MessagePackSerializer.Deserialize<IEnumerable<T>>(result.Data);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return [];
        }
    }

    public T? Get<T>(long id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
    {
        try
        {
            if (OperatingSystem.IsBrowser())
            {
                logger?.LogWarning("Get is not recommended for use in WebAssembly due to potential performance issues. Use GetAsync instead.");
                var request = provider.GetRequiredService<IXmlHttpRequest>();
                var result = request.Send(url, nameof(GetAsync), MessagePackSerializer.Serialize(new object[] { typeof(T).Name, id, commandTimeout ?? int.MaxValue }), globalCommandTimeout);
                if (result == Array.Empty<byte>()) return null;
                return MessagePackSerializer.Deserialize<T>(result);
            }
            return AsyncHelper.RunSync(() => GetAsync<T>(id, transaction, commandTimeout));
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return null;
        }
    }

    public IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
    {
        try
        {
            if (OperatingSystem.IsBrowser())
            {
                logger?.LogWarning("GetAll<T> is not recommended for use in WebAssembly due to potential performance issues. Consider using FindByAny<T> with appropriate filters instead.");
                var request = provider.GetRequiredService<IXmlHttpRequest>();
                var result = request.Send(url, nameof(GetAllAsync), MessagePackSerializer.Serialize(new object[] { typeof(T).Name, commandTimeout ?? int.MaxValue }), globalCommandTimeout);
                if (result == Array.Empty<byte>()) return [];
                return MessagePackSerializer.Deserialize<IEnumerable<T>>(result) ?? [];
            }
            return AsyncHelper.RunSync(() => GetAllAsync<T>(transaction, commandTimeout));
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return [];
        }
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
    {
        try
        {
            using var tcs = new CancellationTokenSource(globalCommandTimeout);
            var service = provider.GetRequiredService<IMagicOnionLayer>();
            var result = await service
                .WithCancellationToken(tcs.Token)
                .GetAllAsync(typeof(T).Name, commandTimeout)
                .ConfigureAwait(false);
            if (!result.Successful) throw new Exception(result.Exception);
            if (result.Data == Array.Empty<byte>()) return [];
            return MessagePackSerializer.Deserialize<IEnumerable<T>>(result.Data) ?? [];
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return [];
        }
    }

    public async Task<T?> GetAsync<T>(long id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
    {
        try
        {
            using var tcs = new CancellationTokenSource(globalCommandTimeout);
            var service = provider.GetRequiredService<IMagicOnionLayer>();
            var result = await service
                .WithCancellationToken(tcs.Token)
                .GetAsync(typeof(T).Name, id, commandTimeout)
                .ConfigureAwait(false);
            if (!result.Successful) throw new Exception(result.Exception);
            if (result.Data == Array.Empty<byte>()) return null;
            return MessagePackSerializer.Deserialize<T>(result.Data);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return null;
        }
    }

    public IEnumerable<T> Where<T>(Dictionary<string, object> filters, int? commandTimeout = null) where T : class, new()
    {
        try
        {
            if (OperatingSystem.IsBrowser())
            {
                logger?.LogWarning("Where<T> is not recommended for use in WebAssembly due to potential performance issues. Consider using WhereAsync<T> instead.");
                var request = provider.GetRequiredService<IXmlHttpRequest>();
                var result = request.Send(url, nameof(WhereAsync), MessagePackSerializer.Serialize(new object[] { typeof(T).Name, filters, commandTimeout ?? int.MaxValue }), globalCommandTimeout);
                if (result == Array.Empty<byte>()) return [];
                return MessagePackSerializer.Deserialize<IEnumerable<T>>(result) ?? [];
            }
            return AsyncHelper.RunSync(() => WhereAsync<T>(filters, commandTimeout));
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return [];
        }
    }

    public async Task<IEnumerable<T>> WhereAsync<T>(Dictionary<string, object> filters, int? commandTimeout = null) where T : class, new()
    {
        try
        {
            using var tcs = new CancellationTokenSource(globalCommandTimeout);
            var service = provider.GetRequiredService<IMagicOnionLayer>();
            var result = await service
                .WithCancellationToken(tcs.Token)
                .WhereAsync(typeof(T).Name, filters, commandTimeout)
                .ConfigureAwait(false);
            if (!result.Successful) throw new Exception(result.Exception);
            if (result.Data == Array.Empty<byte>()) return [];
            return MessagePackSerializer.Deserialize<IEnumerable<T>>(result.Data) ?? [];
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return [];
        }
    }

    public IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        try
        {
            if (OperatingSystem.IsBrowser())
            {
                logger?.LogWarning("Query<TFirst, TSecond, TReturn> is not recommended for use in WebAssembly due to potential performance issues. Consider using QueryAsync<TFirst, TSecond, TReturn> instead.");
                var request = provider.GetRequiredService<IXmlHttpRequest>();
                var result = request.Send(url, nameof(QueryAsync), MessagePackSerializer.Serialize(new object[] { sql, param ?? new object(), commandTimeout ?? int.MaxValue, commandType ?? CommandType.Text }), globalCommandTimeout);
                if (result == Array.Empty<byte>()) return [];
                var rows = MessagePackSerializer.Deserialize<IEnumerable<IDictionary<string, object>>>(result) ?? [];
                return rows.Select(row => MapRow(row, map, splitOn, aliases)).ToList();
            }
            return AsyncHelper.RunSync(() => QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType, aliases));
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return [];
        }
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null)
    {
        return QueryAsync(command.CommandText, map, command.Parameters, command.Transaction, command.Buffered, splitOn, command.CommandTimeout, command.CommandType, aliases);
    }

    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        try
        {
            if (OperatingSystem.IsBrowser())
            {
                logger?.LogWarning("Query<TFirst, TSecond, TThird, TReturn> is not recommended for use in WebAssembly due to potential performance issues. Consider using QueryAsync<TFirst, TSecond, TThird, TReturn> instead.");
                var request = provider.GetRequiredService<IXmlHttpRequest>();
                var result = request.Send(url, nameof(QueryAsync), MessagePackSerializer.Serialize(new object[] { sql, param ?? new object(), commandTimeout ?? int.MaxValue, commandType ?? CommandType.Text }), globalCommandTimeout);
                if (result == Array.Empty<byte>()) return [];
                var rows = MessagePackSerializer.Deserialize<IEnumerable<IDictionary<string, object>>>(result) ?? [];
                return rows.Select(row => MapRow(row, map, splitOn, aliases)).ToList();
            }
            return AsyncHelper.RunSync(() => QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType, aliases));
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return [];
        }
    }

    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        try
        {
            if (OperatingSystem.IsBrowser())
            {
                logger?.LogWarning("Query<TFirst, TSecond, TThird, TFourth, TReturn> is not recommended for use in WebAssembly due to potential performance issues. Consider using QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn> instead.");
                var request = provider.GetRequiredService<IXmlHttpRequest>();
                var result = request.Send(url, nameof(QueryAsync), MessagePackSerializer.Serialize(new object[] { sql, param ?? new object(), commandTimeout ?? int.MaxValue, commandType ?? CommandType.Text }), globalCommandTimeout);
                if (result == Array.Empty<byte>()) return [];
                var rows = MessagePackSerializer.Deserialize<IEnumerable<IDictionary<string, object>>>(result) ?? [];
                return rows.Select(row => MapRow(row, map, splitOn, aliases)).ToList();
            }
            return AsyncHelper.RunSync(() => QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType, aliases));
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return [];
        }
    }

    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        try
        {
            if (OperatingSystem.IsBrowser())
            {
                logger?.LogWarning("Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> is not recommended for use in WebAssembly due to potential performance issues. Consider using QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> instead.");
                var request = provider.GetRequiredService<IXmlHttpRequest>();
                var result = request.Send(url, nameof(QueryAsync), MessagePackSerializer.Serialize(new object[] { sql, param ?? new object(), commandTimeout ?? int.MaxValue, commandType ?? CommandType.Text }), globalCommandTimeout);
                if (result == Array.Empty<byte>()) return [];
                var rows = MessagePackSerializer.Deserialize<IEnumerable<IDictionary<string, object>>>(result) ?? [];
                return rows.Select(row => MapRow(row, map, splitOn, aliases)).ToList();
            }
            return AsyncHelper.RunSync(() => QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType, aliases));
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return [];
        }
    }

    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        try
        {
            if (OperatingSystem.IsBrowser())
            {
                logger?.LogWarning("Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> is not recommended for use in WebAssembly due to potential performance issues. Consider using QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> instead.");
                var request = provider.GetRequiredService<IXmlHttpRequest>();
                var result = request.Send(url, nameof(QueryAsync), MessagePackSerializer.Serialize(new object[] { sql, param ?? new object(), commandTimeout ?? int.MaxValue, commandType ?? CommandType.Text }), globalCommandTimeout);
                if (result == Array.Empty<byte>()) return [];
                var rows = MessagePackSerializer.Deserialize<IEnumerable<IDictionary<string, object>>>(result) ?? [];
                return rows.Select(row => MapRow(row, map, splitOn, aliases)).ToList();
            }
            return AsyncHelper.RunSync(() => QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType, aliases));
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return [];
        }
    }

    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        try
        {
            if (OperatingSystem.IsBrowser())
            {
                logger?.LogWarning("Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> is not recommended for use in WebAssembly due to potential performance issues. Consider using QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> instead.");
                var request = provider.GetRequiredService<IXmlHttpRequest>();
                var result = request.Send(url, nameof(QueryAsync), MessagePackSerializer.Serialize(new object[] { sql, param ?? new object(), commandTimeout ?? int.MaxValue, commandType ?? CommandType.Text }), globalCommandTimeout);
                if (result == Array.Empty<byte>()) return [];
                var rows = MessagePackSerializer.Deserialize<IEnumerable<IDictionary<string, object>>>(result) ?? [];
                return rows.Select(row => MapRow(row, map, splitOn, aliases)).ToList();
            }
            return AsyncHelper.RunSync(() => QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType, aliases));
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return [];
        }
    }

    public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        try
        {
            using var tcs = new CancellationTokenSource(globalCommandTimeout);
            var service = provider.GetRequiredService<IMagicOnionLayer>();
            var result = await service
                .WithCancellationToken(tcs.Token)
                .QueryAsync(sql, param, commandTimeout, commandType ?? CommandType.Text)
                .ConfigureAwait(false);
            if (!result.Successful) throw new Exception(result.Exception);
            if (result.Data == Array.Empty<byte>()) return [];
            var rows = MessagePackSerializer.Deserialize<IEnumerable<IDictionary<string, object>>>(result.Data) ?? [];
            return rows.Select(row => MapRow(row, map, splitOn, aliases)).ToList();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return Enumerable.Empty<TReturn>();
        }
    }

    public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        try
        {
            using var tcs = new CancellationTokenSource(globalCommandTimeout);
            var service = provider.GetRequiredService<IMagicOnionLayer>();
            var result = await service
                .WithCancellationToken(tcs.Token)
                .QueryAsync(sql, param, commandTimeout, commandType ?? CommandType.Text)
                .ConfigureAwait(false);
            if (!result.Successful) throw new Exception(result.Exception);
            if (result.Data == Array.Empty<byte>()) return [];
            var rows = MessagePackSerializer.Deserialize<IEnumerable<IDictionary<string, object>>>(result.Data) ?? [];
            return rows.Select(row => MapRow(row, map, splitOn, aliases)).ToList();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return Enumerable.Empty<TReturn>();
        }
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null)
    {
        return QueryAsync(command.CommandText, map, command.Parameters, command.Transaction, command.Buffered, splitOn, command.CommandTimeout, command.CommandType, aliases);
    }

    public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        try
        {
            using var tcs = new CancellationTokenSource(globalCommandTimeout);
            var service = provider.GetRequiredService<IMagicOnionLayer>();
            var result = await service
                .WithCancellationToken(tcs.Token)
                .QueryAsync(sql, param, commandTimeout, commandType ?? CommandType.Text)
                .ConfigureAwait(false);
            if (!result.Successful) throw new Exception(result.Exception);
            if (result.Data == Array.Empty<byte>()) return [];
            var rows = MessagePackSerializer.Deserialize<IEnumerable<IDictionary<string, object>>>(result.Data) ?? [];
            return rows.Select(row => MapRow(row, map, splitOn, aliases)).ToList();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return Enumerable.Empty<TReturn>();
        }
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null)
    {
        return QueryAsync(command.CommandText, map, command.Parameters, command.Transaction, command.Buffered, splitOn, command.CommandTimeout, command.CommandType, aliases);
    }

    public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        try
        {
            using var tcs = new CancellationTokenSource(globalCommandTimeout);
            var service = provider.GetRequiredService<IMagicOnionLayer>();
            var result = await service
                .WithCancellationToken(tcs.Token)
                .QueryAsync(sql, param, commandTimeout, commandType ?? CommandType.Text)
                .ConfigureAwait(false);
            if (!result.Successful) throw new Exception(result.Exception);
            if (result.Data == Array.Empty<byte>()) return [];
            var rows = MessagePackSerializer.Deserialize<IEnumerable<IDictionary<string, object>>>(result.Data) ?? [];
            return rows.Select(row => MapRow(row, map, splitOn, aliases)).ToList();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return Enumerable.Empty<TReturn>();
        }
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null)
    {
        return QueryAsync(command.CommandText, map, command.Parameters, command.Transaction, command.Buffered, splitOn, command.CommandTimeout, command.CommandType, aliases);
    }

    public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        try
        {
            using var tcs = new CancellationTokenSource(globalCommandTimeout);
            var service = provider.GetRequiredService<IMagicOnionLayer>();
            var result = await service
                .WithCancellationToken(tcs.Token)
                .QueryAsync(sql, param, commandTimeout, commandType ?? CommandType.Text)
                .ConfigureAwait(false);
            if (!result.Successful) throw new Exception(result.Exception);
            if (result.Data == Array.Empty<byte>()) return [];
            var rows = MessagePackSerializer.Deserialize<IEnumerable<IDictionary<string, object>>>(result.Data) ?? [];
            return rows.Select(row => MapRow(row, map, splitOn, aliases)).ToList();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return Enumerable.Empty<TReturn>();
        }
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null)
    {
        return QueryAsync(command.CommandText, map, command.Parameters, command.Transaction, command.Buffered, splitOn, command.CommandTimeout, command.CommandType, aliases);
    }

    public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IEnumerable<DBAlias>? aliases = null)
    {
        try
        {
            using var tcs = new CancellationTokenSource(globalCommandTimeout);
            var service = provider.GetRequiredService<IMagicOnionLayer>();
            var result = await service
                .WithCancellationToken(tcs.Token)
                .QueryAsync(sql, param, commandTimeout, commandType ?? CommandType.Text)
                .ConfigureAwait(false);
            if (!result.Successful) throw new Exception(result.Exception);
            if (result.Data == Array.Empty<byte>()) return [];
            var rows = MessagePackSerializer.Deserialize<IEnumerable<IDictionary<string, object>>>(result.Data) ?? [];
            return rows.Select(row => MapRow(row, map, splitOn, aliases)).ToList();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return Enumerable.Empty<TReturn>();
        }
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, string splitOn = "Id", IEnumerable<DBAlias>? aliases = null)
    {
        return QueryAsync(command.CommandText, map, command.Parameters, command.Transaction, command.Buffered, splitOn, command.CommandTimeout, command.CommandType, aliases);
    }

    private static List<Dictionary<string, object>> SplitRow(IDictionary<string, object> row, string splitOn, int groupCount, IEnumerable<DBAlias>? aliases)
    {
        var splitOns = splitOn.Split(',', StringSplitOptions.TrimEntries);
        var keys = row.Keys.ToList();
        var groups = new List<Dictionary<string, object>>();
        int groupStart = 0;

        for (int g = 0; g < groupCount; g++)
        {
            int groupEnd;

            if (g == groupCount - 1)
            {
                groupEnd = keys.Count;
            }
            else
            {
                // Use the matching splitOn if one was given per-group, else reuse the single value (Dapper convention)
                string splitKey = g < splitOns.Length ? splitOns[g] : splitOns[^1];
                groupEnd = keys.Count;

                for (int i = groupStart + 1; i < keys.Count; i++)
                {
                    if (string.Equals(keys[i], splitKey, StringComparison.OrdinalIgnoreCase))
                    {
                        groupEnd = i;
                        break;
                    }
                }
            }

            var dict = new Dictionary<string, object>();
            for (int i = groupStart; i < groupEnd; i++)
                dict[GetKey(keys[i], aliases)] = row[keys[i]];

            groups.Add(dict);
            groupStart = groupEnd;
        }

        return groups;

        static string GetKey(string key, IEnumerable<DBAlias>? aliases)
        {
            if (aliases is null) return key;
            var alias = aliases.SingleOrDefault(a => a.alias == key);
            return alias?.columnName ?? key;
        }
    }

    private static T MapToObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(IDictionary<string, object> dict)
    {
        var obj = Activator.CreateInstance<T>();
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite);

        foreach (var prop in props)
        {
            var match = dict.Keys.FirstOrDefault(k => string.Equals(k, prop.Name, StringComparison.OrdinalIgnoreCase));
            if (match is null) continue;

            var value = dict[match];
            if (value is null)
            {
                prop.SetValue(obj, null);
                continue;
            }

            var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            var converted = targetType.IsEnum
                ? Enum.ToObject(targetType, value)
                : Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);

            prop.SetValue(obj, converted);
        }

        return obj;
    }

    private static TReturn MapRow<TFirst, TSecond, TReturn>(IDictionary<string, object> row, Func<TFirst, TSecond, TReturn> map, string splitOn, IEnumerable<DBAlias>? aliases)
    {
        var groups = SplitRow(row, splitOn, 2, aliases);

        var first = MapToObject<TFirst>(groups[0]);
        var second = MapToObject<TSecond>(groups[1]);

        return map(first, second);
    }

    private static TReturn MapRow<TFirst, TSecond, TThird, TReturn>(IDictionary<string, object> row, Func<TFirst, TSecond, TThird, TReturn> map, string splitOn, IEnumerable<DBAlias>? aliases)
    {
        var groups = SplitRow(row, splitOn, 3, aliases);

        var first = MapToObject<TFirst>(groups[0]);
        var second = MapToObject<TSecond>(groups[1]);
        var third = MapToObject<TThird>(groups[2]);

        return map(first, second, third);
    }

    private static TReturn MapRow<TFirst, TSecond, TThird, TFourth, TReturn>(IDictionary<string, object> row, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string splitOn, IEnumerable<DBAlias>? aliases)
    {
        var groups = SplitRow(row, splitOn, 4, aliases);

        var first = MapToObject<TFirst>(groups[0]);
        var second = MapToObject<TSecond>(groups[1]);
        var third = MapToObject<TThird>(groups[2]);
        var fourth = MapToObject<TFourth>(groups[3]);

        return map(first, second, third, fourth);
    }

    private static TReturn MapRow<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(IDictionary<string, object> row, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string splitOn, IEnumerable<DBAlias>? aliases)
    {
        var groups = SplitRow(row, splitOn, 5, aliases);

        var first = MapToObject<TFirst>(groups[0]);
        var second = MapToObject<TSecond>(groups[1]);
        var third = MapToObject<TThird>(groups[2]);
        var fourth = MapToObject<TFourth>(groups[3]);
        var fifth = MapToObject<TFifth>(groups[4]);

        return map(first, second, third, fourth, fifth);
    }

    private static TReturn MapRow<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(IDictionary<string, object> row, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, string splitOn, IEnumerable<DBAlias>? aliases)
    {
        var groups = SplitRow(row, splitOn, 6, aliases);

        var first = MapToObject<TFirst>(groups[0]);
        var second = MapToObject<TSecond>(groups[1]);
        var third = MapToObject<TThird>(groups[2]);
        var fourth = MapToObject<TFourth>(groups[3]);
        var fifth = MapToObject<TFifth>(groups[4]);
        var sixth = MapToObject<TSixth>(groups[5]);

        return map(first, second, third, fourth, fifth, sixth);
    }

    private static TReturn MapRow<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(IDictionary<string, object> row, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, string splitOn, IEnumerable<DBAlias>? aliases)
    {
        var groups = SplitRow(row, splitOn, 7, aliases);

        var first = MapToObject<TFirst>(groups[0]);
        var second = MapToObject<TSecond>(groups[1]);
        var third = MapToObject<TThird>(groups[2]);
        var fourth = MapToObject<TFourth>(groups[3]);
        var fifth = MapToObject<TFifth>(groups[4]);
        var sixth = MapToObject<TSixth>(groups[5]);
        var seventh = MapToObject<TSeventh>(groups[6]);

        return map(first, second, third, fourth, fifth, sixth, seventh);
    }

    public int Execute(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        throw new NotImplementedException();
    }

    public Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        throw new NotImplementedException();
    }
}
