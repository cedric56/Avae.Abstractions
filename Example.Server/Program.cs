using Avae.Abstractions;
using Example.Models;
using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Server;
using MemoryPack;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using System.Data.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true);
    //.WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

//// Add services to the container.
builder.Services.AddGrpc(opt =>
{
    opt.EnableDetailedErrors = true;
    opt.MaxReceiveMessageSize = int.MaxValue;
    opt.MaxSendMessageSize = int.MaxValue;
});
builder.Services.AddTransient<DbConnection>(_ => new SqliteConnection("Data Source=data.db;Foreign Keys=True"));
builder.Services.AddSingleton<IDbLayer>(_ => new DBSqlLayer());
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
builder.Services.AddTransient<DbConnection>(_ => new SqliteConnection("Data Source=data.db;Foreign Keys=True"));


var app = builder.Build();
SimpleProvider.ConfigureServices(app.Services);

using var connection = SimpleProvider.GetService<DbConnection>();
connection.Open();

using var cmd = connection.CreateCommand();
cmd.CommandText = GetCommandText(connection);
cmd.ExecuteNonQuery();

// 2. Activer CORS dans le pipeline
//app.UseCors("AllowAvaloniaWasm");
app.UseCors("AllowAll");
app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });
app.MapMagicOnionService().EnableGrpcWeb();//.RequireCors("AllowAll");
app.MapMagicOnionHttpGateway("_", app.Services.GetService<MagicOnionServiceDefinition>().MethodHandlers, GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions()
{
    Credentials = ChannelCredentials.Insecure
}));

app.Run();

string GetCommandText(DbConnection connection)
{
    if (connection is SqliteConnection sqlite)
    {
        return @"
            CREATE TABLE IF NOT EXISTS Person(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FirstName TEXT,
                LastName TEXT
            );

            CREATE TABLE IF NOT EXISTS Contact(
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            IdPerson INTEGER  NOT NULL,
                            IdContact INTEGER  NOT NULL,
                            CONSTRAINT FK_Contact_Person FOREIGN KEY(IdPerson) REFERENCES Person(Id),
                            CONSTRAINT FK_Contact_ContactPerson FOREIGN KEY(IdContact) REFERENCES Person(Id)
                        );
            ";
    }
    else if (connection is SqlConnection sqlServer)
    {
        return @"CREATE TABLE IF NOT EXISTS Person (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        FirstName NVARCHAR(255) NULL,
                        LastName NVARCHAR(255) NULL,
                        Photo VARBINARY(MAX) NULL
                    );

                    CREATE TABLE IF NOT EXISTS Contact (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        IdPerson INT NOT NULL,
                        IdContact INT NOT NULL,
                        CONSTRAINT FK_Contact_Person FOREIGN KEY (IdPerson) REFERENCES Person(Id),
                        CONSTRAINT FK_Contact_ContactPerson FOREIGN KEY (IdContact) REFERENCES Person(Id)
                    );";
    }

    throw new NotImplementedException();
}