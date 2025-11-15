using Avae.Abstractions;
using Dapper.Contrib.Extensions;
using System.Data;
using System.Data.Common;

namespace Avae.DAL;

public static class DapperExtensions
{
    public static long Insert<T>(this T entity, int? commandTimeout = null) where T : class, IModelBase
    {
        using var db = SimpleProvider.GetService<DbConnection>();
        return entity.Insert(db, commandTimeout: commandTimeout);
    }

    public static long Insert<T>(this T entity, IDbConnection db, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, IModelBase
    {
        return db.Insert(entity, transaction, commandTimeout);
    }

    public static bool Update<T>(this T entity, int? commandTimeout = null) where T : class, IModelBase
    {
        using var db = SimpleProvider.GetService<DbConnection>();
        return entity.Update(db, commandTimeout: commandTimeout);
    }

    public static bool Update<T>(this T entity, IDbConnection db, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, IModelBase
    {
        return db.Update(entity, transaction, commandTimeout);
    }

    public static bool Delete<T>(this T entity, int? commandTimeout = null) where T : class, IModelBase
    {
        using var db = SimpleProvider.GetService<DbConnection>();
        return entity.Delete(db, commandTimeout: commandTimeout);
    }

    public static bool Delete<T>(this T entity, IDbConnection db, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, IModelBase
    {
        return db.Delete(entity, transaction, commandTimeout);
    }

    public static Task<bool> DeleteAsync<T>(this T entity, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, IModelBase
    {
        using var db = SimpleProvider.GetService<DbConnection>();
        return db.DeleteAsync(entity, transaction, commandTimeout);
    }

    public static bool DeleteAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class
    {
        using var db = SimpleProvider.GetService<DbConnection>();
        return db.DeleteAll<T>(transaction, commandTimeout);
    }
}
