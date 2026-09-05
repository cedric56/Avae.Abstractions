using Avae.Core;
using Avae.DAL;
using Avae.Server;
using Avae.DAL.SignalR;
using Example.DAL;
using Example.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Grpc.AspNetCore.Server;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging(AddLoggers);
builder.Services.AddSignalR().AddMessagePackProtocol();
builder.Services.AddMagicOnion();
builder.Services.AddGrpc(AddGrpcOptions);
builder.Services.AddCors(AddCorsOptions);
builder.WebHost.ConfigureKestrel(AddKestrelsOptions);

builder.Services.AddSingleton<ConnectionTracker<Person>>();
builder.Services.AddSingleton<RecordHubRepository<Person>>();
builder.Services.AddSingleton<SignalRHub<Person>>();
builder.Services.UseDBSqlLayer<SqliteConnection>();

var app = builder.Build();

app.UseCors("AllowAll");
app.UseWebSockets();//required for StreamingHub on WebAssembly
app.UseGrpcWebSocketRequestRoutingEnabler();
app.UseRouting();
app.UseGrpcWebSocketBridge();
app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });//required for XmlHttpRequest
app.MapMagicOnionService().EnableGrpcWeb();
app.MapHub<SignalRHub<Person>>("/PersonHub");

ServiceLocator.SetDefault(app.Services);
_ = DBBase.Instance;

app.Run();

void AddLoggers(ILoggingBuilder builder)
{
    builder.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Information);
}

void AddGrpcOptions(GrpcServiceOptions options)
{
    //opt.ResponseCompressionAlgorithm = null;
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = int.MaxValue;
    options.MaxSendMessageSize = int.MaxValue;
}

void AddCorsOptions(CorsOptions options)
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("grpc-status", "grpc-message", "grpc-encoding", "grpc-web-text")
                .SetIsOriginAllowed(origin => true);
    });
}

void AddKestrelsOptions(KestrelServerOptions options)
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
        var certPath = Path.Combine(AppContext.BaseDirectory, "Certificates", "server.pfx");
        if (File.Exists(certPath))
        {
            o.UseHttps(certPath, "Ex@duS56");
        }
    });
}