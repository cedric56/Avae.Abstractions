using Avae.Abstractions;
using Example.Models;
using MagicOnion;
using MagicOnion.Server;
using MemoryPack;

namespace Example.Server
{
    public class OnionService : ServiceBase<IDBOnionService>, IDBOnionService
    {
        private abstract class EntityHandler
        {
            public abstract Type Type { get; }
            public abstract Type Enumerable { get; }

            public abstract Task<object> GetAllAsync();
            public abstract Task<object?> GetAsync(long id);
            public abstract Task<object> FindByAnyAsync(Dictionary<string, object> filters);
            public abstract Task<object> WhereAsync(Dictionary<string, object> filters);
        }
        private class EntityHandler<T>() : EntityHandler where T : class, new() {

            public override Type Type => typeof(T);

            public override Type Enumerable => typeof(IEnumerable<T>);

            public override async Task<object> GetAllAsync()
            {
                return await Layer.GetAllAsync<T>();
            }
            public override async Task<object?> GetAsync(long id)
            {
                return await Layer.GetAsync<T>(id);
            }
            public override async Task<object> FindByAnyAsync(Dictionary<string, object> filters)
            {
                return await Layer.FindByAnyAsync<T>(filters);
            }
            public override async Task<object> WhereAsync(Dictionary<string, object> filters)
            {
                return await Layer.WhereAsync<T>(filters);
            }
        }

        readonly Dictionary<string, EntityHandler> Handlers = new()
        {
            { "Person", new EntityHandler<Person>() },
            { "Contact", new EntityHandler<Contact>() }
        };

        private static readonly IDBLayer Layer = SimpleProvider.GetService<IDBLayer>();

        public async UnaryResult<Result> DbTransRemove(DBModelBase modelBase)
        {
            return await modelBase.DbTransRemove(Layer);
        }

        public async UnaryResult<Result> DbTransSave(DBModelBase modelBase)
        {
            return await modelBase.DbTransSave(Layer);
        }

        public async UnaryResult<byte[]> FindByAnyAsync(string type, Dictionary<string, object> filters)
        {
            var value = Handlers.TryGetValue(type, out var handler) ?
                MemoryPackSerializer.Serialize(handler.Enumerable, await handler.FindByAnyAsync(filters)) :
                [];
            return value;
        }

        public async UnaryResult<byte[]> GetAllAsync(string type)
        {
            var value = Handlers.TryGetValue(type, out var handler) ?
                MemoryPackSerializer.Serialize(handler.Enumerable, await handler.GetAllAsync()) :
                [];
            return value;
        }

        public async UnaryResult<byte[]> GetAsync(string type, long id)
        {
            var value = Handlers.TryGetValue(type, out var handler) ?
                MemoryPackSerializer.Serialize(handler.Type, await handler.GetAsync(id)) :
                [];
            return value;
        }

        public async UnaryResult<byte[]> WhereAsync(string type, Dictionary<string, object> filters)
        {
            var value = Handlers.TryGetValue(type, out var handler) ?
                MemoryPackSerializer.Serialize(handler.Enumerable, await handler.WhereAsync(filters)) :
                [];
            return value;
        }
    }
}
