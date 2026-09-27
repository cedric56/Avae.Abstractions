using Avae.Razor;
using Avae.Services;
using Avae.ViewModels;
using Example.Models;
using Example.Razor.Components;
using Example.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;

namespace Example.Razor;

public static class Extensions
{
    public static void UseSharedLibrary(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton,
        TypeRazorProject typeRazorProject = TypeRazorProject.Wasm,
        Action<IServiceProvider>? initialize = null)
    {
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

        services.AddSingleton<IMvvmManager, MainViewModel>();
        services.RegisterViewFor<Home, HomeViewModel>(lifetime);
        services.RegisterViewFor<MenuView, MenuViewModel>(lifetime);
        services.RegisterViewFor<EssentialsView, EssentialsViewModel>(lifetime);
        services.RegisterViewFor((sp) => new ModalFor<ModalView, ModalViewModel, string?> { Class = "center" });
        services.RegisterViewFor((sp) => new ViewFor<FormPage2, FormPage2ViewModel> { Class = "center" });
        services.RegisterViewFor<FormPage3, FormPage3ViewModel, Person>(
            (sp, person) => new ViewFor<FormPage3, FormPage3ViewModel>(sp, null, new Dictionary<string, object>()
               {
                   { nameof(Person), person }
               })
            {
                Class = "center"
            });
        services.RegisterViewFor<FormPage1, FormViewModel>(key: FormViewModel.KEY);
        services.RegisterViewFor<FormView, FormViewModel>();
        services.AddPersonServiceRemote();        
        
        services.UseAvae(
            typeRazorProject,
            NotificationPosition.BottomLeft, 
            5,     
            initialize);
    }
}