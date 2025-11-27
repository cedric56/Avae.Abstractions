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
        async UnaryResult<Result> FindByAnyAsync(string type, Dictionary<string, object> filters)
        {
            if (string.IsNullOrWhiteSpace(type))
                return new Result() { Successful = false, Exception = "Type parameter is required" };

            if (!EntityHandler.Handlers.TryGetValue(type, out var handler))
                return new Result() { Successful = false, Exception = "Unable to find entity handler" };

            return new Result()
            {
                Successful = true,
                Data = MemoryPackSerializer.Serialize(handler.Enumerable, await handler.FindByAnyAsync(filters))
            };
        }

        async UnaryResult<Result> GetAllAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return new Result() { Successful = false, Exception = "Type parameter is required" };

            if (!EntityHandler.Handlers.TryGetValue(type, out var handler))
                return new Result() { Successful = false, Exception = "Unable to find entity handler" };


            return new Result()
            {
                Successful = true,
                Data = MemoryPackSerializer.Serialize(handler.Enumerable, await handler.GetAllAsync())
            };
        }

        async UnaryResult<Result> GetAsync(string type, long id)
        {
            if (string.IsNullOrWhiteSpace(type))
                return new Result() { Successful = false, Exception = "Type parameter is required" };

            if (!EntityHandler.Handlers.TryGetValue(type, out var handler))
                return new Result() { Successful = false, Exception = "Unable to find entity handler" };

            return new Result()
            {
                Successful = true,
                Data = MemoryPackSerializer.Serialize(handler.Type, await handler.GetAsync(id))
            };
        }

        async UnaryResult<Result> WhereAsync(string type, Dictionary<string, object> filters)
        {
            if (string.IsNullOrWhiteSpace(type))
                return new Result() { Successful = false, Exception = "Type parameter is required" };

            if (!EntityHandler.Handlers.TryGetValue(type, out var handler))
                return new Result() { Successful = false, Exception = "Unable to find entity handler" };

            return new Result()
            {
                Successful = true,
                Data = MemoryPackSerializer.Serialize(handler.Enumerable, await handler.WhereAsync(filters))
            };
        }
    }

    public interface IXmlHttpRequest
    {
        Result Send(string url, string data);
    }
}
