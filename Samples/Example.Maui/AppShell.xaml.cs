using Avae.ViewModels;
using Example.ViewModels;

namespace Example.Maui;

public partial class AppShell : Shell
{
    public AppShell(IServiceProvider provider)
    {
        InitializeComponent();

        var vm = provider.GetRequiredService<MainViewModel>();
        
        BindingContext = vm;       
    }

    protected override void OnAppearing()
    {
        var vm = (MainViewModel)BindingContext;
        var dico = new Dictionary<Type, IViewFor>();
        dico.Add(typeof(HomeViewModel), vm.CurrentView!);
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
                    return vm.CurrentView as ContentPage ?? new ContentPage() { Content = new Label() { Text = "Not found" } };
                })
            });
        }

        base.OnAppearing();
    }
}
