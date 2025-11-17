using Avae.DAL;

namespace Example.Models
{
    public interface IDbLayer : IDataAccessLayer
    {
        Task<Result> DbTransSave(DbModelBase modelBase);
        Task<Result> DbTransRemove(DbModelBase modelBase);
    }
}
