using Avae.Services;
using Avae.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using System;

namespace Avae.Abstractions;

public static class Extensions
{
    class CircuitProvider(Action<IServiceProvider> initialize) : ICircuitProvider
    {
        public IServiceProvider Provider { get => null!; set => initialize(value); }
    }

    public static void UseAvaeContainer(this IServiceCollection services,
        ViewFor navMenu,
        NotificationPosition position = NotificationPosition.BottomLeft,
        int maxDispayments = 5,
        Action<IIocContainer>? configure = null,
        RenderFragment? extras = null,
        Action<IServiceProvider>? initialize = null)
    {
        var circuitProvider = new CircuitProvider(initialize ?? (sp => { }));

        services.AddSingleton<ViewFor>(navMenu);
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
        services.AddSingleton<Avae.Services.IDialogService, DialogService>();
        services.AddSingleton<IContentDialogService, ContentDialogService>();
        services.AddSingleton<ITaskDialogService, TaskDialogService>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<IRequestedThemeService, RequestThemeService>();
    }
}
