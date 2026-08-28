using Avae.Core;
using Avae.DAL;
using Avae.DAL.gRPC;
using Dapper;
using MagicOnion;
using MagicOnion.Server;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Data;

namespace Avae.Server;

public abstract class MagicOnionService : ServiceBase<IMagicOnionLayer>, IMagicOnionLayer
{
    private async UnaryResult<DBResult> Request(string type, Func<EntityHandler, DBTransactionalSerializerOptions?, Task<byte[]>> serialize)
    {            
        if (string.IsNullOrWhiteSpace(type))
        {
            return new DBResult() 
            { 
                Successful = false, 
                Exception = "Type parameter is required" 
            };
        }
        else if (!EntityHandler.Handlers.TryGetValue(type, out var handler))
        {
            return new DBResult() 
            { 
                Successful = false, 
                Exception = "Unable to find entity handler" 
            };
        }
        else
        {
            try
            {
                return new DBResult()
                {
                    Successful = true,
                    Data = await serialize(handler, GetOptions(type))
                };
            }
            catch (Exception ex)
            {
                return new DBResult()
                {
                    Successful = false,
                    Exception = ex.Message
                };
            }
        }
    }

    public UnaryResult<DBResult> FindByAnyAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null)
    {
        return Request(type, async (entity, options) => MessagePackSerializer.Serialize(entity.Enumerable, await entity.FindByAnyAsync(filters, commandTimeout), options));
    }

    public UnaryResult<DBResult> GetAllAsync(string type, int? commandTimeout = null)
    {
        return Request(type, async (entity, options) => MessagePackSerializer.Serialize(entity.Enumerable, await entity.GetAllAsync(commandTimeout), options));
    }

    public UnaryResult<DBResult> GetAsync(string type, long id, int? commandTimeout = null)
    {
        return Request(type, async (entity, options) => MessagePackSerializer.Serialize(entity.Type, await entity.GetAsync(id, commandTimeout), options));
    }

    public UnaryResult<DBResult> WhereAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null)
    {
        return Request(type, async (entity, options) => MessagePackSerializer.Serialize(entity.Enumerable, await entity.WhereAsync(filters, commandTimeout), options));
    }

    protected virtual DBTransactionalSerializerOptions? GetOptions(string type)
    {
        return null;
    }

    public async UnaryResult<DBResult> Remove(DBTransactional transactional, string connectionId, int? commandTimeout = null)
    {
        var layer = ServiceLocator.GetRequiredService<IDBLayer>();
        DBContext.CurrentConnectionId.Value = connectionId;
        try
        {
            return await transactional.Remove(layer, commandTimeout);
        }
        finally
        {
            DBContext.CurrentConnectionId.Value = null;
        }            
    }

    public async UnaryResult<DBResult> Save(DBTransactional transactional, string connectionId, int? commandTimeout = null)
    {
        var layer = ServiceLocator.GetRequiredService<IDBLayer>();
        DBContext.CurrentConnectionId.Value = connectionId;
        try
        {
            return await transactional.Save(layer, commandTimeout);
        }
        finally
        {
            DBContext.CurrentConnectionId.Value = null;
        }
    }

    public async UnaryResult<DBResult> QueryAsync(string sql, object? param = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
    {        
        try
        {
            var layer = ServiceLocator.GetRequiredService<IDBLayer>();
            using var db = ServiceLocator.Default.GetRequiredService<IDbConnection>();
            var results = await db.QueryAsync(sql, GetParam(param), commandTimeout: commandTimeout, commandType: commandType);
            return new DBResult()
            {
                Successful = true,
                Data = MessagePackSerializer.Serialize(results.Select(row => (IDictionary<string, object>)row))
            };
        }
        catch (Exception ex)
        {
            return new DBResult()
            {
                Successful = false,
                Exception = ex.Message
            };
        }
    }

    private static object? GetParam(object? param)
    {
        if (param is IEnumerable ie)
        {
            var dp = new DynamicParameters();
            foreach (var item in ie)
            {
                var type = item.GetType();
                var keyProp = type.GetProperty("Key");
                var valueProp = type.GetProperty("Value");

                if (keyProp != null && valueProp != null)
                {
                    var key = keyProp.GetValue(item)?.ToString();
                    var value = valueProp.GetValue(item);
                    if (key != null)
                        dp.Add(key, value);
                }
            }
            return dp;
        }

        return param;
    }
}
