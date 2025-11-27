using Avae.Abstractions;
using Avae.DAL.Interfaces;
using MemoryPack;
using System.Data;
using System.Text.Json;

namespace Avae.DAL
{
    public partial class OnionLayer : IDataAccessLayer
    {
        private static IOnionService Service => SimpleProvider.GetService<IOnionService>();

        public IEnumerable<T> FindByAny<T>(Dictionary<string, object> filters) where T : class, new()
        {
            if (OperatingSystem.IsBrowser())
            {
                var request = SimpleProvider.GetService<IXmlHttpRequest>();
                var result = request.Send(nameof(FindByAnyAsync), $"type={typeof(T).Name}&filters={JsonSerializer.Serialize(filters)}");
                if (!result.Successful) throw new Exception(result.Exception);
                return MemoryPackSerializer.Deserialize<IEnumerable<T>>(result.Data) ?? [];
            }    
            return AsyncHelper.RunSync(() => FindByAnyAsync<T>(filters));
        }

        public async Task<IEnumerable<T>> FindByAnyAsync<T>(Dictionary<string, object> filters) where T : class, new()
        {
            var result = await Service.FindByAnyAsync(typeof(T).Name, filters);
            if (!result.Successful)
                throw new Exception(result.Exception);
            return MemoryPackSerializer.Deserialize<IEnumerable<T>>(result.Data) ?? [];
        }

        public T? Get<T>(long id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            if (OperatingSystem.IsBrowser())
            {
                var request = SimpleProvider.GetService<IXmlHttpRequest>();
                var result = request.Send(nameof(GetAsync), $"type={typeof(T).Name}&id={id}");
                if (!result.Successful) throw new Exception(result.Exception);
                return MemoryPackSerializer.Deserialize<T>(result.Data);
            }
            return AsyncHelper.RunSync(() => GetAsync<T>(id, transaction, commandTimeout));
        }

        public IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            if (OperatingSystem.IsBrowser())
            {
                var request = SimpleProvider.GetService<IXmlHttpRequest>();
                var result = request.Send(nameof(GetAllAsync), $"type={typeof(T).Name}");
                if (!result.Successful) throw new Exception(result.Exception);
                return MemoryPackSerializer.Deserialize<IEnumerable<T>>(result.Data) ?? [];
            }
            return AsyncHelper.RunSync(() => GetAllAsync<T>(transaction, commandTimeout));
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var result = await Service.GetAllAsync(typeof(T).Name);
            if (!result.Successful) throw new Exception(result.Exception);
            return MemoryPackSerializer.Deserialize<IEnumerable<T>>(result.Data) ?? [];
        }

        public async Task<T?> GetAsync<T>(long id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var result = await Service.GetAsync(typeof(T).Name, id);
            if (!result.Successful) throw new Exception(result.Exception);
            return MemoryPackSerializer.Deserialize<T>(result.Data);
        }

        public IEnumerable<T> Where<T>(Dictionary<string, object> filters) where T : class, new()
        {
            if (OperatingSystem.IsBrowser())
            {
                var request = SimpleProvider.GetService<IXmlHttpRequest>();
                var result = request.Send(nameof(WhereAsync), $"type={typeof(T).Name}&filters={JsonSerializer.Serialize(filters)}");
                if (!result.Successful) throw new Exception(result.Exception);
                return MemoryPackSerializer.Deserialize<IEnumerable<T>>(result.Data) ?? [];
            }
            return AsyncHelper.RunSync(() => WhereAsync<T>(filters));
        }

        public async Task<IEnumerable<T>> WhereAsync<T>(Dictionary<string, object> filters) where T : class, new()
        {
            var result = await Service.WhereAsync(typeof(T).Name, filters);
            if (!result.Successful) throw new Exception(result.Exception);
            return MemoryPackSerializer.Deserialize<IEnumerable<T>>(result.Data) ?? [];
        }
    }
}
