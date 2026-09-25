namespace Example.Models;

public interface IPersonGraph
{
    void AttachContacts(Person person, IEnumerable<Contact> contacts);
}

public sealed class PersonGraph(IEntityCache<Person> cache) : IPersonGraph
{
    public void AttachContacts(Person person, IEnumerable<Contact> contacts)
    {
        foreach (var contact in contacts)
        {
            var other = cache.Find(contact.IdPerson);
            if (other != null)
                contact.Person = other;
            contact.PersonContact = person;
        }
    }
}
