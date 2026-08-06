using MagicOnion;
using MemoryPack;

namespace Avae.DAL.Interfaces
{
    public interface IOnionService
    {
        async UnaryResult<Result> FindByAnyAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null)
        {
            if (string.IsNullOrWhiteSpace(type))
                return new Result() { Successful = false, Exception = "Type parameter is required" };

            if (!EntityHandler.Handlers.TryGetValue(type, out var handler))
                return new Result() { Successful = false, Exception = "Unable to find entity handler" };

            return new Result()
            {
                Successful = true,
                Data = MemoryPackSerializer.Serialize(handler.Enumerable, await handler.FindByAnyAsync(filters, commandTimeout))
            };
        }

        async UnaryResult<Result> GetAllAsync(string type, int? commandTimeout = null)
        {
            if (string.IsNullOrWhiteSpace(type))
                return new Result() { Successful = false, Exception = "Type parameter is required" };

            if (!EntityHandler.Handlers.TryGetValue(type, out var handler))
                return new Result() { Successful = false, Exception = "Unable to find entity handler" };


            return new Result()
            {
                Successful = true,
                Data = MemoryPackSerializer.Serialize(handler.Enumerable, await handler.GetAllAsync(commandTimeout))
            };
        }

        async UnaryResult<Result> GetAsync(string type, long id, int? commandTimeout = null)
        {
            if (string.IsNullOrWhiteSpace(type))
                return new Result() { Successful = false, Exception = "Type parameter is required" };

            if (!EntityHandler.Handlers.TryGetValue(type, out var handler))
                return new Result() { Successful = false, Exception = "Unable to find entity handler" };

            return new Result()
            {
                Successful = true,
                Data = MemoryPackSerializer.Serialize(handler.Type, await handler.GetAsync(id, commandTimeout))
            };
        }

        async UnaryResult<Result> WhereAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null)
        {
            if (string.IsNullOrWhiteSpace(type))
                return new Result() { Successful = false, Exception = "Type parameter is required" };

            if (!EntityHandler.Handlers.TryGetValue(type, out var handler))
                return new Result() { Successful = false, Exception = "Unable to find entity handler" };

            return new Result()
            {
                Successful = true,
                Data = MemoryPackSerializer.Serialize(handler.Enumerable, await handler.WhereAsync(filters, commandTimeout))
            };
        }
    }

    public interface IXmlHttpRequest
    {
        bool IsConnected { get; set; }
        Result Send(string url, string data);
    }
}
