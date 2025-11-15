using Avae.Abstractions;
using Avae.DAL;
using CommunityToolkit.Mvvm.ComponentModel;
using Dapper.Contrib.Extensions;
using Example.Dal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KeyAttribute = Dapper.Contrib.Extensions.KeyAttribute;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace Example.Models
{
    [Table(nameof(Person))]
    public class Person : ObservableObject, IModelBase, IDataErrorInfo
    {
        bool inTransaction = false;

        private IList<Contact>? _contacts;
        private string? _firstName;
        private string? _lastName;

        [Key]
        //[MessagePackKey(0)]
        public virtual int Id { get; set; }

        [Required(ErrorMessage = "FirstName must be set")]
        //[MessagePackKey(1)]
        public virtual string? FirstName
        {
            get => _firstName;
            set
            {
                SetProperty(ref _firstName, value);

            }
        }

        [Required(ErrorMessage = "LastName must be set")]
        //[MessagePackKey(2)]
        public virtual string? LastName
        {
            get => _lastName; set
            {
                SetProperty(ref _lastName, value);
            }
        }

        [Computed]
        public virtual IList<Contact> Contacts
        {
            get
            {
                if (_contacts == null)
                {
                    if (Id == 0)
                        _contacts = [];
                    else
                        _contacts = [.. GetContacts(DBBase.Instance)];
                }
                return _contacts;
            }
            set
            {
                _contacts = value;
            }
        }

        public async Task<IEnumerable<Contact>> GetContactsAsync(IDBBase instance)
        {
            return AvoidReadings(await instance.FindContactByAnyAsync(idContact: this.Id));
        }

        public IEnumerable<Contact> GetContacts(IDBBase instance)
        {
            return AvoidReadings(instance.FindContactByAny(idContact: this.Id));
        }

        private IEnumerable<Contact> AvoidReadings(IEnumerable<Contact> results)
        {
            foreach (var contact in results)
            {
                if (!inTransaction)
                {
                    var person = Repository.Instance.Persons.FirstOrDefault(p => p.Id == contact.IdPerson);
                    if (person != null)
                        contact.Person = person;
                }

                contact.PersonContact = this;
            }
            return results;
        }

        public Task<Result> SaveAsync()
        {
            return SavePersonAsync(DBBase.Instance);
        }

        public Task<Result> RemoveAsync()
        {
            return RemovePersonAsync(DBBase.Instance);
        }
        
        public async Task<Result> SavePersonAsync(IDBBase instance)
        {
            bool isSuccessful = false;
            string message = string.Empty;
            using var connection = instance.DbConnection();
            await instance.OpenAsync(connection);

            using (var transaction = instance.BeginTransaction(connection))
            {
                try
                {
                    inTransaction = true;
                    if (Id == 0)
                    {
                        instance.Insert(this, connection, transaction);
                    }
                    else
                    {
                        instance.Update(this, connection, transaction);
                    }

                    var before = await instance.FindContactByAnyAsync(idContact: Id);

                    if (_contacts == null)
                    {
                        var contacts = await GetContactsAsync(instance);
                        Contacts = [.. contacts];
                    }
                    foreach (var contact in Contacts)
                    {
                        contact.IdContact = Id;

                        if (contact.Id == 0)
                            instance.Insert(contact, connection, transaction);
                        else
                            instance.Update(contact, connection, transaction);
                    }

                    foreach (var contact in before.Where(c => !Contacts.Any(p => p.IdPerson == c.IdPerson)))
                        instance.Delete(contact, connection, transaction);

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
                    inTransaction = false;
                    Repository.Instance.SetPersons(null!);
                }
            }

            return new Result()
            {
                Exception = message,
                Success = isSuccessful
            };
        }

        public async Task<Result> RemovePersonAsync(IDBBase instance)
        {
            string message = string.Empty;

            bool isSuccessful = false;

            using var connection = instance.DbConnection();
            await instance.OpenAsync(connection);

            using (var transaction = instance.BeginTransaction(connection))
            {
                try
                {
                    if (_contacts == null)
                    {
                        var contacts = await GetContactsAsync(instance);
                        Contacts = [.. contacts];
                    }
                    foreach (var contact in Contacts)
                    {
                        instance.Delete(contact, connection, transaction);
                    }
                    instance.Delete(this, connection, transaction);

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
                    Repository.Instance.SetPersons(null!);
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

        [Computed]
        public string Error
        {
            get
            {
                return InputValidation<Person>.Error(this);
            }
        }

        [Computed]
        public string this[string columnName]
        {
            get
            {
                return InputValidation<Person>.Validate(this, columnName);
            }
        }
    }
}
