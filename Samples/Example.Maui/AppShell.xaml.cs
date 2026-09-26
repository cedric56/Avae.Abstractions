using Example.ViewModels;

namespace Example.Maui;

public partial class AppShell : Shell
{
    public AppShell(IServiceProvider provider)
    {
        InitializeComponent();

        var vm = provider.GetRequiredService<MainViewModel>();

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
                    //TODO Two instances created

                    //if (navigable.ViewModelType != typeof(HomeViewModel))
                    //{
                        //vm.OnNavigableChanged(navigable).ConfigureAwait(false).GetAwaiter().GetResult();
                        vm.SelectedNavigable = navigable;
                    //}
                    return vm.CurrentView as ContentPage ?? new ContentPage() { Content = new Label() { Text = "Not found" } };
                })
            });
        }
    }
}
