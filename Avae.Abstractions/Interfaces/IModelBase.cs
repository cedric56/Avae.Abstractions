using MemoryPack;
using MessagePack;
using System.Data;
using System.Runtime.CompilerServices;

namespace Avae.Abstractions;

public interface IModelBase
{
    
}

//[MemoryPackable]


public interface IDataAccessLayer
{
    //Task<Result> DbTransSave(IDbModelBase modelBase);
    //Task<Result> DbTransRemove(IDbModelBase modelBase);

    T Get<T>(int id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new();

    Task<T> GetAsync<T>(int id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new();

    IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new();

    Task<IEnumerable<T>> GetAllAsync<T>(IDbTransaction? transaction = null, int? commandTimeout = null) where T : class, new();

    Task<IEnumerable<T>> FindByAnyAsync<T>(object filters) where T : class, new();

    IEnumerable<T> FindByAny<T>(object filters) where T : class, new();

    Task<IEnumerable<T>> WhereAsync<T>(object filters) where T : class, new();

    IEnumerable<T> Where<T>(object filters) where T : class, new();
}
