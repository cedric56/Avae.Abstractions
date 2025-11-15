using Avae.Abstractions;
using Avae.DAL;
using Dapper;
using Example.Models;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Example.Dal
{
    public interface IDBBase : IDataAccessLayer
    {
        Task<IEnumerable<Contact>> FindContactByAnyAsync(int? id = null, int? idPerson = null, int? idContact = null);
        IEnumerable<Contact> FindContactByAny(int? id = null, int? idPerson = null, int? idContact = null);
    }

    public class DBBase : DataAccessLayerBase, IDBBase
    {
        private static object _lock = new object();
        private static IDBBase? _instance;
        public static new IDBBase Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DBSqlLayer();
                        }
                    }
                }
                return _instance;
            }
        }


        public Task<IEnumerable<Contact>> FindContactByAnyAsync(int? id = null, int? idPerson = null, int? idContact = null)
        {
            return Instance.FindContactByAnyAsync(id, idPerson, idContact);
        }
        public IEnumerable<Contact> FindContactByAny(int? id = null, int? idPerson = null, int? idContact = null)
        {
            return Instance.FindContactByAny(id, idPerson, idContact);
        }
    }

    public class DBSqlLayer : SqlLayer, IDBBase
    {
        const string findByAnyQuery = "SELECT * FROM Contact WHERE (@id IS NOT NULL AND Id = @id) OR (@idPerson IS NOT NULL AND IdPerson = @idPerson) OR (@idContact IS NOT NULL AND IdContact = @idContact)";

        public Task<IEnumerable<Contact>> FindContactByAnyAsync(int? id = null, int? idPerson = null, int? idContact = null)
        {
            using var db = SimpleProvider.GetService<DbConnection>();            
            return db.QueryAsync<Contact>(findByAnyQuery, new { id, idPerson, idContact });
        }

        public IEnumerable<Contact> FindContactByAny(int? id = null, int? idPerson = null, int? idContact = null)
        {
            using var db = SimpleProvider.GetService<DbConnection>();
            return db.Query<Contact>(findByAnyQuery, new { id, idPerson, idContact });
        }
    }
}
