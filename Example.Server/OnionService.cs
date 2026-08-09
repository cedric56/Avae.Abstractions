using Avae.DAL;
using Avae.Grpc;
using Avae.Server;
using Example.Models;

namespace Example.Server
{
    public class OnionService : GrpcService
    {
        static UnionMessagePackSerializerOptions options;

        static OnionService()
        {
            options = new UnionMessagePackSerializerOptions(UnionResolver.Instance);

            EntityHandler.Handlers = new Dictionary<string, EntityHandler>()
            {
                 { nameof(Person), new EntityHandler<Person>(Layer) },
                { nameof(Contact), new EntityHandler<Contact>(Layer) }
            };
        }

        protected override UnionMessagePackSerializerOptions? GetOptions(string type)
        {
            if (type == typeof(Person).Name)
                return options;
            return null;
        }
    }
}
