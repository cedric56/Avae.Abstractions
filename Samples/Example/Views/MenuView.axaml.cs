using Avae.ViewModels;
using Example.ViewModels;

namespace Example;

public partial class MenuView : View, IViewFor<MenuViewModel>
{
    public MenuView()
    {
        InitializeComponent();
    }
}