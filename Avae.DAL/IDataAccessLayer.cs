using Avae.Abstractions;
using System.Data;

namespace Avae.DAL
{
    public interface IDataAccessLayer
    {
        IDbConnection DbConnection();

        Task OpenAsync(IDbConnection connection);

        IDbTransaction BeginTransaction(IDbConnection connection);

        long Insert<T>(T entity, IDbConnection db, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, IModelBase, new();

        bool Update<T>(T entity, IDbConnection db, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, IModelBase, new();

        bool Delete<T>(T entity, IDbConnection db, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, IModelBase, new();

        bool DeleteAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, IModelBase, new();

        T Get<T>(int id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new();

        Task<T> GetAsync<T>(int id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new();

        IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new();

        Task<IEnumerable<T>> GetAllAsync<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new();
    }
}
