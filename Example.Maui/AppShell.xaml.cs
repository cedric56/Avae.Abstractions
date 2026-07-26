using Avae.Abstractions;
using Example.ViewModels;

namespace Example.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            BindingContext = new MainViewModel(new Avae.Abstractions.Router(ServiceLocator.Default));
        }
    }
}
