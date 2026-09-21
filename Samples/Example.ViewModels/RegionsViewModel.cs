using Avae.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Example.ViewModels;

public partial class RegionsViewModel : ObservableObject, INavigable
{
    private readonly Router _main;
    private readonly Router _side;

    public RegionsViewModel(IServiceProvider sp)
    {
        _main = sp.GetRequiredService<Router>();
        _side = sp.GetRequiredService<Router>();
    }

    [ObservableProperty] private IViewFor? mainView;
    [ObservableProperty] private IViewFor? sideView;

    [RelayCommand]
    public Task Next() => OpenMenuAsync();

    [RelayCommand]
    public async Task Back()
    {
        await _main.BackAsync();
        MainView = _main.CurrentView;
    }

    public async Task OpenHomeAsync()
        => MainView = await _main.GoTo<HomeViewModel>(out _);

    public async Task OpenEssentialsAsync()
        => SideView = await _main.GoToType(
            typeof(EssentialsViewModel));

    public async Task OpenMenuAsync()
        => MainView = await _side.GoTo<MenuViewModel>(out _);

    public Task<bool> CanNavigateAsync()
    {
        return Task.FromResult(true);
    }

    public Task OnNavigatedTo(NavigableContext context)
    {
        return Task.CompletedTask;
    }

    public async Task OnNavigatedFrom(NavigableContext context)
    {
        await OpenHomeAsync();
        await OpenEssentialsAsync();
    }
}
