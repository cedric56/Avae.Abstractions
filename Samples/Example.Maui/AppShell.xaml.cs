using Example.ViewModels;

namespace Example.Maui;

public partial class AppShell : Shell
{
    public AppShell(IServiceProvider provider)
    {
        InitializeComponent();

        var vm = new MainViewModel(new Avae.ViewModels.Router(provider));

        BindingContext = vm;

        foreach (var navigable in vm.Navigables)
        {
            this.Items.Add(
            new ShellContent()
            {
                Icon = navigable.Source as ImageSource,
                Title = navigable.DisplayName,
                ContentTemplate = new DataTemplate(() =>
                {
                    vm.SelectedNavigable = navigable;
                    return vm.CurrentView;
                })
            });
        }
    }
}
