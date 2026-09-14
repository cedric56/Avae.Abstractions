using Avae.Abstractions;
using Example.ViewModels;

namespace Example;

public partial class EssentialsView : ViewFor<EssentialsViewModel>
{
    public EssentialsView()
    {
        InitializeComponent();
    }
}