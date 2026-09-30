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
        foreach (var navigable in vm.Navigables)
        {
            this.Items.Add(
            new ShellContent()
            {
                BindingContext = navigable,
                Icon = navigable.Source as ImageSource,
                Title = navigable.DisplayName,
                ContentTemplate = new DataTemplate(() =>
                {
                    var page = new ContentPage
                    {
                        Content = new ActivityIndicator { IsRunning = true }
                    };
                    _ = LoadViewAsync(vm, navigable, page);
                    return page;
                })
            });
        }

        base.OnAppearing();
    }

    private async Task LoadViewAsync(MainViewModel vm, NavigableView navigable, ContentPage placeholder)
    {
        await vm.OnNavigableChanged(navigable);
        placeholder.Content = vm.CurrentView as ContentView ?? new ContentView() { Content = new Label { Text = "Not found" } };
    }
}
