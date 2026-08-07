using Avae.Abstractions;
using Avae.DAL;
using Example.Models;
using MagicOnion;
using MagicOnion.Server;
using MessagePack;
//using MemoryPack;

namespace Example.Server
{
    public class OnionService : ServiceBase<IDBOnionService>, IDBOnionService
    {
        private static readonly IDBLayer Layer = ServiceLocator.GetRequiredService<IDBLayer>();

        static OnionService()
        {
            EntityHandler.Handlers = new Dictionary<string, EntityHandler>()
            {
                 { nameof(Person), new EntityHandler<Person>(Layer) },
                { nameof(Contact), new EntityHandler<Contact>(Layer) }
            };
        }

        public async UnaryResult<Result> FindByAnyAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null)
        {
            if (string.IsNullOrWhiteSpace(type))
                return new Result() { Successful = false, Exception = "Type parameter is required" };

            if (!EntityHandler.Handlers.TryGetValue(type, out var handler))
                return new Result() { Successful = false, Exception = "Unable to find entity handler" };

            return new Result()
            {
                Successful = true,
                Data = MessagePackSerializer.Serialize(handler.Enumerable, await handler.FindByAnyAsync(filters, commandTimeout))
            };
        }

        public async UnaryResult<Result> GetAllAsync(string type, int? commandTimeout = null)
        {
            if (string.IsNullOrWhiteSpace(type))
                return new Result() { Successful = false, Exception = "Type parameter is required" };

            if (!EntityHandler.Handlers.TryGetValue(type, out var handler))
                return new Result() { Successful = false, Exception = "Unable to find entity handler" };


            return new Result()
            {
                Successful = true,
                Data = MessagePackSerializer.Serialize(handler.Enumerable, await handler.GetAllAsync(commandTimeout))
            };
        }

        public async UnaryResult<Result> GetAsync(string type, long id, int? commandTimeout = null)
        {
            if (string.IsNullOrWhiteSpace(type))
                return new Result() { Successful = false, Exception = "Type parameter is required" };

            if (!EntityHandler.Handlers.TryGetValue(type, out var handler))
                return new Result() { Successful = false, Exception = "Unable to find entity handler" };

            return new Result()
            {
                Successful = true,
                Data = MessagePackSerializer.Serialize(handler.Type, await handler.GetAsync(id, commandTimeout))
            };
        }

        public async UnaryResult<Result> WhereAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null)
        {
            if (string.IsNullOrWhiteSpace(type))
                return new Result() { Successful = false, Exception = "Type parameter is required" };

            if (!EntityHandler.Handlers.TryGetValue(type, out var handler))
                return new Result() { Successful = false, Exception = "Unable to find entity handler" };

            return new Result()
            {
                Successful = true,
                Data = MessagePackSerializer.Serialize(handler.Enumerable, await handler.WhereAsync(filters, commandTimeout))
            };
        }


        public async UnaryResult<Result> DbTransRemove(DBModelBase modelBase)
        {
            return await modelBase.DbTransRemove(Layer);
        }

        public async UnaryResult<Result> DbTransSave(DBModelBase modelBase)
        {
            return await modelBase.DbTransSave(Layer);
        }
    }
}
