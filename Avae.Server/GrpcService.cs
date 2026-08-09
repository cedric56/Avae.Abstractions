using Avae.Abstractions;
using Avae.DAL;
using Avae.DAL.Interfaces;
using Avae.Grpc;
using MagicOnion;
using MagicOnion.Server;
using MessagePack;

namespace Avae.Server
{    
    public abstract class GrpcService : ServiceBase<IGrpcLayer>, IGrpcLayer
    {
        protected static readonly IDBLayer Layer = ServiceLocator.GetRequiredService<IDBLayer>();

        public async UnaryResult<DBResult> FindByAnyAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null)
        {
            if (RequestEntityHandler(type, out var result, out var handler, out var options))
            {
                return new DBResult()
                {
                    Successful = true,
                    Data = MessagePackSerializer.Serialize(handler!.Enumerable, await handler.FindByAnyAsync(filters, commandTimeout), options)
                };
            }
            return result!;
        }

        public async UnaryResult<DBResult> GetAllAsync(string type, int? commandTimeout = null)
        {
            if (RequestEntityHandler(type, out var result, out var handler, out var options))
            {
                return new DBResult()
                {
                    Successful = true,
                    Data = MessagePackSerializer.Serialize(handler!.Enumerable, await handler.GetAllAsync(commandTimeout), options)
                };
            }
            return result!;
        }

        public async UnaryResult<DBResult> GetAsync(string type, long id, int? commandTimeout = null)
        {
            if (RequestEntityHandler(type, out var result, out var handler, out var options))
            {
                return new DBResult()
                {
                    Successful = true,
                    Data = MessagePackSerializer.Serialize(handler!.Type, await handler.GetAsync(id, commandTimeout), options)
                };
            }
            return result!;
        }

        public async UnaryResult<DBResult> WhereAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null)
        {
            if (RequestEntityHandler(type, out var result, out var handler, out var options))
            {
                return new DBResult()
                {
                    Successful = true,
                    Data = MessagePackSerializer.Serialize(handler!.Enumerable, await handler.WhereAsync(filters, commandTimeout), options)
                };
            }

            return result!;
        }

        private bool RequestEntityHandler(string type, out DBResult? result, out EntityHandler? handler, out UnionMessagePackSerializerOptions? options)
        {
            result = null;
            handler = null;
            options = GetOptions(type);

            if (string.IsNullOrWhiteSpace(type))
                result = new DBResult() { Successful = false, Exception = "Type parameter is required" };
            else if (!EntityHandler.Handlers.TryGetValue(type, out handler))
                result = new DBResult() { Successful = false, Exception = "Unable to find entity handler" };
            
            return result is null;
        }

        protected virtual UnionMessagePackSerializerOptions? GetOptions(string type)
        {
            return null;
        }

        public async UnaryResult<DBResult> Remove(DBTransactional transactional)
        {
            return await transactional.Remove(Layer);
        }

        public async UnaryResult<DBResult> Save(DBTransactional transactional)
        {
            return await transactional.Save(Layer);
        }
    }
}
