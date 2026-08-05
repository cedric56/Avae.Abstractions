using Avae.DAL;
using Avae.DAL.Interfaces;
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
using System.Data;
using NavigationContext = Avae.Abstractions.NavigationContext;

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
            else
            {
                services.AddScoped<IDBOnionService>(sp =>
                {
                    try
                    {
                        return sp.GetMagicOnion<IDBOnionService>("http://localhost:5001");
                    }
                    catch
                    {
                        return new DBOnionNotConnected();
                    }
                });
                services.AddScoped<IOnionService>(provider => provider.GetRequiredService<IDBOnionService>());
                services.AddTransient<IXmlHttpRequest>(sp => new XmlHttpRequest("http://localhost:5001/routes/IDBOnionService/"));
                services.AddScoped<IDBLayer>(provider => new DBOnionLayer(provider));
                services.AddScoped<IDataAccessLayer>(provider => provider.GetRequiredService<IDBLayer>());
            }
        }
    }
}
