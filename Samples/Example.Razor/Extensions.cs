using Avae.Razor;
using Avae.Services;
using Avae.ViewModels;
using Example.Models;
using Example.Razor.Components;
using Example.ViewModels;
using MudBlazor;

namespace Example.Razor;

public static class Extensions
{
    public static void UseSharedLibrary(this IServiceCollection services,
        Runtime runtime,
        Func<IServiceProvider, Task>? onCircuitProviderChanged = null)
    {
        var lifetime = ServiceLifetime.Scoped;

        services.AddNavigationRegion("main", lifetime);
        services.AddNavigationRegion("side", lifetime);
        services.RegisterWithLifetime(HomeViewModel.TaskDialogKey, (sp, parameters) =>
        {
            return parameters[0] switch
            {
                "Footer" => new ViewFor<MudText>("This is my footer"),
                "IconSource" => new ViewFor<MudImage>()
                {
                    Parameters = new Dictionary<string, object>
                    {
                        { nameof(MudImage.Src), "avalonia-logo.ico" },
                        { nameof(MudImage.Height), 30 },
                        { nameof(MudImage.Width), 30 }
                    }
                },
                "Content" => new ViewFor<MudText>("Here is my content") { Class = "center" },
                _ => throw new NotImplementedException()
            };
        });
        services.AddScoped<IMvvmManager, MainViewModel>();
        services.RegisterViewFor<Home, HomeViewModel>();
        services.RegisterViewFor<MenuView, MenuViewModel>();
        services.RegisterViewFor<EssentialsView, EssentialsViewModel>();
        services.RegisterViewFor<Home, HomeViewModel>("HomeView", nameof(HomeViewModel));
        services.RegisterViewFor<MenuView, MenuViewModel>("MenuView", nameof(MenuViewModel));
        services.RegisterViewFor<EssentialsView, EssentialsViewModel>("EssentialsView", nameof(EssentialsViewModel));
        services.RegisterViewFor<RegionsView, RegionsViewModel>();
        services.RegisterModalFor<ModalView, ModalViewModel, string?>();
        services.RegisterViewFor((sp) => new ViewFor<FormPage2, FormPage2ViewModel> { Class = "center" });
        services.RegisterViewFor<FormPage3, FormPage3ViewModel, Person>(
            (sp, person) => new ViewFor<FormPage3, FormPage3ViewModel>(new Dictionary<string, object>()
               {
                   { nameof(Person), person }
               })
            {
                Class = "center"
            });
        services.RegisterViewFor<FormPage1, FormViewModel>(viewModelKey: FormViewModel.KEY);
        services.RegisterViewFor<FormView, FormViewModel>();
        services.AddPersonServiceRemote();

        services.UseAvae(
            runtime,
            onCircuitProviderChanged,
            NotificationPosition.BottomLeft,
            5);
    }
}