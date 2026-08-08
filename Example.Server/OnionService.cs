using Avae.Abstractions;
using Avae.DAL;
using Avae.DAL.Grpc;
using Avae.DAL.Interfaces;
using Example.Models;
using MagicOnion;
using MagicOnion.Server;
using MessagePack;

namespace Example.Server
{
    public class OnionService : ServiceBase<IOnionService>, IOnionService
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
            if (RequestEntityHandler(type, out var result, out var handler, out var options))
            {
                return new Result()
                {
                    Successful = true,
                    Data = MessagePackSerializer.Serialize(handler!.Enumerable, await handler.FindByAnyAsync(filters, commandTimeout), options)
                };
            }
            return result!;
        }

        public async UnaryResult<Result> GetAllAsync(string type, int? commandTimeout = null)
        {
            if (RequestEntityHandler(type, out var result, out var handler, out var options))
            {
                return new Result()
                {
                    Successful = true,
                    Data = MessagePackSerializer.Serialize(handler!.Enumerable, await handler.GetAllAsync(commandTimeout), options)
                };
            }
            return result!;
        }

        public async UnaryResult<Result> GetAsync(string type, long id, int? commandTimeout = null)
        {
            if (RequestEntityHandler(type, out var result, out var handler, out var options))
            {
                return new Result()
                {
                    Successful = true,
                    Data = MessagePackSerializer.Serialize(handler!.Type, await handler.GetAsync(id, commandTimeout), options)
                };
            }
            return result!;
        }

        public async UnaryResult<Result> WhereAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null)
        {
            if(RequestEntityHandler(type, out var result, out var handler, out var options))
            {
                return new Result()
                {
                    Successful = true,
                    Data = MessagePackSerializer.Serialize(handler!.Enumerable, await handler.WhereAsync(filters, commandTimeout), options)
                };
            }

            return result!;
        }

        private bool RequestEntityHandler(string type, out Result? result, out EntityHandler? handler, out MessagePackSerializerOptions? options)       
        {
            result = null;
            handler = null;
            options = null;

            if (string.IsNullOrWhiteSpace(type))
                result = new Result() { Successful = false, Exception = "Type parameter is required" };
            else if (!EntityHandler.Handlers.TryGetValue(type, out handler))
                result = new Result() { Successful = false, Exception = "Unable to find entity handler" };

            if (type == typeof(Person).Name)
            {
                options = new PersonOptions(new MessagePackSerializerOptions(CustomResolver.Instance));
            }

            return result is null;
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
