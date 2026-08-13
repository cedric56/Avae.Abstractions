using Avae.Abstractions;
using Avae.DAL;
using Avae.SignalR;
using Example.DAL;
using Example.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<SignalRHub<Person>>();
builder.Services.UseDBSqlLayer<SqliteConnection>();
builder.Services.AddSignalR();
builder.Services.AddMagicOnion(options =>
{
    // Automatic registration
    //options.EnableStreamingHubHeartbeat = true;

    //// Enable heartbeat for all StreamingHub instances
    //options.EnableStreamingHubHeartbeat = true;
    //// Send heartbeat every 30 seconds, disconnect if no response within 5 seconds
    //options.StreamingHubHeartbeatInterval = TimeSpan.FromSeconds(30);
    //options.StreamingHubHeartbeatTimeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddGrpc(opt =>
{
    opt.EnableDetailedErrors = true;
    opt.MaxReceiveMessageSize = int.MaxValue;
    opt.MaxSendMessageSize = int.MaxValue;
});
builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("grpc-status", "grpc-message", "grpc-encoding")
            .SetIsOriginAllowed(origin => true);
}));
builder.WebHost.ConfigureKestrel(options =>
{
    //GRPC port
    options.ListenAnyIP(5000, o => o.Protocols = HttpProtocols.Http2);
    //REST port
    options.ListenAnyIP(5001, o => o.Protocols = HttpProtocols.Http1AndHttp2);
});

var app = builder.Build();
app.UseCors("AllowAll");
app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });
var endpointConventionBuilder = app.MapMagicOnionService().EnableGrpcWeb();
app.MapHub<SignalRHub<Person>>("/PersonHub");

//Trigger is needed
ServiceLocator.SetDefault(app.Services);
//Launch DBMonitor
_ = ServiceLocator.GetRequiredService<SignalRHub<Person>>();
//Create DB
_ = DBBase.Instance;

app.Run();