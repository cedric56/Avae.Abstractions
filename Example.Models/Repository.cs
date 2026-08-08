using Avae.Abstractions;
using Avae.DAL;
using Avae.DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Models
{
    public class Repository : IDisposable
    {
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
                        _instance ??= new Repository(ServiceLocator.Default);
                    }
                }
                return _instance;
            }
        }

        private readonly ISqlMonitor<Person>? personMonitor;

        private Repository(IServiceProvider provider)
        {
            personMonitor = provider.GetService<ISqlMonitor<Person>>();
            personMonitor?.OnChanged += Monitor_OnChanged;
        }

        private async void Monitor_OnChanged(object? sender, IRecord<Person> e)
        {
            await ClearPersons();
        }

        private List<Person>? _persons;        

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
            personMonitor?.OnChanged -= Monitor_OnChanged;
        }
    }
}
