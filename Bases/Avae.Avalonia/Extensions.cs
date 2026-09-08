using Avae.Services;
using Avae.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Avae.Avalonia;

public static class AvaeBuilder
{
    public static AppBuilder Configure<TApp>(
        Func<IServiceProvider, TApp> appFactory,
       Action<IServiceCollection>? configureServices = null,
       Action<IIocContainer>? configure = null,
       Action<ILoggingBuilder>? configureBuilder = null) where TApp : AvaeApplication
    {
        IServiceProvider? provider = null;
        return AppBuilder
            .Configure(() => appFactory(provider!))
            .ConfigureIocContainer<TApp>(
            out provider,
            services => configureServices?.Invoke(services),
            container => configure?.Invoke(container),
            build => configureBuilder?.Invoke(build));
    }

    public static AppBuilder ConfigureIocContainer<TApp>(this AppBuilder builder,
       out IServiceProvider provider,
       Action<IServiceCollection>? configureServices = null,
       Action<IIocContainer>? configure = null,
       Action<ILoggingBuilder>? build = null)
       where TApp : AvaeApplication
    {
        var services = new ServiceCollection();
        services.AddSingleton<TApp>(_ => (TApp)builder.Instance!);
        services.AddTransient<Router>(sp => new Router(sp));
        services.AddSingleton<IBrokerService, BrokerService>();
        services.AddSingleton<IIocContainer>(sp => new IocContainer(sp, sp.GetRequiredService<IIocConfiguration>()));
        services.AddSingleton<IIocConfiguration>(sp => new IocConfiguration(() => (IocContainer)sp.GetRequiredService<IIocContainer>(), configure));
        services.AddSingleton<IDialogService>(sp =>
        {
            var app = sp.GetRequiredService<TApp>();
            return app.TypeDialog == TypeDialog.Box ? new DialogService(sp, app.IconUrl) :
                         sp.GetRequiredService<IContentDialogService>() as ContentDialogService ??
                         throw new InvalidOperationException("Failed to resolve IContentDialogService.");
        });
        services.AddTransient<INotificationService, NotificationService>();
        services.AddSingleton<IContentDialogService>(sp => new ContentDialogService(sp));
        services.AddSingleton<ITaskDialogService, TaskDialogService>();
        services.AddSingleton<IRequestedThemeService>(sp => sp.GetRequiredService<TApp>());
        services.AddSingleton<ILogger>(LoggerFactory.Create(builder => build?.Invoke(builder)).CreateLogger<TApp>());
        configureServices?.Invoke(services);
        provider = services.BuildServiceProvider();        
        return builder;
    }
}

internal static partial class Extensions
{
    public static IEnumerable<string> SplitOnCapitals(this string text)
    {
        var regex = CapitalizedWordRegex();
        foreach (Match match in regex.Matches(text))
        {
            yield return match.Value;
        }
    }

    public static T ToEnum<T>(this string value, T defaultValue = default) where T : struct
    {
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        return Enum.TryParse(value, true, out T result) ? result : defaultValue;
    }

    public static T TryParse<T>(this Enum val) where T : struct => Enum.TryParse(val.ToString(), out T value) ? value : default;

    [GeneratedRegex(@"\p{Lu}\p{Ll}*")]
    private static partial Regex CapitalizedWordRegex();
}
