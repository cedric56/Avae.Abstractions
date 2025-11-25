using Avae.Abstractions;
using Avae.DAL;

namespace Example.Models
{
    public class DBOnionLayer : OnionLayer, IDBLayer
    {
        public async Task<Result> DbTransRemove(DBModelBase modelBase)
        {
            IDBOnionService Service = SimpleProvider.GetService<IDBOnionService>();
            return await Service.DbTransRemove(modelBase);
        }

        public async Task<Result> DbTransSave(DBModelBase modelBase)
        {
            IDBOnionService Service = SimpleProvider.GetService<IDBOnionService>();
            return await Service.DbTransSave(modelBase);
        }
    }
}
