using Avae.Abstractions;
using Avae.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;

namespace Avae.Razor
{
    public static class Extensions
    {
        public static void ConfigureBase(this IServiceCollection services,
            NotificationPosition position = NotificationPosition.BottomLeft,
            int maxDispayments = 5,
            Action<IIocContainer>? configure = null)
        {
            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration = new SnackbarConfiguration()
                {
                    PositionClass = position switch
                    {
                        NotificationPosition.TopLeft => Defaults.Classes.Position.TopLeft,
                        NotificationPosition.TopCenter => Defaults.Classes.Position.TopCenter,
                        NotificationPosition.TopRight => Defaults.Classes.Position.TopRight,
                        NotificationPosition.BottomLeft => Defaults.Classes.Position.BottomLeft,
                        NotificationPosition.BottomCenter => Defaults.Classes.Position.BottomCenter,
                        NotificationPosition.BottomRight => Defaults.Classes.Position.BottomRight,
                        _ => Defaults.Classes.Position.TopRight
                    },
                    MaxDisplayedSnackbars = maxDispayments
                };
            });
            services.ConfigureIocContainer(configure);
        }

        private static void ConfigureIocContainer(this IServiceCollection services,
            Action<IIocContainer>? configure = null,
            Action<ILoggingBuilder>? build = null)
        {
            services.AddSingleton<IIocContainer>(sp => new IocContainer(GetConfiguration(sp), false));
            services.AddSingleton<IIocConfiguration>(sp => new IocConfiguration(sp, configure));
            services.AddTransient<Router>(sp => new Router(sp));
            services.AddSingleton<Avae.Services.IDialogService>(GetConfiguration);
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
