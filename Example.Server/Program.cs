using Avae.Abstractions;
using Avae.DAL;
using Example.Models;
using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
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
builder.Services.UseDbLayer<IDBLayer, DBSqlLayer>();

var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
var dbPath = Path.Combine(folder, "database.db");
var connectionString = $"Data Source={dbPath};Foreign Keys=True";

builder.Services.UseSqlMonitors<SqliteConnection>(connectionString, factory =>
{
    factory.AddDbMonitor<Person>();
});
builder.Services.AddMagicOnion();
builder.WebHost.ConfigureKestrel(options =>
{
    //GRPC port
    options.ListenAnyIP(5000, o =>
    {
        o.Protocols = HttpProtocols.Http2;
        //for non-container based development
        //o.UseHttps("../dev/certs/backend.pfx", "complexpassword1");
    });
    //REST port
    options.ListenAnyIP(5001, o => o.Protocols = HttpProtocols.Http1AndHttp2);
});

var app = builder.Build();
SimpleProvider.ConfigureServices(app.Services);

app.UseCors("AllowAll");
app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });
app.MapMagicOnionService().EnableGrpcWeb();
var methods = app.Services.GetService<MagicOnionServiceDefinition>()?.MethodHandlers ?? [];
app.MapMagicOnionSwagger("routes", methods, string.Empty);
app.MapMagicOnionHttpGateway("routes", methods, GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions()
{
    Credentials = ChannelCredentials.Insecure
}));
app.MapHub<SqlHub<Person>>("/PersonHub");
app.Run();