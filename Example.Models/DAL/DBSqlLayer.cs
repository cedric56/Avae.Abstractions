using Avae.DAL;

namespace Example.Models
{
    public class DBSqlLayer : SqlLayer, IDbLayer
    {
        public Task<Result> DbTransRemove(DBModelBase modelBase)
        {
            return modelBase.DbTransRemove(this);
        }

        public Task<Result> DbTransSave(DBModelBase modelBase)
        {
            return modelBase.DbTransSave(this);
        }
    }
}
