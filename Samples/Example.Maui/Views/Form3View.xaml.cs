using Avae.ViewModels;
using Example.Models;

namespace Example.Maui.Views;

public partial class Form3View : ContentView, IViewFor
{
    public object? Context { get => BindingContext; set => BindingContext = value; }

    public Form3View(Person person)
    {
        InitializeComponent();
        lbl.Text = $"Hello {person.FullName}";
    }
}