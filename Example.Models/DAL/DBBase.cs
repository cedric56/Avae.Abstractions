using Avae.Abstractions;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace Example.Models
{
    public class DBBase
    {
        private static string GetCommandText(DbConnection connection)
        {
            if (connection is SqliteConnection)
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
            else if (connection is SqlConnection)
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

        private static readonly object _lock = new();
        private static IDBLayer? _instance;
        public static IDBLayer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            if (!OperatingSystem.IsBrowser())
                            {
                                //Create db
                                using var connection = SimpleProvider.GetService<DbConnection>();
                                if (connection is not null)
                                {
                                    connection.Open();

                                    using var cmd = connection.CreateCommand();
                                    cmd.CommandText = GetCommandText(connection);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            _instance = SimpleProvider.GetService<IDBLayer>();                            
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
