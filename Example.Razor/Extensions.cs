using Avae.Abstractions;
using Avae.DAL;
using Avae.Razor;
using Avae.Razor.Components;
using Avae.Services;
using Example.Models;
using Example.Razor.Components;
using Example.ViewModels;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Example.Razor
{
    public static class Extensions
    {
        public static void ConfigureProject(this IServiceCollection services,
            NotificationPosition position = NotificationPosition.BottomLeft,
            int maxDispayments = 5)
        {
            services.ConfigureBase(
                new ComponentView<NavMenu>(),
                position, maxDispayments, container =>
            {
                container.Register<CenteredComponentView<ModalView, ModalViewModel>>();
                container.Register(typeof(FormViewModel).Name, (sp, parameters) =>
                {
                    if(parameters.FirstOrDefault() is NavigationContext context)
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
            });
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<MenuViewModel>();
            services.AddTransient<ModalViewModel>();
            services.AddTransient<FormPage2ViewModel>();
            services.AddTransient<FormPage3ViewModel>();

            if (!OperatingSystem.IsBrowser())
            { 
                services.UseDbLayer<IDBLayer>(sp => new DBSqlLayer(sp));

                var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var dbPath = Path.Combine(folder, "database.db");
                var connectionString = $"Data Source={dbPath};Foreign Keys=True";
                services.AddTransient<IDbConnection>(_ => new SqliteConnection(connectionString));
            }
        }
    }
}
