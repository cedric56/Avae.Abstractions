using Avae.Services;
using Avae.ViewModels;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace Avae.Avalonia;

public static class AvaeBuilder
{
    public static AppBuilder CreateAvaloniaApp<TApp>(
       Func<IServiceProvider, TApp> appFactory,
       string iconUrl,
       Action<IServiceCollection>? configureServices = null,
       Action<IIocContainer>? configure = null,
       TypeDialog typeDialog = TypeDialog.Box) where TApp : Application
    {
        IServiceProvider? provider = null;
        return AppBuilder
            .Configure(() => appFactory(provider!))
            .ConfigureIocContainer<TApp>(
            out provider,
            services => configureServices?.Invoke(services),
            container => configure?.Invoke(container),
            iconUrl,
            typeDialog);
    }

    public static AppBuilder CreateAvaeApp<TApp>(
       Func<IServiceProvider, TApp> appFactory,
       Action<IServiceCollection>? configureServices = null,
       Action<IIocContainer>? configure = null) where TApp : AvaeApplication
    {
        IServiceProvider? provider = null;
        return AppBuilder
            .Configure(() => appFactory(provider!))
            .ConfigureIocContainer<TApp>(
            out provider,
            services => configureServices?.Invoke(services),
            container => configure?.Invoke(container));
    }

    public static AppBuilder ConfigureIocContainer<TApp>(this AppBuilder builder,
       out IServiceProvider provider,
       Action<IServiceCollection>? configureServices = null,
       Action<IIocContainer>? configure = null,
       string? iconUrl = null,
       TypeDialog typeDialog = TypeDialog.Box)
       where TApp : Application
    {
        var services = new ServiceCollection();
        services.AddSingleton<TApp>(_ => (TApp)builder.Instance!);
        services.AddTransient<Router>(sp => new Router(sp));
        services.AddSingleton<IBrokerService, BrokerService>();
        services.AddSingleton<IIocContainer>(sp =>
        {
            var container = new IocContainer(sp);
            configure?.Invoke(container);
            return container;
        });
        services.AddSingleton<IIocConfiguration>(sp => (IocContainer)sp.GetRequiredService<IIocContainer>());
        services.AddTransient<INotificationService, NotificationService>();
        services.AddSingleton<IContentDialogService>(sp => new ContentDialogService(sp));
        services.AddSingleton<ITaskDialogService, TaskDialogService>();
        services.AddSingleton<IRequestedThemeService, RequestThemeService>();
        services.AddSingleton<IDialogService>(sp =>
        {
            var app = sp.GetRequiredService<TApp>();
            if (app is AvaeApplication avae)
            {
                iconUrl = avae.IconUrl;
                typeDialog = avae.TypeDialog;
            }

            if (typeDialog == TypeDialog.Fluent)
                return (ContentDialogService)sp.GetRequiredService<IContentDialogService>();
            return new DialogService(sp, iconUrl ?? string.Empty);
        });
        configureServices?.Invoke(services);
        provider = services.BuildServiceProvider();
        return builder;
    }
}
