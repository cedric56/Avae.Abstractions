using Avae.Server;
using Example.Models;

namespace Example.Server;

public class OnionHub : MagicRecordHub<Person>
{
    public OnionHub(RecordHubRepository<Person> repository) : base(repository)
    {

    }
}
