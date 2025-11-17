using Avae.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using Dapper.Contrib.Extensions;
using MemoryPack;
using MessagePack;

namespace Example.Models
{
    [Table(nameof(Contact))]
    [MessagePackObject]
    [MemoryPackable]
    public partial class Contact : ObservableObject, IModelBase
    {
        private Person? person;
        private Person? contact;

        [Dapper.Contrib.Extensions.Key]
        [MessagePack.Key(0)]
        public int Id { get; set; }

        [Computed]
        [IgnoreMember]
        [MemoryPackIgnore]
        public Person Person
        {
            get { return person ??= DBBase.Instance.Get<Person>(IdPerson); }
            set { person = value; }
        }

        [MessagePack.Key(1)]
        public int IdPerson { get; set; }
        [MessagePack.Key(2)]
        public int IdContact { get; set; }

        [Computed]
        [IgnoreMember]
        [MemoryPackIgnore]
        public Person PersonContact
        {
            get { return contact ??= DBBase.Instance.Get<Person>(IdContact); }
            set { contact = value; }
        }

        public override bool Equals(object? obj)
        {
            return obj is Contact contact && contact.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
