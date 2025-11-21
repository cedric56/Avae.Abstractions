using Avae.Abstractions;
using Avae.DAL;
using Example.Models;
using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using System.Data.Common;

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
builder.Services.AddSingleton<IDbLayer>(_ => new DBSqlLayer());
builder.Services.AddSingleton<SqlHub<Person>>();
builder.Services.AddSingleton<IDataAccessLayer>(provider => provider.GetRequiredService<IDbLayer>());
builder.Services.AddSingleton<IDbFactory>(_ =>
{
    var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    var dbPath = Path.Combine(folder, "database.db");
    var factory = new SqlFactory<SqliteConnection>($"Data Source={dbPath};Foreign Keys=True");
    factory.AddDbMonitor<Person>();
    return factory;
});
builder.Services.AddTransient<DbConnection>(provider =>
{
    var factory = provider.GetRequiredService<IDbFactory>();
    return factory.CreateConnection()!;
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
//builder.Services.AddTransient<DbConnection>(_ => new SqliteConnection("Data Source=data.db;Foreign Keys=True"));


var app = builder.Build();
SimpleProvider.ConfigureServices(app.Services);

using var connection = SimpleProvider.GetService<DbConnection>();
connection.Open();

using var cmd = connection.CreateCommand();
cmd.CommandText = GetCommandText(connection);
cmd.ExecuteNonQuery();

app.UseCors("AllowAll");
app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });
app.MapMagicOnionService().EnableGrpcWeb();
app.MapMagicOnionHttpGateway("_", app.Services.GetService<MagicOnionServiceDefinition>().MethodHandlers, GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions()
{
    Credentials = ChannelCredentials.Insecure
}));
app.MapHub<SqlHub<Person>>("/PersonHub");
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