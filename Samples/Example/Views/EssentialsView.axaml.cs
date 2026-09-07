using Avae.ViewModels;
using Avalonia.Controls;
using Example.ViewModels;

namespace Example;

public partial class EssentialsView : View, IViewFor<EssentialsViewModel>
{
    public EssentialsView()
    {
        InitializeComponent();
    }
}