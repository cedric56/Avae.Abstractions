using Avae.Abstractions;
using Avae.DAL;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Models
{
    public class DBOnionLayer(IServiceProvider provider) : OnionLayer(provider), IDBLayer
    {
        public async Task<Result> DbTransRemove(DBModelBase modelBase)
        {
            IDBOnionService Service = provider.GetRequiredService<IDBOnionService>();
            return await Service.DbTransRemove(modelBase);
        }

        public async Task<Result> DbTransSave(DBModelBase modelBase)
        {
            IDBOnionService Service = provider.GetRequiredService<IDBOnionService>();
            return await Service.DbTransSave(modelBase);
        }
    }
}
