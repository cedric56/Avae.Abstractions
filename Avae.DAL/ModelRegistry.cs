//namespace Avae.DAL
//{
//    public static class ModelRegistry
//    {
//        private static readonly Dictionary<int, Type> _models = new();
//        private static readonly Dictionary<Type, int> _modelIds = new();
//        private static int _nextId = 1;

//        public static void Register<T>(int? id = null) where T : DBModelBase
//        {
//            var typeId = id ?? _nextId++;
//            _models[typeId] = typeof(T);
//            _modelIds[typeof(T)] = typeId;
//        }

//        public static Type? GetType(int id)
//        {
//            return _models.TryGetValue(id, out var type) ? type : null;
//        }

//        public static int? GetId(Type type)
//        {
//            return _modelIds.TryGetValue(type, out var id) ? id : null;
//        }

//        public static int? GetId<T>() where T : DBModelBase
//        {
//            return GetId(typeof(T));
//        }

//        public static IEnumerable<Type> GetAllTypes()
//        {
//            return _models.Values;
//        }
//    }
//}
