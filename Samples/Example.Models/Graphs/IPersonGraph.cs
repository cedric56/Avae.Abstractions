namespace Example.Models;

public interface IPersonGraph
{
    void AttachContacts(Person person, IEnumerable<Contact> contacts);
}
