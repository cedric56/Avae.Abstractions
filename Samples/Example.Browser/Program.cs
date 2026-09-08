using Avae.DAL;
using Avalonia;
using Avalonia.Browser;
using Avalonia.Labs.Notifications;
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
        => App.CreateApp<BrowserApp>(configure: services => services.UseDBOnionLayer())
                        .WithAppNotifications();

    public class BrowserApp : App
    {
        public BrowserApp(IServiceProvider provider)
            : base(provider) { }

        Func<Task>? unsuscribe = null;
        protected override async Task AfterCompletedAsync()
        {
            var monitor = provider.GetRequiredService<IDBMonitor<Person>>();
            Repository.Initialize(monitor);
            DBBase.Initialize(provider.GetRequiredService<IDBLayer>());
            //unsuscribe = await Container.Provider.AddSignalR(monitor);
            unsuscribe = await monitor.AddStreamingHub();
        }

        public override async void Dispose()
        {
            if (unsuscribe != null)
                await unsuscribe();

            base.Dispose();
        }
    }
}

