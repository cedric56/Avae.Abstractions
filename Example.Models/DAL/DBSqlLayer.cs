using Avae.DAL;

namespace Example.Models
{
    public class DBSqlLayer : SqlLayer, IDbLayer
    {
        public Task<Result> DbTransRemove(DbModelBase modelBase)
        {
            return modelBase.DbTransRemove(this);
        }

        public Task<Result> DbTransSave(DbModelBase modelBase)
        {
            return modelBase.DbTransSave(this);
        }
    }
}
