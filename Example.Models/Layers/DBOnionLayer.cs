using Avae.Abstractions;
using Avae.DAL;
using Avae.DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Models
{
    public class DBOnionLayer : OnionLayer, IDBLayer
    {
        IServiceProvider provider;
        public DBOnionLayer(IServiceProvider provider)
            : base(provider)
        {
            this.provider = provider;

            EntityHandler.Handlers = new Dictionary<string, EntityHandler>()
            {
                 { nameof(Person), new EntityHandler<Person>(this) },
                { nameof(Contact), new EntityHandler<Contact>(this) }
            };
        }

        public async Task<Result> DbTransRemove(DBModelBase modelBase)
        {
            var service = provider.GetRequiredService<IDBOnionService>();
            if (service is IOnionNotConnected)
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
            if (service is IOnionNotConnected)
                return new Result()
                {
                    Successful = false,
                    Exception = "Service not connected"
                };
            return await service.DbTransSave(modelBase);
        }
    }
}
