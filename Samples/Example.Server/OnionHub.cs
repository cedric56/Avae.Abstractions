using Avae.Server;
using Example.Models;

namespace Example.Server;

public class OnionHub : RecordHub<Person>
{
    public OnionHub(RecordHubRepository<Person> repository) : base(repository)
    {

    }
}
