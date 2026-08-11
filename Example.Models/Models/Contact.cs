using Avae.Abstractions;
using Avae.DAL;
using Dapper.Contrib.Extensions;
using MessagePack;
using System.ComponentModel;

namespace Example.Models
{
    [Table(nameof(Contact))]
    [MessagePackObject]
    public partial class Contact : INotifyPropertyChanged, IModelBase
    {
        private Person? person;
        private Person? contact;

        [Dapper.Contrib.Extensions.Key]
        [MessagePack.Key(0)]
        public long Id { get; set; }

        [Computed]
        [IgnoreMember]
        public Person Person
        {
            get { return person ??= DBBase.Instance.Get<Person>(IdPerson)!; }
            set {
                person = value;
                OnPropertyChanged(nameof(Person));
            }
        }

        [MessagePack.Key(1)]
        public long IdPerson { get; set; }

        [MessagePack.Key(2)]
        public long IdContact { get; set; }

        [Computed]
        [IgnoreMember]
        public Person PersonContact
        {
            get { return contact ??= DBBase.Instance.Get<Person>(IdContact)!; }
            set 
            {
                contact = value;
                OnPropertyChanged(nameof(PersonContact));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public override bool Equals(object? obj)
        {
            return obj is Contact contact && contact.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
