using Avae.Abstractions;
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
                var response = request.Send(nameof(FindByAnyAsync), $"filters={JsonSerializer.Serialize(filters)}");
                return MemoryPackSerializer.Deserialize<IEnumerable<T>>(response);
            }
            return AsyncHelper.RunSync(() => FindByAnyAsync<T>(filters));
        }

        public async Task<IEnumerable<T>> FindByAnyAsync<T>(Dictionary<string, object> filters) where T : class, new()
        {
            var bytes = await Service.FindByAnyAsync(typeof(T).Name, filters);
            return MemoryPackSerializer.Deserialize<IEnumerable<T>>(bytes);
        }

        public T Get<T>(long id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            if (OperatingSystem.IsBrowser())
            {
                var request = SimpleProvider.GetService<IXmlHttpRequest>();
                var response = request.Send(nameof(GetAsync), $"type={typeof(T).Name}&id={id}");
                return MemoryPackSerializer.Deserialize<T>(response);
            }
            return AsyncHelper.RunSync(() => GetAsync<T>(id, transaction, commandTimeout));
        }

        public IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            if (OperatingSystem.IsBrowser())
            {
                var request = SimpleProvider.GetService<IXmlHttpRequest>();
                var response = request.Send(nameof(GetAllAsync), $"type={typeof(T).Name}");
                return MemoryPackSerializer.Deserialize<IEnumerable<T>>(response);
            }
            return AsyncHelper.RunSync(() => GetAllAsync<T>(transaction, commandTimeout));
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var bytes = await Service.GetAllAsync(typeof(T).Name);
            return MemoryPackSerializer.Deserialize<IEnumerable<T>>(bytes);
        }

        public async Task<T> GetAsync<T>(long id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var bytes = await Service.GetAsync(typeof(T).Name, id);
            return MemoryPackSerializer.Deserialize<T>(bytes);
        }

        public IEnumerable<T> Where<T>(Dictionary<string, object> filters) where T : class, new()
        {
            if (OperatingSystem.IsBrowser())
            {
                var request = SimpleProvider.GetService<IXmlHttpRequest>();
                var response = request.Send(nameof(WhereAsync), $"filters={JsonSerializer.Serialize(filters)}");
                return MemoryPackSerializer.Deserialize<IEnumerable<T>>(response);
            }
            return AsyncHelper.RunSync(() => WhereAsync<T>(filters));
        }

        public async Task<IEnumerable<T>> WhereAsync<T>(Dictionary<string, object> filters) where T : class, new()
        {
            var bytes = await Service.WhereAsync(typeof(T).Name, filters);
            return MemoryPackSerializer.Deserialize<IEnumerable<T>>(bytes);
        }
    }
}
