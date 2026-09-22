using Avae.DAL;
using Dapper.Contrib.Extensions;
using MessagePack;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Runtime.CompilerServices;

namespace Example.Models;

[Table(nameof(Person))]
//[MessagePackObject]    
public partial class Person : DBTransactional, INotifyPropertyChanged, IDataErrorInfo
{
    private IList<Contact>? _contacts;
    private string? _firstName;
    private string? _lastName;

    public event PropertyChangedEventHandler? PropertyChanged;

    [Dapper.Contrib.Extensions.Key]
    public long Id { get; set; }

    [Required(ErrorMessage = "FirstName must be set")]
    public string? FirstName
    {
        get => _firstName;
        set
        {
            _firstName = value;
            OnPropertyChanged(nameof(FirstName));
            OnPropertyChanged(nameof(FullName));
        }
    }

    [Required(ErrorMessage = "LastName must be set")]
    public string? LastName
    {
        get => _lastName;
        set
        {
            _lastName = value;
            OnPropertyChanged(nameof(LastName));
            OnPropertyChanged(nameof(FullName));
        }
    }

    [Computed]
    public IList<Contact> Contacts
    {
        get
        {
            if (_contacts == null)
            {
                if (Id == 0)
                    _contacts = [];
                else
                {
                    var contacts = DBBase.Instance.FindByAny<Contact>((nameof(Contact.IdContact), Id));
                    AvoidReadings(contacts);
                    _contacts = [.. contacts];
                }
            }
            return _contacts;
        }
        //private this must be avoid but necessary now
        set
        {
            _contacts = value;
        }
    }

    private void AvoidReadings(IEnumerable<Contact> contacts)
    {
        foreach (var contact in contacts)
        {
            var person = Repository.Instance.Persons
                .FirstOrDefault(p => p.Id == contact.IdPerson);

            if (person != null)
                contact.Person = person;

            contact.PersonContact = this;
        }
    }

    public async Task LoadContactsAsync()
    {
        if (_contacts != null || Id == 0)
            return;

        var contacts = await DBBase.Instance.FindByAnyAsync<Contact>((nameof(Contact.IdContact), Id));

        AvoidReadings(contacts);

        Contacts = [.. contacts];
    }

    public override async Task<DBResult> Save(IDBLayer instance, IDBFactory factory, int? commandTimeout = null)
    {
        bool isSuccessful = false;
        string message = string.Empty;
        using var connection = factory.CreateConnection()!;
        connection.Open();
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                if (Id == 0)
                {
                    connection.Insert(this, transaction, commandTimeout);
                }
                else
                {
                    connection.Update(this, transaction, commandTimeout);
                }


                var before = await instance.FindByAnyAsync<Contact>((nameof(Contact.IdContact), Id));

                if (_contacts == null)
                {
                    Contacts = [.. before];
                }

                foreach (var contact in Contacts)
                {
                    contact.IdContact = Id;

                    if (contact.Id == 0)
                        connection.Insert(contact, transaction, commandTimeout);
                    else
                        connection.Update(contact, transaction, commandTimeout);
                }

                foreach (var contact in before.Where(c => !Contacts.Any(p => p.IdPerson == c.IdPerson)))
                    connection.Delete(contact, transaction, commandTimeout);

                transaction.Commit();

                isSuccessful = true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                message = string.Join("\n", ex.Message, ex.InnerException?.Message);
            }
        }

        return new DBResult()
        {
            Exception = message,
            Successful = isSuccessful
        };
    }

    public override async Task<DBResult> Remove(IDBLayer instance, IDBFactory factory, int? commandTimeout = null)
    {
        string message = string.Empty;

        bool isSuccessful = false;

        using var connection = factory.CreateConnection()!;
        connection.Open();
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                if (_contacts == null)
                {
                    var contacts = await instance.FindByAnyAsync<Contact>((nameof(Contact.IdContact), Id));
                    Contacts = [.. contacts];
                }
                foreach (var contact in Contacts)
                {
                    connection.Delete(contact, transaction, commandTimeout);
                }
                connection.Delete(this, transaction, commandTimeout);

                transaction.Commit();

                isSuccessful = true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                message = $"Suppression impossible.\n{ex.GetType().Name}: {ex.Message}" +
                    (ex.InnerException != null ? $"\n{ex.InnerException.Message}" : string.Empty);
            }
        }

        return new DBResult()
        {
            Exception = message,
            Successful = isSuccessful
        };
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Person other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Id != 0 && other.Id != 0 && Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id != 0 ? Id.GetHashCode() : RuntimeHelpers.GetHashCode(this);
    }

    [Computed]
    [IgnoreMember]
    public string Error
    {
        get
        {
            return EntityValidator.Error(this) ?? string.Empty;
        }
    }

    [Computed]
    [IgnoreMember]
    public string this[string columnName]
    {
        get
        {
            return EntityValidator.ValidateProperty(this, columnName) ?? string.Empty;
        }
    }

    [Computed]
    [IgnoreMember]
    public string? FullName
    {
        get { return FirstName + " " + LastName; }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
