using Avae.Abstractions;
using Avae.DAL;
using Avae.SignalR;
using Example.DAL;
using Example.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<SqlHub<Person>>();
builder.Services.UseDBSqlLayer<SqliteConnection>();
builder.Services.AddSignalR();
builder.Services.AddMagicOnion();
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
app.MapMagicOnionService().EnableGrpcWeb();
app.MapHub<SqlHub<Person>>("/PersonHub");

//Trigger is needed
ServiceLocator.SetDefault(app.Services);
//Launch DBMonitor
_ = ServiceLocator.GetRequiredService<SqlHub<Person>>();
//Create DB
_ = DBBase.Instance;

app.Run();