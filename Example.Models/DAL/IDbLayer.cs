using Avae.DAL;

namespace Example.Models
{
    public interface IDbLayer : IDataAccessLayer
    {
        Task<Result> DbTransSave(DBModelBase modelBase);
        Task<Result> DbTransRemove(DBModelBase modelBase);
    }
}
