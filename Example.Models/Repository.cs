namespace Example.Models
{
    public class Repository
    {
        private List<Person>? _persons;
        private static readonly object _lock = new();
        
        private static Repository? _instance = null;
        public static Repository Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new Repository();
                    }
                }
                return _instance;
            }
        }

        public List<Person> Persons
        {
            get
            {
                _persons ??= new(DBBase.Instance.GetAll<Person>());
                return _persons ?? [];
            }
        }


        public async Task ClearPersons()
        {
            _persons = new(await DBBase.Instance.GetAllAsync<Person>());
        }
    }
}
