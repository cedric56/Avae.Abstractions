using Avae.Abstractions;
using Avae.DAL;
using Avae.DAL.Interfaces;
using Example.Models;
using Example.Razor;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using MagicOnion;
using MagicOnion.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.ConfigureProject();

builder.Services.AddScoped<IDBOnionService>(_ => GetMagicOnion<IDBOnionService>());
builder.Services.AddScoped<IOnionService>(provider => provider.GetRequiredService<IDBOnionService>());
builder.Services.AddTransient<IXmlHttpRequest, XmlHttpRequest>();
builder.Services.AddScoped<IDBLayer>(provider => new DBOnionLayer(provider));
builder.Services.AddScoped<IDataAccessLayer>(provider => provider.GetRequiredService<IDBLayer>());
IGrpc GetMagicOnion<IGrpc>() where IGrpc : IService<IGrpc>
{
    var client = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
    {
        DefaultRequestVersion = HttpVersion.Version20,
        Timeout = TimeSpan.FromSeconds(5)
    };
    var channel = GrpcChannel.ForAddress(
        "http://localhost:5001", new GrpcChannelOptions()
        {
            HttpClient = client,
        });
    return MagicOnionClient.Create<IGrpc>(channel);
}

var app = builder.Build();
ServiceLocator.SetDefault(app.Services);
await app.RunAsync();
   