using Avae.Abstractions;
using Avae.DAL;

namespace Example.Models
{
    public class DBOnionLayer : OnionLayer, IDBLayer
    {
        public IDBOnionService Service = SimpleProvider.GetService<IOnionService>() as IDBOnionService;

        public async Task<Result> DbTransRemove(DBModelBase modelBase)
        {
            return await Service.DbTransRemove(modelBase);
        }

        public async Task<Result> DbTransSave(DBModelBase modelBase)
        {
            return await Service.DbTransSave(modelBase);
        }
    }
}
