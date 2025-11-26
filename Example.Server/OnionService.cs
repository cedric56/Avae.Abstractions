using Avae.Abstractions;
using Avae.DAL.Interfaces;
using Example.Models;
using MagicOnion;
using MagicOnion.Server;

namespace Example.Server
{
    public class OnionService : ServiceBase<IDBOnionService>, IDBOnionService
    {
        private static readonly IDBLayer Layer = SimpleProvider.GetService<IDBLayer>();

        static OnionService()
        {
            EntityHandler.Handlers = new Dictionary<string, EntityHandler>()
            {
                 { "Person", new EntityHandler<Person>(Layer) },
                { "Contact", new EntityHandler<Contact>(Layer) }
            };
        }
        
        public async UnaryResult<Result> DbTransRemove(DBModelBase modelBase)
        {
            return await modelBase.DbTransRemove(Layer);
        }

        public async UnaryResult<Result> DbTransSave(DBModelBase modelBase)
        {
            return await modelBase.DbTransSave(Layer);
        }
    }
}
