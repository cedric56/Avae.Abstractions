using Avae.Abstractions;
using Avae.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Example.ViewModels;
using UXDivers.Popups.Services;

namespace Example.Maui.Views;

public partial class HomeView : ContentPage, IViewFor<HomeViewModel>
{
    public HomeView()
    {
        InitializeComponent();
    }

    public object? Context { get => BindingContext; set => BindingContext = value; }

    private async void Button_Clicked(object? sender, EventArgs e)
    {
        TestPopupPage page = null!;

        page = new TestPopupPage<string>("Hello", new System.Collections.ObjectModel.ObservableCollection<NamedCommand>()
        {
            new NamedCommand()
            {
                 Name = "Test",
                 Command = new RelayCommand(async () =>
                 {
                     await IPopupService.Current.PopAsync(page);
                 })
            },
            new NamedCommand()
            {
                 Name = "Test",
                 Command = new RelayCommand(async () =>
                 {
                     await IPopupService.Current.PopAsync(page);
                 })
            }
        });
        await IPopupService.Current.PushAsync(page);
    }
}