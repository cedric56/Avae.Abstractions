using Avae.Core;
using Avae.DAL;
using Avae.Server;
using Avae.DAL.SignalR;
using Example.DAL;
using Example.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ConnectionTracker<Person>>();
builder.Services.AddSingleton<RecordHubRepository<Person>>();
builder.Services.AddSingleton<SignalRHub<Person>>();
builder.Services.UseDBSqlLayer<SqliteConnection>();
builder.Services.AddSignalR().AddMessagePackProtocol();
builder.Services.AddMagicOnion();
builder.Services.AddGrpc(opt =>
{
    //opt.ResponseCompressionAlgorithm = null;
    opt.EnableDetailedErrors = true;
    opt.MaxReceiveMessageSize = int.MaxValue;
    opt.MaxSendMessageSize = int.MaxValue;
});
builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("grpc-status", "grpc-message", "grpc-encoding", "grpc-web-text")
            .SetIsOriginAllowed(origin => true);
}));

builder.WebHost.ConfigureKestrel(options =>
{
    // Desktop/server gRPC clients — HTTP/2 only is fine here (native gRPC channel, not browser)
    options.ListenAnyIP(5000, o =>
    {
        o.Protocols = HttpProtocols.Http2;
        o.UseHttps();
    });

    // Browser-facing gRPC-Web port — MUST allow HTTP/1.1
    options.ListenAnyIP(5001, o =>
    {
        o.Protocols = HttpProtocols.Http1AndHttp2;
        o.UseHttps();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");
//required for StreamingHub on WebAssembly
app.UseWebSockets();
app.UseGrpcWebSocketRequestRoutingEnabler();
app.UseRouting();
app.UseGrpcWebSocketBridge();
//required for XmlHttpRequest
app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });
app.MapMagicOnionService().EnableGrpcWeb();
app.MapHub<SignalRHub<Person>>("/PersonHub");

ServiceLocator.SetDefault(app.Services);
_ = DBBase.Instance;

app.Run();