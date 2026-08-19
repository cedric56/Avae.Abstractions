using Avae.ViewModels;
using Avae.Core;
using Example.ViewModels;

namespace Example.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            var vm = new MainViewModel(new Avae.ViewModels.Router(ServiceLocator.Default));

            BindingContext = vm;

            foreach(var page in vm.ViewModels)
            {
                this.Items.Add(new ShellContent()
                {
                    Title = page.DisplayName,
                    ContentTemplate = new DataTemplate(() =>
                    {
                        vm.SelectedViewModel = page;
                        return vm.CurrentView;
                    })
                });
            }
        }
    }
}
