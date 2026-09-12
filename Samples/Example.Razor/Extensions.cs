using Avae.Razor;
using Avae.Razor.Components;
using Avae.Razor.Interfaces;
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
    private static void RegisterViews(IIocContainer container)
    {
        container.Register(HomeViewModel.TaskDialogKey, (sp, parameters) =>
        {
            return parameters[0] switch
            {
                "Footer" => new ComponentView<MudText>("Footer"),
                "IconSource" => new ComponentView<MudImage>() { Parameters = new Dictionary<string, object>() { { nameof(MudImage.Src), "avalonia-logo.ico" } } },
                "Content" => new CenteredComponentView<MudText>("Here is my content"),
                _ => throw new NotImplementedException()
            };
        });

        container.Register<CenteredComponentView<ModalView, ModalViewModel>>();
        container.Register<CenteredComponentView<EssentialsView, EssentialsViewModel>>();
        container.Register(typeof(FormViewModel).Name, (sp, parameters) =>
        {
            if (parameters.FirstOrDefault() is NavigableContext context)
            {
                if (context.FactoryParameters.OfType<string>().Any(p => p == FormViewModel.KEY))
                {
                    return new ComponentView<FormPage1, FormViewModel>();
                }
            }

            return new ComponentView<FormView, FormViewModel>();
        });

        container.Register<CenteredComponentView<FormPage2, FormPage2ViewModel>>();
        container.Register(typeof(FormPage3ViewModel).Name, (sp, parameters) =>
        {
            if (parameters.FirstOrDefault() is NavigableContext context)
            {
                return new CenteredComponentView<FormPage3, FormPage3ViewModel>(sp, context, new Dictionary<string, object>()
                    {
                        { nameof(Person), context.ViewParameters[0] }
                    });
            }

            throw new InvalidOperationException();
        });
    }


    public static void UseSharedLibrary(this IServiceCollection services,
        bool useScoped = false,
        NotificationPosition position = NotificationPosition.BottomLeft,
        int maxDispayments = 5,
        RenderFragment? extras = null,
        ICircuitProvider? circuitProvider = null)
    {
        var navMenu = new ComponentView<NavMenu>();

        services.ConfigureBase(navMenu, position, maxDispayments, RegisterViews, extras, circuitProvider);
        if (useScoped)
        {
            services.AddScoped<HomeViewModel>();
            services.AddScoped<MenuViewModel>();
            services.AddScoped<EssentialsViewModel>();
        }
        else
        {
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<MenuViewModel>();
            services.AddSingleton<EssentialsViewModel>();
        }
        services.AddTransient<ModalViewModel>();
        services.AddTransient<FormPage2ViewModel>();
        services.AddTransient<FormPage3ViewModel>();
        if (!OperatingSystem.IsBrowser())
        {
            services.UseDBSqlLayer<SqliteConnection>();
        }
        else
        {
            services.UseDBOnionLayer();
        }
    }
}