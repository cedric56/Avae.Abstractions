using Avae.ViewModels;
using Example.ViewModels;
using System.Diagnostics;

namespace Example;

public partial class MenuView : View, IViewFor<MenuViewModel>
{
    public MenuView()
    {
        InitializeComponent();
    }
}