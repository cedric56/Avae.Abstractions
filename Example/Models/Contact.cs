using Avae.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using Dapper.Contrib.Extensions;
using Example.Dal;

namespace Example.Models
{
    //[SQLite.Table(nameof(Contact))]
    [Dapper.Contrib.Extensions.Table(nameof(Contact))]
    public partial class Contact : ObservableObject, IModelBase
    {
        private Person? person;
        private Person? contact;

        [Key]
        public int Id { get; set; }

        [Computed]
        public Person Person
        {
            get { return person ??= DBBase.Instance.Get<Person>(IdPerson); }
            set { person = value; }
        }

        //[MessagePack.Key(1)]
        public int IdPerson { get; set; }

        //[MessagePack.Key(2)]
        public virtual int IdContact { get; set; }


        [Computed]
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
