using Avae.DAL;
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
            try
            {
                var service = provider.GetRequiredService<IDBOnionService>();
                return await service.DbTransRemove(modelBase);
            }
            catch (Exception ex)
            {
                return new Result()
                {
                    Successful = false,
                    Exception = ex.Message
                };
            }
        }

        public async Task<Result> DbTransSave(DBModelBase modelBase)
        {
            try
            {
                var service = provider.GetRequiredService<IDBOnionService>();
                return await service.DbTransSave(modelBase);
            }
            catch (Exception ex)
            {
                return new Result()
                {
                    Successful = false,
                    Exception = ex.Message
                };
            }
        }
    }
}
