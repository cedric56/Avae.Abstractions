using Avae.Abstractions;
using Avae.DAL;
using Example.Models;
using MagicOnion;
using MagicOnion.Server;
using MemoryPack;
using System.Buffers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Example.Server
{
    public class OnionService : ServiceBase<IDBOnionService>, IDBOnionService
    {
        private abstract class EntityHandler
        {
            public abstract Type Type { get; }
            public abstract Type Enumerable { get; }

            public abstract Task<object> GetAllAsync();
            public abstract Task<object> GetAsync(long id);
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
            public override async Task<object> GetAsync(long id)
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

        Dictionary<string, EntityHandler> Dic = new Dictionary<string, EntityHandler>()
        {
            { "Person", new EntityHandler<Person>() },
            { "Contact", new EntityHandler<Contact>() }
        };

        private static IDBLayer Layer = SimpleProvider.GetService<IDBLayer>();

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
            var value = Dic.GetValueOrDefault(type);
            return MemoryPackSerializer.Serialize(value.Enumerable, await value.FindByAnyAsync(filters));
        }

        public async UnaryResult<byte[]> GetAllAsync(string type)
        {
            var value = Dic.GetValueOrDefault(type);
            return MemoryPackSerializer.Serialize(value.Enumerable, await value.GetAllAsync());
        }

        public async UnaryResult<byte[]> GetAsync(string type, long id)
        {
            var value = Dic.GetValueOrDefault(type);
            return MemoryPackSerializer.Serialize(value.Type, await value.GetAsync(id));
        }

        public async UnaryResult<byte[]> WhereAsync(string type, Dictionary<string, object> filters)
        {
            var value = Dic.GetValueOrDefault(type);
            return MemoryPackSerializer.Serialize(value.Enumerable, await value.WhereAsync(filters));
        }

        public async UnaryResult<string> GetAllAsyncAsString(string type)
        {
            var value = Dic.GetValueOrDefault(type);
            return JsonSerializer.Serialize(await value.GetAllAsync());
        }
    }
}
