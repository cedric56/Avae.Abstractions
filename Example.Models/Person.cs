using Avae.Abstractions;
using Avae.DAL;
using CommunityToolkit.Mvvm.ComponentModel;
using Dapper.Contrib.Extensions;
using MemoryPack;
using MessagePack;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace Example.Models
{
    [Dapper.Contrib.Extensions.Table(nameof(Person))]
    //[MessagePackObject]
    [MemoryPackable]
    [MessagePackObject]
    [ObservableObject]
    public partial class Person : DBModelBase, IModelBase, IDataErrorInfo
    {   
        private IList<Contact>? _contacts;
        private string? _firstName;
        private string? _lastName;

        [Dapper.Contrib.Extensions.Key]
        [MessagePack.Key(0)]
        public int Id { get; set; }

        [Required(ErrorMessage = "FirstName must be set")]
        [MessagePack.Key(1)]
        public string? FirstName
        {
            get => _firstName;
            set
            {
                SetProperty(ref _firstName, value);

            }
        }

        [Required(ErrorMessage = "LastName must be set")]
        //[MessagePackKey(2)]
        [MessagePack.Key(2)]
        public string? LastName
        {
            get => _lastName; set
            {
                SetProperty(ref _lastName, value);
            }
        }

        [Dapper.Contrib.Extensions.Computed]
        [MessagePack.Key(3)]
        [MemoryPackIgnore]
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
                        foreach (var contact in contacts)
                        {
                            var person = Repository.Instance.Persons.FirstOrDefault(p => p.Id == contact.IdPerson);
                            if (person != null)
                                contact.Person = person;

                            contact.PersonContact = this;
                        }

                        _contacts = [.. contacts];
                    }
                }
                return _contacts;
            }
            private set
            {
                _contacts = value;
            }
        }       
        
        public override async Task<Result> DbTransSave(IDataAccessLayer instance)
        {
            bool isSuccessful = false;
            string message = string.Empty;
            using var connection = SimpleProvider.GetService<DbConnection>();
            await connection.OpenAsync();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    if (Id == 0)
                    {
                        connection.Insert(this, transaction);                        
                    }
                    else
                    {
                        connection.Update(this, transaction);
                    }

                    var before = await instance.FindByAnyAsync<Contact>((nameof(Contact.IdContact), Id));

                    if (_contacts == null)
                    {
                        //var contacts = await GetContactsAsync(instance);
                        Contacts = [.. before];
                    }

                    foreach (var contact in Contacts)
                    {
                        contact.IdContact = Id;

                        if (contact.Id == 0)
                            connection.Insert(contact, transaction);
                        else
                            connection.Update(contact, transaction);
                    }

                    foreach (var contact in before.Where(c => !Contacts.Any(p => p.IdPerson == c.IdPerson)))
                        connection.Delete(contact, transaction);

                    transaction.Commit();

                    isSuccessful = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    message = ex.Message;
                }
                finally
                {
                   await Repository.Instance.ClearPersons();
                }
            }

            return new Result()
            {
                Exception = message,
                Success = isSuccessful
            };
        }

        public override async Task<Result> DbTransRemove(IDataAccessLayer instance)
        {
            string message = string.Empty;

            bool isSuccessful = false;

            using var connection = SimpleProvider.GetService<DbConnection>();
            await connection.OpenAsync();

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
                        connection.Delete(contact, transaction);
                    }
                    connection.Delete(this, transaction);

                    transaction.Commit();

                    isSuccessful = true;
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    message = $"Suppression impossible, cette personne fait partie des contacts d'un autre usager." +
                        "\n" + ex.Message;
                }
                finally
                {
                    await Repository.Instance.ClearPersons();
                }
            }

            return new Result()
            {
                Exception = message,
                Success = isSuccessful
            };
        }

        public override bool Equals(object? obj)
        {
            return obj is Person person && person.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        [Dapper.Contrib.Extensions.Computed]
        [MessagePack.IgnoreMember]
        public string Error
        {
            get
            {
                return InputValidation<Person>.Error(this);
            }
        }

        [Dapper.Contrib.Extensions.Computed]
        [MessagePack.IgnoreMember]
        public string this[string columnName]
        {
            get
            {
                return InputValidation<Person>.Validate(this, columnName);
            }
        }
    }
}
