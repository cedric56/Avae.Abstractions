using Avae.Abstractions;
using Avae.Essentials.Core;
using Avae.Services;
using Microsoft.Extensions.Logging;
using UXDivers.Popups.Maui;

namespace Avae.Maui
{
    public static class Extensions
    {
        public static MauiAppBuilder ConfigureIocContainer<TApp>(this MauiAppBuilder builder,
            Action<IIocContainer>? configure = null,
            Action<ILoggingBuilder>? build = null)
            where TApp : Application
        {
            builder.Services.RegisterEssentials();

            //WindowsToastNotifyApi.Toast.Initialize("test", "here");

            builder.UseUXDiversPopups();
            builder.Services.AddSingleton<IIocContainer>(sp => new IocContainer(GetConfiguration(sp), false));
            builder.Services.AddSingleton<IIocConfiguration>(sp => new IocConfiguration(sp, () => (IocContainer)sp.GetRequiredService<IIocContainer>(), configure));
            builder.Services.AddTransient<Router>(sp => new Router(sp));
            builder.Services.AddSingleton<IDialogService>(GetConfiguration);
            builder.Services.AddSingleton<IContentDialogService>(GetConfiguration);
            builder.Services.AddSingleton<ITaskDialogService>(GetConfiguration);
            builder.Services.AddSingleton<ISystemNotificationService>(GetConfiguration);
            builder.Services.AddSingleton<INotificationService>(GetConfiguration);
            builder.Services.AddSingleton<IRequestedThemeService>(GetConfiguration);
            builder.Services.AddSingleton<ILogger>(LoggerFactory.Create(builder =>
            {
                build?.Invoke(builder);

            }).CreateLogger<TApp>());
            return builder;

            IocConfiguration GetConfiguration(IServiceProvider provider)
            {
                return (IocConfiguration)provider.GetRequiredService<IIocConfiguration>();
            }
        }
    }
}
