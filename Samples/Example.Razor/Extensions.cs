using Avae.Razor;
using Avae.Services;
using Avae.ViewModels;
using Example.DAL;
using Example.Models;
using Example.Razor.Components;
using Example.Razor.Layout;
using Example.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;

namespace Example.Razor;

public static class Extensions
{
    public static void UseSharedLibrary(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton,
        RenderFragment? extras = null,
        Action<IServiceProvider>? initialize = null)
    {
        services.RegisterWithLifetime(HomeViewModel.TaskDialogKey, (sp, parameters) =>
        {
            return parameters[0] switch
            {
                "Footer" => new ViewFor<MudText>("Footer"),
                "IconSource" => new ViewFor<MudImage>()
                {
                    Parameters = new Dictionary<string, object> { { nameof(MudImage.Src), "avalonia-logo.ico" } }
                },
                "Content" => new ViewFor<MudText>("Here is my content") { Class = "center" },
                _ => throw new NotImplementedException()
            };
        });

        services.RegisterViewFor((sp) => new ViewFor<Home, HomeViewModel>(), lifetime);
        services.RegisterViewFor((sp) => new ViewFor<MenuView, MenuViewModel>(), lifetime);
        services.RegisterViewFor((sp) => new ViewFor<EssentialsView, EssentialsViewModel>(), lifetime);
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
        services.RegisterViewFor((sp) => new ViewFor<FormPage1, FormViewModel>(), key: FormViewModel.KEY);
        services.RegisterViewFor((sp) => new ViewFor<FormView, FormViewModel>());

        if (!OperatingSystem.IsBrowser())
        {
            services.UseDBSqlLayer<SqliteConnection>();
        }
        else
        {
            services.UseDBOnionLayer();
        }

        var navMenu = new ViewFor<NavMenu>();
        services.UseAvae(navMenu,
            NotificationPosition.BottomLeft, 
            5,            
            extras,
            initialize);
    }
}