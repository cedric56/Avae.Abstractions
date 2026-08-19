using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BlazorWebAssemblyExtension;

[assembly: HostingStartup(typeof(BlazorWebAssemblyExtensionStartup))]

namespace BlazorWebAssemblyExtension;

public class BlazorWebAssemblyExtensionStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Register the startup filter
            services.AddSingleton<IStartupFilter, BlazorWebAssemblyExtensionStartupFilter>();

            // You can register other services here as needed
        });
    }
}
