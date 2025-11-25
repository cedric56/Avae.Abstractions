using Avae.DAL.Interfaces;

namespace Example.Models
{
    public interface IDBLayer : IDataAccessLayer
    {
        Task<Result> DbTransSave(DBModelBase modelBase);
        Task<Result> DbTransRemove(DBModelBase modelBase);
    }
}
