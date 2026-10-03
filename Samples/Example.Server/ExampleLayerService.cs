using Avae.DAL;
using Example.Models;

namespace Example.Server;

public class ExampleLayerService : MagicOnionService
{
    public ExampleLayerService(IDBFactory factory, IServiceProvider provider)
        : base(factory, new EntityHandlerRegistry(new Dictionary<string, EntityHandler>()
        {
            { nameof(Person), new EntityHandler<Person>(factory, provider.GetRequiredService<IDbTransaction<Person>>()) },
            { nameof(Contact), new EntityHandler<Contact>(factory) }
        }))
    {

    }
}
