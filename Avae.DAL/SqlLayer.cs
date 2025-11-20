using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;

namespace Avae.DAL
{
    public class SqlLayer : IDataAccessLayer
    {
        public T Get<T>(int id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using var db = new LoggedConnection();
            return db.Get<T>(id, transaction, commandTimeout);
        }

        public IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using var db = new LoggedConnection();
            return db.GetAll<T>(transaction, commandTimeout);
        }

        public Task<IEnumerable<T>> GetAllAsync<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using var db = new LoggedConnection();
            return db.GetAllAsync<T>(transaction, commandTimeout);
        }

        public Task<T> GetAsync<T>(int id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using var db = new LoggedConnection();
            return db.GetAsync<T>(id, transaction, commandTimeout);
        }

        private string Create<T>(Dictionary<string, object> filters, string condition, out DynamicParameters parameters)
        {
            var conditions = new List<string>();
            parameters = new DynamicParameters();

            foreach (var pair in filters)
            {
                // Always add parameter
                parameters.Add(pair.Key, pair.Value);

                // Build: (@Param IS NOT NULL AND Column = @Param)
                conditions.Add(
                    $"(@{pair.Key} IS NOT NULL AND {pair.Key} = @{pair.Key})"
                );
            }

            // Join all parts
            string where = string.Join(condition, conditions);

            return $"SELECT * FROM {typeof(T).Name} WHERE {where}";
        }

        public Task<IEnumerable<T>> FindByAnyAsync<T>(Dictionary<string, object> filters) where T : class, new()
        {
            var sql = Create<T>(filters, " OR ", out var parameters);
            using var db = new LoggedConnection();
            return db.QueryAsync<T>(sql, parameters);
        }

        public IEnumerable<T> FindByAny<T>(Dictionary<string, object> filters) where T : class, new()
        {
            var sql = Create<T>(filters, " OR ", out var parameters);
            using var db = new LoggedConnection();
            return db.Query<T>(sql, parameters);
        }

        public Task<IEnumerable<T>> WhereAsync<T>(Dictionary<string, object> filters) where T : class, new()
        {
            var sql = Create<T>(filters, " AND ", out var parameters);
            using var db = new LoggedConnection();
            return db.QueryAsync<T>(sql, parameters);
        }

        public IEnumerable<T> Where<T>(Dictionary<string, object> filters) where T : class, new()
        {
            var sql = Create<T>(filters, " AND ", out var parameters);
            using var db = new LoggedConnection();
            return db.Query<T>(sql, parameters);
        }
    }
}
