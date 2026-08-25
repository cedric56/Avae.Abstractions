using Avae.DAL;
using MessagePack;
using MessagePack.Formatters;
using System.Text;

namespace Example.Models.MessagePackFormatters;

public class PersonFormatter : IMessagePackFormatter<Person?>, IDBTransactionalFormatter//, IMessagePackFormatter<DBTransactional?>
{
    public void Serialize(ref MessagePackWriter writer, Person? value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNil();
            return;
        }

        if (options is DBTransactionalSerializerOptions)
        {
            //Client input => When we call server it sends only without contacts
            SetPersonFields(ref writer, 3);
        }
        else
        {
            //Server output => When we send to server we send contacts
            SetPersonFields(ref writer, 4);

            var contacts = value.Contacts;
            writer.WriteArrayHeader(contacts.Count);
            foreach (var contact in contacts)
            {
                SetContactFields(ref writer, contact);
            }
        }

        void SetPersonFields(ref MessagePackWriter writer, int headers)
        {
            writer.WriteArrayHeader(headers);
            writer.WriteInt64(value.Id);
            writer.WriteString(Encoding.UTF8.GetBytes(value.FirstName ?? string.Empty));
            writer.WriteString(Encoding.UTF8.GetBytes(value.LastName ?? string.Empty));
        }

        void SetContactFields(ref MessagePackWriter writer, Contact contact)
        {
            writer.WriteInt64(contact.Id);
            writer.WriteInt64(contact.IdPerson);
            writer.WriteInt64(contact.IdContact);
        }
    }

    public Person? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
            return null;

        // Check if it's an array
        if (reader.NextMessagePackType != MessagePackType.Array)
            throw new MessagePackSerializationException($"Invalid Person format: expected Array, got {reader.NextMessagePackType}");

        var count = reader.ReadArrayHeader();
        if (count == 3)
        {
            //Client input => When we call we only read without contacts
            return new Person
            {
                Id = reader.ReadInt64(),
                FirstName = reader.ReadString() ?? string.Empty,
                LastName = reader.ReadString() ?? string.Empty
            };
        }
        else if (count == 4)
        {
            //Server input => When client call IDBLayer.Save contacts are sent also
            var person = new Person
            {
                Id = reader.ReadInt64(),
                FirstName = reader.ReadString() ?? string.Empty,
                LastName = reader.ReadString() ?? string.Empty
            };
            var contacts = new List<Contact>();
            count = reader.ReadArrayHeader();
            for (int i = 0; i < count; i++)
            {
                contacts.Add(new Contact()
                {
                    Id = reader.ReadInt64(),
                    IdPerson = reader.ReadInt64(),
                    IdContact = reader.ReadInt64()
                });
            }
            person.Contacts = contacts;
            return person;
        }

        throw new MessagePackSerializationException($"Invalid Person format: expected count = 3, got {count}");
    }

    public void Serialize(ref MessagePackWriter writer, object? value, MessagePackSerializerOptions options)
    {
        Serialize(ref writer, value as Person, options);
    }

    object? IDBTransactionalFormatter.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        return Deserialize(ref reader, options);
    }
}
