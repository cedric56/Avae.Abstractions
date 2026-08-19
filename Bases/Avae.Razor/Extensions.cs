using Append.Blazor.Notifications;
using Avae.ViewModels;
using Avae.Razor.Components;
using Avae.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;
using System.Runtime.CompilerServices;
using Avae.Razor.Implementations;

namespace Avae.Razor;

public static class Extensions
{
    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    [return: UnsafeAccessorType("Append.Blazor.Notifications.NotificationService, Append.Blazor.Notifications")]
    internal extern static object CreateService(IJSRuntime jSRuntime);

    public static void ConfigureBase(this IServiceCollection services,
        ComponentView navMenu,
        bool useSystemNotificationService = false,
        NotificationPosition position = NotificationPosition.BottomLeft,
        int maxDispayments = 5,            
        Action<IIocContainer>? configure = null)
    {
        if (useSystemNotificationService)
            services.AddSingleton<ISystemNotificationService, SystemNotificationService>();

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
        //services.AddSingleton<ISystemNotificationService>(GetConfiguration);
        services.AddSingleton<Services.INotificationService>(GetConfiguration);
        services.AddSingleton<IRequestedThemeService>(GetConfiguration);
        services.AddNotifications();
        IocConfiguration GetConfiguration(IServiceProvider provider)
        {
            return (IocConfiguration)provider.GetRequiredService<IIocConfiguration>();
        }
    }
}
