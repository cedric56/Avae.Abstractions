using Avae.Abstractions;
using Avae.SignalR;
using Example.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("grpc-status", "grpc-message", "grpc-encoding")
            .SetIsOriginAllowed(origin => true);
}));

//// Add services to the container.
builder.Services.AddGrpc(opt =>
{
    opt.EnableDetailedErrors = true;
    opt.MaxReceiveMessageSize = int.MaxValue;
    opt.MaxSendMessageSize = int.MaxValue;
});

builder.Services.AddSingleton<SqlHub<Person>>();
builder.Services.UseDBSqlLayer<SqliteConnection>();
builder.Services.AddMagicOnion();
builder.WebHost.ConfigureKestrel(options =>
{
    //GRPC port
    options.ListenAnyIP(5000, o => o.Protocols = HttpProtocols.Http2);
    //REST port
    options.ListenAnyIP(5001, o => o.Protocols = HttpProtocols.Http1AndHttp2);
});

var app = builder.Build();
ServiceLocator.SetDefault(app.Services);

app.UseCors("AllowAll");
app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });
app.MapMagicOnionService().EnableGrpcWeb();
//var methods = app.Services.GetService<MagicOnionServiceDefinition>()?.MethodHandlers ?? [];
//app.MapMagicOnionSwagger("routes", methods, string.Empty);
//app.MapMagicOnionHttpGateway("routes", methods, GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions()
//{
//    Credentials = ChannelCredentials.Insecure
//}));
app.MapHub<SqlHub<Person>>("/PersonHub");

//Trigger is needed
_ = ServiceLocator.GetRequiredService<SqlHub<Person>>();
_ = DBBase.Instance;

app.Run();