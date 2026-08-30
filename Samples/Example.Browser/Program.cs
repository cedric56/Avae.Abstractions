using Avae.DAL;
using Avalonia;
using Avalonia.Browser;
using Avalonia.Labs.Notifications;
using Avalonia.Media;
using Example;
using Example.DAL;
using Example.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

internal sealed partial class Program
{
    private static Task Main(string[] args) => 
        BuildAvaloniaApp().StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<BrowserApp>()
                        .WithAppNotifications()
                        .WithInterFont();

    public class BrowserApp : App
    {
        public override async void Configure(IServiceCollection services)
        {
            base.Configure(services);

            services.UseDBOnionLayer();
        }
        Func<Task> unsuscribe = null;
        protected override async Task AfterCompletedAsync()
        {
            var monitor = Container.Provider.GetRequiredService<IDBMonitor<Person>>();

            //unsuscribe = await Container.Provider.AddSignalR(monitor);
            unsuscribe = await Container.Provider.AddStreamingHub(monitor);
        }

        public override async void Dispose()
        {
            if (unsuscribe != null)
                await unsuscribe();

            base.Dispose();
        }
    }
}

