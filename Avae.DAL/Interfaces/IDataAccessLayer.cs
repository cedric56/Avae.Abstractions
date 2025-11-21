using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avae.DAL
{
    public interface IDataAccessLayer
    {
        T Get<T>(long id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new();

        Task<T> GetAsync<T>(long id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new();

        IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new();

        Task<IEnumerable<T>> GetAllAsync<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new();

        Task<IEnumerable<T>> FindByAnyAsync<T>(Dictionary<string, object> filters) where T : class, new();

        Task<IEnumerable<T>> FindByAnyAsync<T>(params (string key, object value)[] filters) where T : class, new()
        {
            return FindByAnyAsync<T>(filters.ToDictionary(x => x.key, y => y.value));
        }

        IEnumerable<T> FindByAny<T>(Dictionary<string, object> filters) where T : class, new();

        IEnumerable<T> FindByAny<T>(params (string key, object value)[] filters) where T : class, new()
        {
            return FindByAny<T>(filters.ToDictionary(x => x.key, y => y.value));
        }

        Task<IEnumerable<T>> WhereAsync<T>(Dictionary<string, object> filters) where T : class, new();

        Task<IEnumerable<T>> WhereAsync<T>(params (string key, object value)[] filters) where T : class, new()
        {
            return WhereAsync<T>(filters.ToDictionary(x => x.key, y => y.value));
        }

        IEnumerable<T> Where<T>(Dictionary<string, object> filters) where T : class, new();

        IEnumerable<T> Where<T>(params (string key, object value)[] filters) where T : class, new()
        {
            return Where<T>(filters.ToDictionary(x => x.key, y => y.value));
        }

    }
}
