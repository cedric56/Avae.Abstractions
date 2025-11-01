using Avae.Abstractions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Example.ViewModels;

namespace Example;

public partial class HomeView : UserControl, IContextFor<HomeViewModel>
{
    public HomeView()
    {
        InitializeComponent();
    }
}