using MessagePack;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Example.Models;

[Table(nameof(Contact))]
[MessagePackObject]
public partial class Contact : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    [MessagePack.Key(0)]
    public long Id { get; set; }

    [MessagePack.Key(1)]
    public long IdContact { get; set; }

    [MessagePack.Key(2)]
    public long IdPerson { get; set; }

    private Person? _person;
    private Person? _personContact;

    [IgnoreMember]
    [NotMapped]
    public Person? Person { get => _person; set { _person = value; OnPropertyChanged(nameof(Person)); } }

    [IgnoreMember]
    [NotMapped]
    public Person? PersonContact { get => _personContact; set { _personContact = value; OnPropertyChanged(nameof(PersonContact)); } }

    public override bool Equals(object? obj)
    {
        if (obj is not Contact other) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id != 0 && other.Id != 0 && Id == other.Id;
    }

    public override int GetHashCode() =>
        Id != 0 ? Id.GetHashCode() : System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}