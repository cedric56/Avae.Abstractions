using Avae.Abstractions;
using Avalonia.Controls;
using Example.ViewModels;

namespace Example;

public partial class HomeView : UserControl, IViewFor<HomeViewModel>
{
    public object? Context { get => DataContext; set => DataContext = value; }
    public HomeView()
    {
        InitializeComponent();
    }
}