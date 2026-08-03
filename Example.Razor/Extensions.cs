using Avae.Abstractions;
using Avae.Services;
using Example.Razor.Pages;
using Example.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace Example.Razor
{
    public static class Extensions
    {
        public static void ConfigureProject(this IServiceCollection services)
        {
            services.AddMudServices();
            services.ConfigureIocContainer(container =>
            {
                container.Register(nameof(ModalViewModel), (sp, obj) =>
                {
                    return typeof(ModalView);
                });
            });
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<ModalViewModel>();
        }

        public static void ConfigureIocContainer(this IServiceCollection services,
            Action<IIocContainer>? configure = null,
            Action<ILoggingBuilder>? build = null)
        {
            services.AddSingleton<IIocContainer>(sp => new IocContainer(GetConfiguration(sp), false));
            services.AddSingleton<IIocConfiguration>(sp => new IocConfiguration(sp, configure));
            services.AddTransient<Router>(sp => new Router(sp));
            services.AddSingleton<IDialogService>(GetConfiguration);
            services.AddSingleton<IContentDialogService>(GetConfiguration);
            services.AddSingleton<ITaskDialogService>(GetConfiguration);
            services.AddSingleton<ISystemNotificationService>(GetConfiguration);
            services.AddSingleton<INotificationManager>(GetConfiguration);
            services.AddSingleton<IRequestedTheme>(GetConfiguration);
            //services.AddSingleton<ILogger>(LoggerFactory.Create(builder =>
            //{
            //    build?.Invoke(builder);

            //}).CreateLogger<TApp>());
            //return builder;

            IocConfiguration GetConfiguration(IServiceProvider provider)
            {
                return (IocConfiguration)provider.GetRequiredService<IIocConfiguration>();
            }
        }
    }
}
