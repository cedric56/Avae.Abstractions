using Avae.Abstractions;
using MemoryPack;
using System.Data;

namespace Avae.DAL
{
    public partial class OnionLayer : IDataAccessLayer
    {
        private static IOnionService Service => SimpleProvider.GetService<IOnionService>();


        public IEnumerable<T> FindByAny<T>(Dictionary<string, object> filters) where T : class, new()
        {
            return AsyncHelper.RunSync(() => FindByAnyAsync<T>(filters));
        }

        public async Task<IEnumerable<T>> FindByAnyAsync<T>(Dictionary<string, object> filters) where T : class, new()
        {
            var bytes = await Service.FindByAnyAsync(typeof(T).Name, filters);//, transaction, commandTimeout);
            return MemoryPackSerializer.Deserialize<IEnumerable<T>>(bytes);
        }

        public T Get<T>(int id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            if (OperatingSystem.IsBrowser())
            {
                var request = SimpleProvider.GetService<IXmlHttpRequest>();
                var response = request.Send("http://localhost:5001/_/IDbService/GetAsync", $"type={typeof(T).Name}&id={id}");
                return MemoryPackSerializer.Deserialize<T>(response);
            }
            return AsyncHelper.RunSync(() => GetAsync<T>(id, transaction, commandTimeout));
        }

        public IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            if (OperatingSystem.IsBrowser())
            {
                var request = SimpleProvider.GetService<IXmlHttpRequest>();
                var response = request.Send("http://localhost:5001/_/IDbService/GetAllAsync", $"type={typeof(T).Name}");
                return MemoryPackSerializer.Deserialize<IEnumerable<T>>(response);
            }
            return AsyncHelper.RunSync(() => GetAllAsync<T>(transaction, commandTimeout));
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var bytes = await Service.GetAllAsync(typeof(T).Name);//, transaction, commandTimeout);
            return MemoryPackSerializer.Deserialize<IEnumerable<T>>(bytes);
        }

        public Task<T> GetAsync<T>(int id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Where<T>(Dictionary<string, object> filters) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> WhereAsync<T>(Dictionary<string, object> filters) where T : class, new()
        {
            throw new NotImplementedException();
        }
    }
}
