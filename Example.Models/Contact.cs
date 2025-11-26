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
        public long Id { get; set; }

        [Computed]
        [IgnoreMember]
        [MemoryPackIgnore]
        public Person Person
        {
            get { return person ??= DBBase.Instance.Get<Person>(IdPerson)!; }
            set { SetProperty(ref person, value); }
        }

        [MessagePack.Key(1)]
        public long IdPerson { get; set; }

        [MessagePack.Key(2)]
        public long IdContact { get; set; }

        [Computed]
        [IgnoreMember]
        [MemoryPackIgnore]
        public Person PersonContact
        {
            get { return contact ??= DBBase.Instance.Get<Person>(IdContact)!; }
            set { SetProperty(ref contact, value); }
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
