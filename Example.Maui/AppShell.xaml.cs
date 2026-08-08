using Avae.Abstractions;
using Example.ViewModels;

namespace Example.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            var vm = new MainViewModel(new Avae.Abstractions.Router(ServiceLocator.Default));

            BindingContext = vm;

            foreach(var page in vm.Pages)
            {
                this.Items.Add(new ShellContent()
                {
                    Title = page.DisplayName,
                    ContentTemplate = new DataTemplate(() =>
                    {
                        vm.SelectedPage = page;
                        return vm.ContextFor;
                    })
                });
            }
        }
    }
}
