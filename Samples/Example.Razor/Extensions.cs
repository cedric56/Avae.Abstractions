using Avae.Abstractions;
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
using Microsoft.Extensions.DependencyInjection.Extensions;
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
                "Footer" => new ViewFor<MudText>("Footer"),
                "IconSource" => new ViewFor<MudImage>() { Parameters = new Dictionary<string, object>() { { nameof(MudImage.Src), "avalonia-logo.ico" } } },
                "Content" => new ViewFor<MudText>("Here is my content") { Class = "center" },
                _ => throw new NotImplementedException()
            };
        });

        container.Register((sp, ctx) => new ViewFor<ModalView, ModalViewModel>() { Class = "center" });
        container.Register((sp, ctx) => new ViewFor<EssentialsView, EssentialsViewModel>() { Class = "center" });
        container.Register(typeof(FormViewModel).Name, (sp, parameters) =>
        {
            if (parameters.FirstOrDefault() is NavigableContext context)
            {
                if (context.FactoryParameters.OfType<string>().Any(p => p == FormViewModel.KEY))
                {
                    return new ViewFor<FormPage1, FormViewModel>();
                }
            }

            return new ViewFor<FormView, FormViewModel>();
        });

        container.Register((sp, ctx) => new ViewFor<FormPage2, FormPage2ViewModel>() { Class = "center" });
        container.Register(typeof(FormPage3ViewModel).Name, (sp, parameters) =>
        {
            if (parameters.FirstOrDefault() is NavigableContext context)
            {
                    return new ViewFor<FormPage3, FormPage3ViewModel>(sp, context, new Dictionary<string, object>()
                    {
                        { nameof(Person), context.ViewParameters[0] }
                    })
                    {
                        Class = "center"
                    };
            }

            throw new InvalidOperationException();
        });
    }


    public static void UseSharedLibrary(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton,
        NotificationPosition position = NotificationPosition.BottomLeft,
        int maxDispayments = 5,
        RenderFragment? extras = null,
        ICircuitProvider? circuitProvider = null)
    {
        var navMenu = new ViewFor<NavMenu>();

        services.ConfigureIocContainer(navMenu, 
            position, 
            maxDispayments, 
            RegisterViews, 
            extras, 
            circuitProvider);

        services.TryAdd(ServiceDescriptor.Describe(typeof(HomeViewModel), typeof(HomeViewModel), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(MenuViewModel), typeof(MenuViewModel), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(EssentialsViewModel), typeof(EssentialsViewModel), lifetime));


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