using Avae.Abstractions;
using System.Data;

namespace Avae.DAL
{        
    public abstract class DataAccessLayerBase : IDataAccessLayer
    {
        public static IDataAccessLayer Instance { get; } = SimpleProvider.GetService<IDataAccessLayer>();

        public IDbConnection DbConnection()
        {
            return Instance.DbConnection();
        }

        public Task OpenAsync(IDbConnection connection)
        {
            return Instance.OpenAsync(connection);
        }

        public IDbTransaction BeginTransaction(IDbConnection connection)
        {
            return Instance.BeginTransaction(connection);
        }

        public long Insert<T>(T entity, IDbConnection db, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, IModelBase, new()
        {
            return Instance.Insert<T>(entity, db, transaction, commandTimeout);
        }

        public bool Update<T>(T entity, IDbConnection db, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, IModelBase, new()
        {
            return Instance.Update<T>(entity, db, transaction, commandTimeout);
        }

        public bool Delete<T>(T entity, IDbConnection db, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, IModelBase, new()
        {
            return Instance.Delete<T>(entity, db, transaction, commandTimeout);
        }

        public bool DeleteAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, IModelBase, new()
        {
            return Instance.DeleteAll<T>(transaction, commandTimeout);
        }

        public T Get<T>(int id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            return Instance.Get<T>(id, transaction, commandTimeout);
        }

        public Task<T> GetAsync<T>(int id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            return Instance.GetAsync<T>(id, transaction, commandTimeout);
        }

        public IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            return Instance.GetAll<T>(transaction, commandTimeout);
        }

        public Task<IEnumerable<T>> GetAllAsync<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            return Instance.GetAllAsync<T>(transaction, commandTimeout);
        }
    }
}
