using Avae.DAL;

namespace Example.Models;

public sealed class PersonServiceRemote(MagicOnionLayer layer) : IPersonService
{
    public async Task LoadContactsAsync(Person person, CancellationToken ct = default)
    {
        if (person.Id == 0)
        {
            person.Contacts = [];
            return;
        }
        var contacts = await layer.FindByAnyAsync<Contact>(
            new Dictionary<string, object>()
            {
            { nameof(Contact.IdContact), person.Id }
            });
        person.Contacts = [.. contacts];
    }

    public Task<DBResult> SaveAsync(Person person, CancellationToken ct = default)
        => layer.SaveAsync(person);

    public Task<DBResult> RemoveAsync(Person person, CancellationToken ct = default)
        => layer.RemoveAsync(person);
}
