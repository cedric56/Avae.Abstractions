using Avae.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Example.ViewModels;

public partial class RegionsViewModel : ObservableObject, INavigable
{
    private readonly Router _main;
    private readonly Router _side;

    public RegionsViewModel(IServiceProvider sp)
    {
        _main = sp.GetRegion("main");
        _side = sp.GetRegion("side");
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
        => SideView = await _side.GoToType(
            typeof(EssentialsViewModel));

    public async Task OpenMenuAsync()
        => MainView = await _main.GoTo<MenuViewModel>(out _);

    public async Task OnNavigatedTo(NavigableContext context)
    {
        await OpenHomeAsync();
        await OpenEssentialsAsync();        
    }
}
