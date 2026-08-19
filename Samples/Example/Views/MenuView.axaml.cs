using Avae.ViewModels;
using Avalonia.Controls;
using Example.ViewModels;

namespace Example;

public partial class MenuView : UserControl, IViewFor<MenuViewModel>
{
    public MenuView()
    {
        InitializeComponent();
    }

    public object? Context { get => DataContext; set => DataContext = value; }
}