using Avae.Core;
using Avae.Razor.Components;
using Avae.Razor.Interfaces;
using Avae.Razor.Services;
using Avae.Services;
using Avae.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;

namespace Avae.Razor;

public static class Extensions
{
    class CircuitProvider : ICircuitProvider
    {
        public required IServiceProvider Provider { get; set; }
    }

    public static void ConfigureBase(this IServiceCollection services,
        ComponentView navMenu,
        NotificationPosition position = NotificationPosition.BottomLeft,
        int maxDispayments = 5,
        Action<IIocContainer>? configure = null,
        RenderFragment? extras = null,
        ICircuitProvider? circuitProvider = null)
    {
        circuitProvider ??= new CircuitProvider() { Provider = null! };

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
        services.ConfigureIocContainer(circuitProvider, configure, extras: extras);
    }

    private static void ConfigureIocContainer(this IServiceCollection services,
        ICircuitProvider circuitProvider,
        Action<IIocContainer>? configure = null,
        RenderFragment? extras = null)
    {
        services.AddSingleton<ICircuitProvider>(circuitProvider);
        services.AddSingleton<IIocContainer>(sp =>
        {
            var container = new IocContainer(sp);
            configure?.Invoke(container);
            return container;
        });
        services.AddSingleton<IIocConfiguration>(sp => (IocContainer)sp.GetRequiredService<IIocContainer>());
        services.AddSingleton<RenderFragment>(_ => extras ?? new RenderFragment(_ => { }));
        services.AddTransient<Router>();
        services.AddSingleton<Avae.Services.IDialogService, Services.DialogService>();
        services.AddSingleton<IContentDialogService, ContentDialogService>();
        services.AddSingleton<ITaskDialogService,TaskDialogService>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<IRequestedThemeService,RequestThemeService>();
    }
}
