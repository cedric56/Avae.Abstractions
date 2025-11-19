using Avae.Abstractions;
using Avae.DAL;
using Example.Models;
using MagicOnion;
using MagicOnion.Server;
using MemoryPack;
using System.Buffers;

namespace Example.Server
{

    public class OnionService : ServiceBase<IDbService>, IDbService
    {
        //private static readonly Dictionary<string, IEntityHandler> Handlers = new()
        //{
        //    ["Person"] = new EntityHandler<Person>(),
        //    ["Contact"] = new EntityHandler<Contact>(),
        //    // Ajoute les autres ici
        //};

        //// On ne stocke QUE le buffer réutilisable par thread
        //[ThreadStatic]
        //private static ArrayBufferWriter<byte>? t_buffer;

        //private static ArrayBufferWriter<byte> GetBuffer()
        //{
        //    t_buffer ??= new ArrayBufferWriter<byte>(16 * 1024); // 16 KB initial
        //    t_buffer.Clear();
        //    return t_buffer;
        //}

        //private interface IEntityHandler
        //{
        //    ValueTask SerializeGetAllAsync(ArrayBufferWriter<byte> buffer);
        //    ValueTask SerializeGetAsync(ArrayBufferWriter<byte> buffer, int id);
        //    ValueTask SerializeFindByAnyAsync(ArrayBufferWriter<byte> buffer, object filters);
        //    ValueTask SerializeWhereAsync(ArrayBufferWriter<byte> buffer, object filters);
        //}

        //private sealed class EntityHandler<T> : IEntityHandler where T : class, new()
        //{
        //    public async ValueTask SerializeGetAllAsync(ArrayBufferWriter<byte> buffer)
        //    {
        //        var data = await Layer.GetAllAsync<T>();
        //        MemoryPackSerializer.Serialize(buffer, data); // direct sur le buffer
        //    }

        //    public async ValueTask SerializeGetAsync(ArrayBufferWriter<byte> buffer, int id)
        //    {
        //        var item = await Layer.GetAsync<T>(id);
        //        MemoryPackSerializer.Serialize(buffer, item ?? (T?)null); // null géré proprement
        //    }

        //    public async ValueTask SerializeFindByAnyAsync(ArrayBufferWriter<byte> buffer, object filters)
        //    {
        //        var data = await Layer.FindByAnyAsync<T>(filters);
        //        MemoryPackSerializer.Serialize(buffer, data);
        //    }

        //    public async ValueTask SerializeWhereAsync(ArrayBufferWriter<byte> buffer, object filters)
        //    {
        //        var data = await Layer.WhereAsync<T>(filters);
        //        MemoryPackSerializer.Serialize(buffer, data);
        //    }
        //}

        //private static IDbLayer Layer = SimpleProvider.GetService<IDbLayer>();

        //public UnaryResult<Result> DbTransRemove(DBModelBase modelBase)
        //{
        //    return new UnaryResult<Result>(modelBase.DbTransRemove(Layer));
        //}

        //public UnaryResult<Result> DbTransSave(DBModelBase modelBase)
        //{
        //    return new UnaryResult<Result>(modelBase.DbTransSave(Layer));
        //}

        //public async UnaryResult<byte[]> GetAllAsync(string type)
        //{
        //    if (!Handlers.TryGetValue(type, out var handler))
        //        throw new KeyNotFoundException($"Type {type} not registered");

        //    var buffer = GetBuffer();
        //    await handler.SerializeGetAllAsync(buffer);
        //    return buffer.WrittenSpan.ToArray();
        //}

        //public async UnaryResult<byte[]> GetAsync(string type, int id)
        //{
        //    if (!Handlers.TryGetValue(type, out var handler))
        //        throw new KeyNotFoundException($"Type {type} not registered");

        //    var buffer = GetBuffer();
        //    await handler.SerializeGetAsync(buffer, id);
        //    return buffer.WrittenSpan.ToArray();
        //}

        //public async UnaryResult<byte[]> FindByAnyAsync(string type, object filters)
        //{
        //    if (!Handlers.TryGetValue(type, out var handler))
        //        throw new KeyNotFoundException($"Type {type} not registered");

        //    var buffer = GetBuffer();
        //    await handler.SerializeFindByAnyAsync(buffer, filters);
        //    return buffer.WrittenSpan.ToArray();
        //}

        //public async UnaryResult<byte[]> WhereAsync(string type, object filters)
        //{
        //    if (!Handlers.TryGetValue(type, out var handler))
        //        throw new KeyNotFoundException($"Type {type} not registered");

        //    var buffer = GetBuffer();
        //    await handler.SerializeWhereAsync(buffer, filters);
        //    return buffer.WrittenSpan.ToArray();
        //}

        private abstract class Test
        {
            public abstract Type Type { get; }
            public abstract Type Enumerable { get; }

            public abstract Task<object> GetAllAsync();
            public abstract Task<object> GetAsync(int id);
            public abstract Task<object> FindByAnyAsync(Dictionary<string, object> filters);
            public abstract Task<object> WhereAsync(Dictionary<string, object> filters);
        }
        private class Test<T>() : Test where T : class, new() {

            public override Type Type => typeof(T);

            public override Type Enumerable => typeof(IEnumerable<T>);

            public override async Task<object> GetAllAsync()
            {
                return await Layer.GetAllAsync<T>();
            }
            public override async Task<object> GetAsync(int id)
            {
                return await Layer.GetAsync<T>(id);
            }
            public override async Task<object> FindByAnyAsync(Dictionary<string, object> filters)
            {
                return await Layer.FindByAnyAsync<T>(filters);
            }
            public override async Task<object> WhereAsync(Dictionary<string, object> filters)
            {
                return await Layer.WhereAsync<T>(filters);
            }
        }

        Dictionary<string, Test> Dic = new Dictionary<string, Test>()
        {
            { "Person", new Test<Person>() },
            { "Contact", new Test<Contact>() }
        };

        private static IDbLayer Layer = SimpleProvider.GetService<IDbLayer>();

        public async UnaryResult<Result> DbTransRemove(DBModelBase modelBase)
        {
            return await modelBase.DbTransRemove(Layer);
        }

        public async UnaryResult<Result> DbTransSave(DBModelBase modelBase)
        {
            return await modelBase.DbTransSave(Layer);
        }

        public async UnaryResult<byte[]> FindByAnyAsync(string type, Dictionary<string, object> filters)
        {
            var value = Dic.GetValueOrDefault(type);
            return MemoryPackSerializer.Serialize(value.Enumerable, await value.FindByAnyAsync(filters));
        }

        public async UnaryResult<byte[]> GetAllAsync(string type)
        {
            var value = Dic.GetValueOrDefault(type);
            return MemoryPackSerializer.Serialize(value.Enumerable, await value.GetAllAsync());
        }

        public async UnaryResult<byte[]> GetAsync(string type, int id)
        {
            var value = Dic.GetValueOrDefault(type);
            return MemoryPackSerializer.Serialize(value.Type, await value.GetAsync(id));
        }

        public async UnaryResult<byte[]> WhereAsync(string type, Dictionary<string, object> filters)
        {
            var value = Dic.GetValueOrDefault(type);
            return MemoryPackSerializer.Serialize(value.Enumerable, await value.WhereAsync(filters));
        }
    }
}
