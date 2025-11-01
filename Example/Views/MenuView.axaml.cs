using Avae.Abstractions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Example.ViewModels;

namespace Example;

public partial class MenuView : UserControl, IContextFor<MenuViewModel>
{
    public MenuView()
    {
        InitializeComponent();
    }
}