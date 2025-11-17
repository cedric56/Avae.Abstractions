using Avae.Abstractions;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;
using System.Data.Common;

namespace Avae.DAL
{
    public class SqlLayer : IDataAccessLayer
    {
        public T Get<T>(int id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using var db = SimpleProvider.GetService<DbConnection>();
            return db.Get<T>(id, transaction, commandTimeout);
        }

        public IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using var db = SimpleProvider.GetService<DbConnection>();
            return db.GetAll<T>(transaction, commandTimeout);
        }

        public Task<IEnumerable<T>> GetAllAsync<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using var db = SimpleProvider.GetService<DbConnection>();
            return db.GetAllAsync<T>(transaction, commandTimeout);
        }

        public Task<T> GetAsync<T>(int id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {            
            using var db = SimpleProvider.GetService<DbConnection>();
            return db.GetAsync<T>(id, transaction, commandTimeout);
        }

        private string Create<T>(object filters, string condition, out DynamicParameters parameters)
        {
            var props = filters.GetType().GetProperties();

            var conditions = new List<string>();
            parameters = new DynamicParameters();

            foreach (var prop in props)
            {
                var value = prop.GetValue(filters);

                // Always add parameter
                parameters.Add($"@{prop.Name}", value);

                // Build: (@Param IS NOT NULL AND Column = @Param)
                conditions.Add(
                    $"(@{prop.Name} IS NOT NULL AND {prop.Name} = @{prop.Name})"
                );
            }

            // Join all parts
            string where = string.Join(condition, conditions);

            return $"SELECT * FROM {typeof(T).Name} WHERE {where}";
        }

        public Task<IEnumerable<T>> FindByAnyAsync<T>(object filters) where T : class, new()
        {
            var sql = Create<T>(filters, " OR ", out var parameters);
            using var db = SimpleProvider.GetService<DbConnection>();
            return db.QueryAsync<T>(sql, parameters);
        }

        public IEnumerable<T> FindByAny<T>(object filters) where T : class, new()
        {
            var sql = Create<T>(filters, " OR ", out var parameters);
            using var db = SimpleProvider.GetService<DbConnection>();
            return db.Query<T>(sql, parameters);
        }

        public Task<IEnumerable<T>> WhereAsync<T>(object filters) where T : class, new()
        {
            var sql = Create<T>(filters, " AND ", out var parameters);
            using var db = SimpleProvider.GetService<DbConnection>();
            return db.QueryAsync<T>(sql, parameters);
        }

        public IEnumerable<T> Where<T>(object filters) where T : class, new()
        {
            var sql = Create<T>(filters, " AND ", out var parameters);
            using var db = SimpleProvider.GetService<DbConnection>();
            return db.Query<T>(sql, parameters);
        }

        //public Task<Result> DbTransSave(IDbModelBase modelBase)
        //{
        //    return modelBase.DbTransSave(this);
        //}

        //public Task<Result> DbTransRemove(IDbModelBase modelBase)
        //{
        //    return modelBase.DbTransRemove(this);
        //}
    }
}
