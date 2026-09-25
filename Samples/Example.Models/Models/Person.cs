//using Dapper.Contrib.Extensions;
using MessagePack;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Example.Models;

//[Dapper.Contrib.Extensions.Table(nameof(Person))]
[System.ComponentModel.DataAnnotations.Schema.Table(nameof(Person))]
[MessagePackObject]
public partial class Person : INotifyPropertyChanged, IDataErrorInfo
{
    private string? _firstName;
    private string? _lastName;

    public event PropertyChangedEventHandler? PropertyChanged;

    //[Dapper.Contrib.Extensions.Key]
    [MessagePack.Key(0)]
    public long Id { get; set; }

    [Required(ErrorMessage = "FirstName must be set")]
    [MessagePack.Key(1)]
    public string? FirstName
    {
        get => _firstName;
        set { _firstName = value; OnPropertyChanged(nameof(FirstName)); OnPropertyChanged(nameof(FullName)); }
    }

    [Required(ErrorMessage = "LastName must be set")]
    [MessagePack.Key(2)]
    public string? LastName
    {
        get => _lastName;
        set { _lastName = value; OnPropertyChanged(nameof(LastName)); OnPropertyChanged(nameof(FullName)); }
    }

    //[Computed]
    [MessagePack.Key(3)]
    //[IgnoreMember]
    public IList<Contact> Contacts { get; set; } = [];

    public override bool Equals(object? obj)
    {
        if (obj is not Person other) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id != 0 && other.Id != 0 && Id == other.Id;
    }

    public override int GetHashCode() => Id != 0 ? Id.GetHashCode() : RuntimeHelpers.GetHashCode(this);

    //[Computed]
    [IgnoreMember]
    [NotMapped]
    public string Error => EntityValidator.Error(this) ?? string.Empty;

    //[Computed]
    [IgnoreMember]
    [NotMapped]
    public string this[string columnName] => EntityValidator.ValidateProperty(this, columnName) ?? string.Empty;

    //[Computed]
    [IgnoreMember]
    [NotMapped]
    public string? FullName => FirstName + " " + LastName;

    [IgnoreMember]
    [NotMapped]
    public bool IsValid => string.IsNullOrEmpty(Error);

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
