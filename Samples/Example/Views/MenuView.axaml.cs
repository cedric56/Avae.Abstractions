using Example.ViewModels;

namespace Example;

public partial class MenuView : ViewFor<MenuViewModel>
{
    public MenuView()
    {
        InitializeComponent();
    }
}