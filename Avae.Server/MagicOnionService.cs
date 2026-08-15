using Avae.Abstractions;
using Avae.DAL;
using Avae.MagicLayer;
using MagicOnion;
using MagicOnion.Server;
using MessagePack;

namespace Avae.Server
{    
    public abstract class MagicOnionService : ServiceBase<IMagicOnionLayer>, IMagicOnionLayer
    {
        private async UnaryResult<DBResult> Request(string type, Func<EntityHandler, UnionMessagePackSerializerOptions?, Task<byte[]>> serialize)
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

        protected virtual UnionMessagePackSerializerOptions? GetOptions(string type)
        {
            return null;
        }

        public async UnaryResult<DBResult> Remove(DBTransactional transactional, string connectionId)
        {
            var layer = ServiceLocator.GetRequiredService<IDBLayer>();
            DBContext.CurrentConnectionId.Value = connectionId;
            try
            {
                return await transactional.Remove(layer);
            }
            finally
            {
                DBContext.CurrentConnectionId.Value = null;
            }            
        }

        public async UnaryResult<DBResult> Save(DBTransactional transactional, string connectionId)
        {
            var layer = ServiceLocator.GetRequiredService<IDBLayer>();
            DBContext.CurrentConnectionId.Value = connectionId;
            try
            {
                return await transactional.Save(layer);
            }
            finally
            {
                DBContext.CurrentConnectionId.Value = null;
            }
        }
    }
}
