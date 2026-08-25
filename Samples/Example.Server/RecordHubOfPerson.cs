using Avae.Server;
using Example.Models;

namespace Example.Server;

public class RecordHubOfPerson : RecordHub<Person>
{
    public RecordHubOfPerson(RecordHubRepository<Person> repository) : base(repository)
    {

    }
}
