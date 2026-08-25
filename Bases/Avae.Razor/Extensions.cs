using Avae.Core;
using Avae.Razor.Components;
using Avae.Services;
using Avae.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;

namespace Avae.Razor;

public static class Extensions
{
    public static void ConfigureBase(this IServiceCollection services,
        ComponentView navMenu,
        NotificationPosition position = NotificationPosition.BottomLeft,
        int maxDispayments = 5,            
        Action<IIocContainer>? configure = null,
        RenderFragment? extras = null)
    {
        services.AddSingleton<ComponentView>(navMenu);
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
        services.ConfigureIocContainer(configure, extras: extras);
    }

    private static void ConfigureIocContainer(this IServiceCollection services,
        Action<IIocContainer>? configure = null,
        Action<ILoggingBuilder>? build = null,
        RenderFragment? extras = null)
    {
        services.AddSingleton<CircuitServiceAccessor>();
        services.AddSingleton<IIocContainer>(sp => new IocContainer(GetConfiguration(sp), false));
        services.AddSingleton<IIocConfiguration>(sp => new IocConfiguration(sp, configure, extras));
        services.AddTransient<Router>(sp => new Router(sp));
        services.AddSingleton<Services.IDialogService>(GetConfiguration);
        services.AddSingleton<IContentDialogService>(GetConfiguration);
        services.AddSingleton<ITaskDialogService>(GetConfiguration);
        services.AddSingleton<Services.INotificationService>(GetConfiguration);
        services.AddSingleton<IRequestedThemeService>(GetConfiguration);
        IocConfiguration GetConfiguration(IServiceProvider provider)
        {
            return (IocConfiguration)provider.GetRequiredService<IIocConfiguration>();
        }
    }
}
