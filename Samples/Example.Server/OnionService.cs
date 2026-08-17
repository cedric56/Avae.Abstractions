using Avae.Abstractions;
using Avae.DAL;
using Avae.MagicLayer;
using Avae.Server;
using Example.Models;

namespace Example.Server;

public class OnionService : MagicOnionService
{
    static UnionMessagePackSerializerOptions options;

    static OnionService()
    {
        var layer = ServiceLocator.GetRequiredService<IDBLayer>();

        options = new UnionMessagePackSerializerOptions(UnionResolver.Instance);

        EntityHandler.Handlers = new Dictionary<string, EntityHandler>()
        {
             { nameof(Person), new EntityHandler<Person>(layer) },
             { nameof(Contact), new EntityHandler<Contact>(layer) }
        };
    }

    protected override UnionMessagePackSerializerOptions? GetOptions(string type)
    {
        if (type == typeof(Person).Name)
            return options;
        return null;
    }
}
