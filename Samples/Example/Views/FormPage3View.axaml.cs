using Avae.Abstractions;
using Example.Models;
using Example.ViewModels;

namespace Example;

public partial class FormPage3View : ViewFor<FormPage3ViewModel>
{
    public FormPage3View()
    {
        InitializeComponent();
    }

    public FormPage3View(Person person)
        : this()
    {

    }
}