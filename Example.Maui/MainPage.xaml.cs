using Avae.Abstractions;
using Example.ViewModels;

namespace Example.Maui
{
    public partial class MainPage : FlyoutPage, IContextFor<MainViewModel>
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public object? Context { get => BindingContext; set => BindingContext = value; }
    }
}
