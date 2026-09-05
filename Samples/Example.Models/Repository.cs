using Avae.Core;
using Avae.DAL;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Models;

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

    private readonly IDBMonitor<Person>? personMonitor;

    private Repository(IServiceProvider provider)
    {
        personMonitor = provider.GetService<IDBMonitor<Person>>();
        personMonitor?.OnRecordChanged += Monitor_OnChanged;
    }

    private async void Monitor_OnChanged(object? sender, Record<Person> e)
    {
        await ClearPersons();
    }

    private IEnumerable<Person>? _persons;

    public IEnumerable<Person> Persons
    {
        get
        {
            _persons ??= DBBase.Instance.GetAll<Person>();
            return _persons ?? [];
        }
    }

    public event EventHandler<EventArgs>? PersonsChanged;

    public async Task ClearPersons()
    {
        _persons = await DBBase.Instance.GetAllAsync<Person>();
        PersonsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        personMonitor?.OnRecordChanged -= Monitor_OnChanged;
    }
}
