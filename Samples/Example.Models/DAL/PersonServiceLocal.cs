using Avae.DAL;
using Dommel;
using System.Data;

namespace Example.Models;

public sealed class PersonServiceLocal(
//DbProviderFactory factory, 
IDBFactory factory,
IPersonGraph graph) : IPersonService
{
    public async Task LoadContactsAsync(Person person, CancellationToken ct = default)
    {
        if (person.Id == 0)
        {
            person.Contacts = [];
            return;
        }

        using var connection = factory.CreateConnection()!;
        connection.Open();

        var contacts = (await connection.SelectAsync<Contact>(c => c.IdContact == person.Id).ConfigureAwait(false)).ToList();
        graph.AttachContacts(person, contacts);
        person.Contacts = contacts;
    }

    public Task<DBResult> SaveAsync(Person person, CancellationToken ct = default)
    {
        return DbTransaction.RunAsync(factory, Save);
        //.WithCancellation(ct);

        async Task Save(IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            var before = (await dbConnection.SelectAsync<Contact>(c => c.IdContact == person.Id, dbTransaction)).ToList();

            //ChildSync.Sync(dbConnection, dbTransaction, before, person.Contacts,
            //    contact =>
            //    {
            //        contact.IdContact = person.Id;
            //        return contact.Id == 0;
            //    },
            //    (first, second) => first.IdPerson != second.IdPerson);

            if (person.Id == 0)
                dbConnection.Insert(person, dbTransaction);
            else
                dbConnection.Update(person, dbTransaction);


            foreach (var contact in person.Contacts)
            {
                contact.IdContact = person.Id;
                if (contact.Id == 0)
                    dbConnection.Insert(contact, dbTransaction);
                else
                    dbConnection.Update(contact, dbTransaction);
            }

            foreach (var old in before.Where(c => person.Contacts.All(p => p.IdPerson != c.IdPerson)))
                dbConnection.Delete(old, dbTransaction);
        }
    }

    public Task<DBResult> RemoveAsync(Person person, CancellationToken ct = default)
    {
        return DbTransaction.RunAsync(factory, Remove);

        async Task Remove(IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            var contacts = person.Contacts.Count > 0
                 ? person.Contacts
                 : (await dbConnection.SelectAsync<Contact>(c => c.IdContact == person.Id, dbTransaction)).ToList();

            foreach (var contact in contacts)
                dbConnection.Delete(contact, dbTransaction);

            dbConnection.Delete(person, dbTransaction);
        }
    }
}
