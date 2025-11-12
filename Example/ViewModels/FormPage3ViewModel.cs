using Avae.Abstractions;
using Example.Models;

namespace Example.ViewModels
{
    internal class FormPage3ViewModel(Person person) : IViewModelBase
    {
        public Person Person { get; } = person;
        public string Title => "Welcome to page 3";
    }
}
