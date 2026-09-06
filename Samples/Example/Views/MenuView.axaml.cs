using Avae.ViewModels;
using Avalonia.Controls;
using Example.ViewModels;

namespace Example;

public partial class MenuView : UserControl, IViewFor<MenuViewModel>
{
    public MenuView()
    {
        InitializeComponent();
        Loaded += (sender, e) =>
        {
            //hack because of DrawerPage
            //so we need to force it to update
            var dataContext = DataContext as MenuViewModel;
            DataContext = null;
            DataContext = dataContext;
        };
    }

    public object? Context { get => DataContext; set => DataContext = value; }
}