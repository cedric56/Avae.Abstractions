using Avalonia.Controls;

namespace Example.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        MenuItemsListBox.SelectionChanged += (sender, e) =>
        {



        };
    }
}
