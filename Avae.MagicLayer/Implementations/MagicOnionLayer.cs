using Avae.Abstractions;
using Avae.DAL;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Diagnostics;

namespace Avae.MagicLayer
{
    

    //TODO CommandTimeout on WHERE AND FINDBYANY
    public partial class MagicOnionLayer(IServiceProvider provider) : IDBLayer
    {        
        public async Task<DBResult> Remove(DBTransactional transactional)
        {
            try
            {
                //TODO difference of class 
                IDBLayer.Sessions.TryGetValue(transactional.GetType(), out var sessionId);
                var service = provider.GetRequiredService<IMagicOnionLayer>();
                return await service.Remove(transactional, sessionId);
            }
            catch (Exception ex)
            {
                return new DBResult()
                {
                    Successful = false,
                    Exception = ex.Message
                };
            }
        }

        public async Task<DBResult> Save(DBTransactional transactional)
        {
            try
            {
                //TODO difference of class 
                IDBLayer.Sessions.TryGetValue(transactional.GetType(), out var sessionId);
                var service = provider.GetRequiredService<IMagicOnionLayer>();
                return await service.Save(transactional, sessionId);
            }
            catch (Exception ex)
            {
                return new DBResult()
                {
                    Successful = false,
                    Exception = ex.Message
                };
            }
        }

        public IEnumerable<T> FindByAny<T>(Dictionary<string, object> filters) where T : class, new()
        {
            try
            {
                if (OperatingSystem.IsBrowser())
                {
                    var request = provider.GetRequiredService<IXmlHttpRequest>();
                    var result = request.Send(nameof(FindByAnyAsync), MessagePackSerializer.Serialize(new object[] { typeof(T).Name, filters }));
                    if (result == Array.Empty<byte>()) return [];
                    return MessagePackSerializer.Deserialize<IEnumerable<T>>(result) ?? [];
                }
                return AsyncHelper.RunSync(() => FindByAnyAsync<T>(filters));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return [];
            }
        }

        public async Task<IEnumerable<T>> FindByAnyAsync<T>(Dictionary<string, object> filters) where T : class, new()
        {
            try
            {
                var service = provider.GetRequiredService<IMagicOnionLayer>();
                var result = await service.FindByAnyAsync(typeof(T).Name, filters);
                if (!result.Successful) throw new Exception(result.Exception);
                if (result.Data == Array.Empty<byte>()) return [];
                return MessagePackSerializer.Deserialize<IEnumerable<T>>(result.Data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return [];
            }
        }

        public T? Get<T>(long id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            try
            {
                if (OperatingSystem.IsBrowser())
                {
                    var request = provider.GetRequiredService<IXmlHttpRequest>();
                    var result = request.Send(nameof(GetAsync), MessagePackSerializer.Serialize(new object[] { typeof(T).Name, id, commandTimeout ?? int.MaxValue }));
                    if (result == Array.Empty<byte>()) return null;
                    return MessagePackSerializer.Deserialize<T>(result);
                }
                return AsyncHelper.RunSync(() => GetAsync<T>(id, transaction, commandTimeout));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            try
            {
                if (OperatingSystem.IsBrowser())
                {
                    var request = provider.GetRequiredService<IXmlHttpRequest>();
                    var result = request.Send(nameof(GetAllAsync), MessagePackSerializer.Serialize(new object[] { typeof(T).Name, commandTimeout ?? int.MaxValue }));
                    if (result == Array.Empty<byte>()) return [];
                    return MessagePackSerializer.Deserialize<IEnumerable<T>>(result) ?? [];
                }
                return AsyncHelper.RunSync(() => GetAllAsync<T>(transaction, commandTimeout));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return [];
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            try
            {
                var service = provider.GetRequiredService<IMagicOnionLayer>();
                var result = await service.GetAllAsync(typeof(T).Name);
                if (!result.Successful) throw new Exception(result.Exception);
                if (result.Data == Array.Empty<byte>()) return [];
                return MessagePackSerializer.Deserialize<IEnumerable<T>>(result.Data) ?? [];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return [];
            }
        }

        public async Task<T?> GetAsync<T>(long id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            try
            {
                var service = provider.GetRequiredService<IMagicOnionLayer>();
                var result = await service.GetAsync(typeof(T).Name, id);
                if (!result.Successful) throw new Exception(result.Exception);
                if (result.Data == Array.Empty<byte>()) return null;
                return MessagePackSerializer.Deserialize<T>(result.Data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public IEnumerable<T> Where<T>(Dictionary<string, object> filters) where T : class, new()
        {
            try
            {
                if (OperatingSystem.IsBrowser())
                {
                    var request = provider.GetRequiredService<IXmlHttpRequest>();
                    var result = request.Send(nameof(WhereAsync), MessagePackSerializer.Serialize(new object[] { typeof(T).Name, filters }));
                    if (result == Array.Empty<byte>()) return [];
                    return MessagePackSerializer.Deserialize<IEnumerable<T>>(result) ?? [];
                }
                return AsyncHelper.RunSync(() => WhereAsync<T>(filters));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return [];
            }
        }

        public async Task<IEnumerable<T>> WhereAsync<T>(Dictionary<string, object> filters) where T : class, new()
        {
            try
            {
                var service = provider.GetRequiredService<IMagicOnionLayer>();
                var result = await service.WhereAsync(typeof(T).Name, filters);
                if (!result.Successful) throw new Exception(result.Exception);
                if (result.Data == Array.Empty<byte>()) return [];
                return MessagePackSerializer.Deserialize<IEnumerable<T>>(result.Data) ?? [];
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
                return [];
            }
}
    }
}
