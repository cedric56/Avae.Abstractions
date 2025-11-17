using Avae.DAL;
using Example.Models;
using MagicOnion;
using MagicOnion.Server;
using MemoryPack;

namespace Example.Server
{
    public class OnionService : ServiceBase<IDbService>, IDbService
    {
        Dictionary<string, Type> Dic = new Dictionary<string, Type>()
        {
            { "Person", typeof(Person) },
            { "Contact", typeof(Contact) }
        };

        SqlLayer Layer = new SqlLayer();

        public UnaryResult<Result> DbTransRemove(DbModelBase modelBase)
        {
            Console.WriteLine(modelBase.GetType());
            return new UnaryResult<Result>(modelBase.DbTransRemove(Layer));
        }

        public UnaryResult<Result> DbTransSave(DbModelBase modelBase)
        {
            Console.WriteLine(modelBase.GetType());
            return new UnaryResult<Result>(modelBase.DbTransSave(Layer));
        }

        public async UnaryResult<byte[]> FindByAnyAsync(string type, object filters)
        {
            var orginal = Dic.GetValueOrDefault(type);

            var method = typeof(SqlLayer).GetMethod(nameof(SqlLayer.FindByAnyAsync));
            var generic = method!.MakeGenericMethod(orginal);
            var task = (Task)generic.Invoke(Layer, new object[] { filters })!;

            // Wait for completion
            await task.ConfigureAwait(false);

            var resultProp = task.GetType().GetProperty("Result");
            var result = resultProp!.GetValue(task);

            // Serialize to byte[]
            return MemoryPackSerializer.Serialize(typeof(List<>).MakeGenericType(orginal), result);
        }

        public async UnaryResult<byte[]> GetAllAsync(string type)
        {
            var orginal = Dic.GetValueOrDefault(type);
            // Get the generic method definition (your real GetAllAsync<T>)
            var method = typeof(SqlLayer).GetMethod(nameof(SqlLayer.GetAllAsync));

            // Create constructed generic method: GetAllAsync<Person>, GetAllAsync<Contact>, etc.
            var generic = method!.MakeGenericMethod(orginal);

            // Invoke the async method
            var task = (Task)generic.Invoke(Layer, new object[] { null, null})!;

            // Wait for completion
            await task.ConfigureAwait(false);

            // Get the result: Task<IEnumerable<T>> → IEnumerable<T>
            var resultProp = task.GetType().GetProperty("Result");
            var result = resultProp!.GetValue(task);

            // Serialize to byte[]
            return MemoryPackSerializer.Serialize(typeof(List<>).MakeGenericType(orginal), result);
        }

        public UnaryResult<byte[]> GetAsync(Type type, int id)
        {
            throw new NotImplementedException();
        }

        public UnaryResult<byte[]> WhereAsync(Type type, object filters)
        {
            throw new NotImplementedException();
        }
    }
}
