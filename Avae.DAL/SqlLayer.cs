using Avae.Abstractions;
using Dapper.Contrib.Extensions;
using System.Data;
using System.Data.Common;

namespace Avae.DAL
{
    public class SqlLayer : IDataAccessLayer
    {
        public IDbTransaction BeginTransaction(IDbConnection connection)
        {
            return connection.BeginTransaction();
        }

        public IDbConnection DbConnection()
        {
            return SimpleProvider.GetService<DbConnection>();
        }

        public T Get<T>(int id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using var db = DbConnection();
            return db.Get<T>(id, transaction, commandTimeout);
        }

        public IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using var db = DbConnection();
            return db.GetAll<T>(transaction, commandTimeout);
        }

        public Task<IEnumerable<T>> GetAllAsync<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using var db = DbConnection();
            return db.GetAllAsync<T>(transaction, commandTimeout);
        }

        public Task<T> GetAsync<T>(int id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using var db = DbConnection();
            return db.GetAsync<T>(id, transaction, commandTimeout);
        }

        public Task OpenAsync(IDbConnection connection)
        {
            if (connection is DbConnection db)
                return db.OpenAsync();

            throw new NotImplementedException();
        }

        bool IDataAccessLayer.Delete<T>(T entity, IDbConnection db, IDbTransaction? transaction, int? commandTimeout)
        {
            return entity.Delete(db, transaction, commandTimeout);
        }

        bool IDataAccessLayer.DeleteAll<T>(IDbTransaction? transaction, int? commandTimeout)
        {
            using var db = DbConnection();
            return db.DeleteAll<T>(transaction, commandTimeout);
        }

        long IDataAccessLayer.Insert<T>(T entity, IDbConnection db, IDbTransaction? transaction, int? commandTimeout)
        {
            return entity.Insert(db, transaction, commandTimeout);
        }

        bool IDataAccessLayer.Update<T>(T entity, IDbConnection db, IDbTransaction? transaction, int? commandTimeout)
        {
            return entity.Update(db, transaction, commandTimeout);
        }
    }
}
