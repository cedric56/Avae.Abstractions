using Avae.Abstractions;
using Avae.Razor;
using Avae.Razor.Components;
using Avae.Services;
using Example.Models;
using Example.Razor.Components;
using Example.Razor.Layout;
using Example.ViewModels;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using NavigationContext = Avae.Abstractions.NavigationContext;

namespace Example.Razor
{
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
                    "Content" => new ComponentView<MudText>("Here is my content"),
                    _ => throw new NotImplementedException()
                };
            });

            container.Register<CenteredComponentView<ModalView, ModalViewModel>>();
            container.Register(typeof(FormViewModel).Name, (sp, parameters) =>
            {
                if (parameters.FirstOrDefault() is NavigationContext context)
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
                if (parameters.FirstOrDefault() is NavigationContext context)
                {
                    return new CenteredComponentView<FormPage3, FormPage3ViewModel>(sp, context, new Dictionary<string, object>()
                        {
                            { "Person", context.ViewParameters[0] }
                        });
                }

                throw new InvalidOperationException();
            });
        }

        public static void ConfigureProject(this IServiceCollection services,
            NotificationPosition position = NotificationPosition.BottomLeft,
            int maxDispayments = 5)
        {
            services.ConfigureBase(new ComponentView<NavMenu>(), position, maxDispayments, RegisterViews);
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<MenuViewModel>();
            services.AddTransient<ModalViewModel>();
            services.AddTransient<FormPage2ViewModel>();
            services.AddTransient<FormPage3ViewModel>();

            if (!OperatingSystem.IsBrowser())
            {
                services.UseDBSqlLayer<SqliteConnection>(out _, out _);
            }
            else
            {
               services.UseDBOnionLayer(out _, out _);
            }
        }
    }
}