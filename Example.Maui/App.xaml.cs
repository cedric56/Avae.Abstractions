namespace Example.Maui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());

            //return new Window(new NavigationPage(new MainPage()
            //{
            //    BindingContext = new MainViewModel(new Avae.Abstractions.Router(ServiceLocator.Default)),
            //}));
        }
    }
}