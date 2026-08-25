using Avae.Core;
using Avae.DAL;
using Avae.DAL.gRPC;
using Avae.Server;
using Example.Models;

namespace Example.Server;

public class ExampleLayerService : MagicOnionService
{
    static DBTransactionalSerializerOptions options;

    static ExampleLayerService()
    {
        var layer = ServiceLocator.GetRequiredService<IDBLayer>();

        options = new DBTransactionalSerializerOptions(DBTransactionalResolver.Instance);

        EntityHandler.Handlers = new Dictionary<string, EntityHandler>()
        {
             { nameof(Person), new EntityHandler<Person>(layer) },
             { nameof(Contact), new EntityHandler<Contact>(layer) }
        };
    }

    protected override DBTransactionalSerializerOptions? GetOptions(string type)
    {
        if (type == typeof(Person).Name)
            return options;
        return null;
    }
}
