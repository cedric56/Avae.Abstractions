using Avae.Abstractions;
using Avae.DAL;

namespace Example.Models
{
    public class Repository : IDisposable
    {
        private ISqlMonitor<Person>? personMonitor;

        private Repository()
        {
            personMonitor = SimpleProvider.GetService<ISqlMonitor<Person>>();
            if (personMonitor != null)
            {
                personMonitor.OnChanged += Monitor_OnChanged;
            }
        }

        private async void Monitor_OnChanged(object? sender, IRecord<Person> e)
        {
            await ClearPersons();
        }

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
                        if (_instance is null)
                        {
                            _instance = new Repository();
                        }
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

        public void Dispose()
        {
            if (personMonitor != null)
                personMonitor.OnChanged -= Monitor_OnChanged;
        }
    }
}
