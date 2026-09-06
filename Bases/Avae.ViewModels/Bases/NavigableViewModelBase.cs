using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Avae.ViewModels;

public abstract partial class NavigableViewModelBase<TResult>(Router router, bool initialize = true) :
    NavigableViewModelBase(router, initialize),
    ICloseableViewModel<TResult>
{
    public event EventHandler<TResult?>? CloseRequested;

    public abstract string Title { get; }

    public virtual Task<bool> CanClose() => Task.FromResult(true);

    private ICommand? closeCommand;

    public ICommand CloseCommand
    {
        get
        {
            return closeCommand ??= new AsyncRelayCommand(async () =>
            {
                if (await CanClose())
                    await Close(default);
            });
        }
    }

    public virtual ObservableCollection<NamedCommand> Commands => [new() { Command = CloseCommand, Name = "Close" }];

    public Task Close(TResult? value)
    {

        CloseRequested?.Invoke(this, value);
        return Task.CompletedTask;
    }
}

/// <summary>
/// This class is used to manage the pages in the application.
/// </summary>
public abstract partial class NavigableViewModelBase : RouterViewModelBase, IViewModelBase 
{
    public EventHandler<IViewFor>? CurrentViewChanged;

    protected override void OnViewModelChanged(IViewModelBase viewModel)
    {
        var type = viewModel.GetType();
        _selectedNavigable = Navigables.First(p => p.ViewModelType == type);
        if (dico.TryGetValue(_selectedNavigable, out var context))
        {
            _currentView = context.Key;
        }
        NotifyPropertyChanged(nameof(SelectedNavigable));
        NotifyPropertyChanged(nameof(CurrentView));
        CurrentViewChanged?.Invoke(this, _currentView);
        base.OnViewModelChanged(viewModel);
    }

    protected abstract void NotifyPropertyChanged(string propertyName);


    /// <summary>
    /// A dictionary to store the context for each page.
    /// </summary>
    private readonly Dictionary<NavigableView, KeyValuePair<IViewFor, IViewModelBase>> dico = [];

    /// <summary>
    /// The currently selected page in the menu.
    /// </summary>
    private IViewFor _currentView = null!;
    public IViewFor CurrentView 
    { 
        get { return _currentView; } 
        set 
        { 
            _currentView = value;
            NotifyPropertyChanged(nameof(CurrentView));
            CurrentViewChanged?.Invoke(this, _currentView);
        } 
    }

    /// <summary>
    /// The currently selected page in the menu.
    /// </summary>
    private NavigableView? _selectedNavigable;
    public NavigableView? SelectedNavigable
    {
        get { return _selectedNavigable; }
        set
        {
            _selectedNavigable = value;
            OnSelectedNavigableChanged(value);
            NotifyPropertyChanged(nameof(SelectedNavigable));
        }
    }

    public NavigableViewModelBase(Router router, bool initialize = true)
        : base(router)
    {
        if (initialize)
        {
            SelectedNavigable = Navigables.FirstOrDefault();
        }
    }

    private ObservableCollection<NavigableView>? _navigables;
    /// <summary>
    /// The list of pages to be displayed in the menu.
    /// </summary>
    public ObservableCollection<NavigableView> Navigables { get {return _navigables ??= GetNavigables(); } }

    protected abstract ObservableCollection<NavigableView> GetNavigables();

    /// <summary>
    /// This method is called when the selected page changes.
    /// </summary>
    /// <param name="value"></param>
    protected async void OnSelectedNavigableChanged(NavigableView? value)
    {
        if (value == null)
            return;

        if (dico.TryGetValue(value, out var view))
        {
            CurrentView = view.Key;
            _router.AddHistory(view.Value);
        }
        else
        {
            var viewFor = GoTo(value, out var viewModel);                  
            dico.Add(value, new KeyValuePair<IViewFor, IViewModelBase>(viewFor, viewModel));
            await value.OnLaunched(viewModel);
            CurrentView = viewFor;
        }

        RaiseCanExecutesChanged();
    }

    protected virtual IViewFor GoTo(NavigableView value, out IViewModelBase viewModel)
    {
        IViewFor viewFor;
        if (value.ViewModel != null)
        {
            viewFor = _router.GoTo(viewModel = value.ViewModel, value.Context);
        }
        else
        {
            viewFor = _router.GoTo(value.ViewModelType, out viewModel, value.Context);
        }

        return viewFor;
    }
}
