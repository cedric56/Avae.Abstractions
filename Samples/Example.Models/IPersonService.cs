using Avae.DAL;

namespace Example.Models;

public interface IPersonService : IDbTransaction<Person>
{
    Task LoadContactsAsync(Person person, CancellationToken ct = default);
}
