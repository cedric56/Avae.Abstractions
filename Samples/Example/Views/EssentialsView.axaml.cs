using Avae.ViewModels;
using Avalonia.Controls;
using Example.ViewModels;

namespace Example;

public partial class EssentialsView : UserControl, IViewFor<EssentialsViewModel>
{
    public EssentialsView()
    {
        InitializeComponent();
    }

    public object? Context { get => DataContext; set => DataContext = value; }
}