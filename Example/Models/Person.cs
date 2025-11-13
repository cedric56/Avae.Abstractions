using Avae.Abstractions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Example.Models
{
    public class Person : IDataErrorInfo
    {
        [Required(ErrorMessage = "You have to enter first name.")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "You have to enter last name.")]
        public string? LastName { get; set; }

        public string Error
        {
            get
            {
                return InputValidation<Person>.Error(this);
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidation<Person>.Validate(this, columnName);
            }
        }
    }
}
