using Avae.DAL;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Models
{
    public class DBOnionLayer(IServiceProvider provider) : OnionLayer(provider), IDBLayer
    {
        public async Task<Result> DbTransRemove(DBModelBase modelBase)
        {
            var service = provider.GetRequiredService<IDBOnionService>();
            if (!service.IsConnected)
                return new Result()
                {
                    Successful = false,
                    Exception = "Service not connected"
                };
            return await service.DbTransRemove(modelBase);
        }

        public async Task<Result> DbTransSave(DBModelBase modelBase)
        {            
            var service = provider.GetRequiredService<IDBOnionService>();
            if (!service.IsConnected)
                return new Result()
                {
                    Successful = false,
                    Exception = "Service not connected"
                };
            return await service.DbTransSave(modelBase);
        }
    }
}
