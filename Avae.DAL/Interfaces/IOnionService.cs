using MagicOnion;
using MemoryPack;

namespace Avae.DAL.Interfaces
{
    public abstract class EntityHandler
    {
        public static Dictionary<string, EntityHandler> Handlers { get; set; } = [];

        public abstract Type Type { get; }
        public abstract Type Enumerable { get; }

        public abstract Task<object> GetAllAsync();
        public abstract Task<object?> GetAsync(long id);
        public abstract Task<object> FindByAnyAsync(Dictionary<string, object> filters);
        public abstract Task<object> WhereAsync(Dictionary<string, object> filters);
    }

    public class EntityHandler<T>(IDataAccessLayer layer) : EntityHandler where T : class, new()
    {
        public override Type Type => typeof(T);

        public override Type Enumerable => typeof(IEnumerable<T>);

        public override async Task<object> GetAllAsync()
        {
            return await layer.GetAllAsync<T>();
        }
        public override async Task<object?> GetAsync(long id)
        {
            return await layer.GetAsync<T>(id);
        }
        public override async Task<object> FindByAnyAsync(Dictionary<string, object> filters)
        {
            return await layer.FindByAnyAsync<T>(filters);
        }
        public override async Task<object> WhereAsync(Dictionary<string, object> filters)
        {
            return await layer.WhereAsync<T>(filters);
        }
    }

    public interface IOnionService
    {
        async UnaryResult<byte[]> FindByAnyAsync(string type, Dictionary<string, object> filters)
        {
            var value = EntityHandler.Handlers.TryGetValue(type, out var handler) ?
                MemoryPackSerializer.Serialize(handler.Enumerable, await handler.FindByAnyAsync(filters)) :
                [];
            return value;
        }

        async UnaryResult<byte[]> GetAllAsync(string type)
        {
            var value = EntityHandler.Handlers.TryGetValue(type, out var handler) ?
                MemoryPackSerializer.Serialize(handler.Enumerable, await handler.GetAllAsync()) :
                [];
            return value;
        }

        async UnaryResult<byte[]> GetAsync(string type, long id)
        {
            var value = EntityHandler.Handlers.TryGetValue(type, out var handler) ?
                MemoryPackSerializer.Serialize(handler.Type, await handler.GetAsync(id)) :
                [];
            return value;
        }

        async UnaryResult<byte[]> WhereAsync(string type, Dictionary<string, object> filters)
        {
            var value = EntityHandler.Handlers.TryGetValue(type, out var handler) ?
                MemoryPackSerializer.Serialize(handler.Enumerable, await handler.WhereAsync(filters)) :
                [];
            return value;
        }
    }

    public interface IXmlHttpRequest
    {
        byte[]? Send(string url, string data);
    }
}
