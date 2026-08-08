using Avae.Abstractions;
using Avae.DAL;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using System.Runtime.CompilerServices;
using System.Text;
using static CommunityToolkit.Mvvm.ComponentModel.__Internals.__TaskExtensions.TaskAwaitableWithoutEndValidation;

namespace Example.Models
{

    public class PersonOptions(MessagePackSerializerOptions options) : MessagePackSerializerOptions(options)
    {

    }

    public class PersonFormatter : IMessagePackFormatter<Person?>, IMessagePackFormatter<DBModelBase?>
    {
        public void Serialize(ref MessagePackWriter writer, Person? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            if (options is PersonOptions)
            {
                SetPersonFields(ref writer, 3);
            }
            else
            {
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
                writer.WriteString(Encoding.UTF8.GetBytes(value.FirstName ?? string.Empty));
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
            if(count == 3)
            {
                return new Person
                {
                    Id = reader.ReadInt64(),
                    FirstName = reader.ReadString() ?? string.Empty,
                    LastName = reader.ReadString() ?? string.Empty
                };
            }
            else if(count == 4)
            {
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

        public void Serialize(ref MessagePackWriter writer, DBModelBase? value, MessagePackSerializerOptions options)
        {
            if (value is Person person)
                Serialize(ref writer, person, options);
            else
                writer.WriteNil();
        }

        DBModelBase? IMessagePackFormatter<DBModelBase?>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return Deserialize(ref reader, options);
        }
    }

    public class CustomResolver : IFormatterResolver
    {
        public static readonly CustomResolver Instance = new CustomResolver();
        private readonly PersonFormatter _personFormatter = new PersonFormatter();


        private CustomResolver() { }

        public IMessagePackFormatter<T>? GetFormatter<T>()
        {
            // ✅ This single line handles ALL types your formatter supports
            if (typeof(T) == typeof(Person) ||
                typeof(T) == typeof(DBModelBase))
            {
                return (IMessagePackFormatter<T>)_personFormatter;
            }

            //if (typeof(T) == typeof(IEnumerable<Person>) || typeof(T) == typeof(List<Person>))
            //    return StandardResolver.Instance.GetFormatter<T>();

            return StandardResolver.Instance.GetFormatter<T>();
        }
    }

    public static class ModelInitialization
    {
#pragma warning disable CA2255 // L’attribut ’ModuleInitializer’ ne doit pas être utilisé dans les bibliothèques
        [ModuleInitializer]
#pragma warning restore CA2255 // L’attribut ’ModuleInitializer’ ne doit pas être utilisé dans les bibliothèques
        public static void Init()
        {
            InputValidation<Person>.Init();

            var resolver = CompositeResolver.Create(CustomResolver.Instance);
            MessagePackSerializer.DefaultOptions = MessagePackSerializer.DefaultOptions.WithResolver(resolver);


        }
    }
}
