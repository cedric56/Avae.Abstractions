using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Avae.ViewModels;

/// <summary>
/// Base class for a navigable view model that can be closed and return a result value.
/// </summary>
/// <typeparam name="TResult">The type of the result produced when this view model is closed.</typeparam>
/// <param name="router">The router used to manage navigation between views.</param>
/// <param name="initialize">
/// If <see langword="true"/>, the first available navigable item is selected automatically on construction.
/// </param>
public abstract partial class NavigableViewModelBase<TResult>(Router router, bool initialize = true) :
    NavigableViewModelBase(router, initialize),
    ICloseableViewModel<TResult>
{
    /// <summary>
    /// Occurs when a close has been requested for this view model, carrying the resulting value, if any.
    /// </summary>
    public event EventHandler<TResult?>? CloseRequested;

    /// <summary>
    /// Gets the display title for this view model.
    /// </summary>
    public abstract string Title { get; }

    /// <summary>
    /// Determines whether this view model can currently be closed.
    /// </summary>
    /// <returns>
    /// A task that resolves to <see langword="true"/> if closing is allowed; otherwise, <see langword="false"/>.
    /// The base implementation always returns <see langword="true"/>.
    /// </returns>
    public virtual Task<bool> CanClose() => Task.FromResult(true);

    private ICommand? closeCommand;

    /// <summary>
    /// Gets the command that, when executed, checks <see cref="CanClose"/> and closes the view model if allowed.
    /// </summary>
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

    /// <summary>
    /// Gets the collection of commands exposed by this view model. By default, contains only the close command.
    /// </summary>
    public virtual ObservableCollection<NamedCommand> Commands => [new() { Command = CloseCommand, Name = "Close" }];

    /// <summary>
    /// Closes this view model, raising <see cref="CloseRequested"/> with the supplied result value.
    /// </summary>
    /// <param name="value">The result value to pass to subscribers of <see cref="CloseRequested"/>.</param>
    /// <returns>A completed task.</returns>
    public Task Close(TResult? value)
    {

        CloseRequested?.Invoke(this, value);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Base class for a view model that manages navigation between a set of <see cref="NavigableView"/> items,
/// caching the view/view-model pair for each one as it is visited.
/// </summary>
public abstract partial class NavigableViewModelBase : RouterViewModelBase, IViewModelBase
{
    /// <summary>
    /// Occurs when the currently displayed view changes.
    /// </summary>
    public EventHandler<IViewFor>? CurrentViewChanged;

    /// <summary>
    /// Updates <see cref="SelectedNavigable"/> and <see cref="CurrentView"/> to reflect a change in the
    /// active view model, and raises the corresponding change notifications.
    /// </summary>
    /// <param name="viewModel">The view model that has become active.</param>
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

    /// <summary>
    /// Raises a property-changed notification for the specified property.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected abstract void NotifyPropertyChanged(string propertyName);

    /// <summary>
    /// Cache mapping each <see cref="NavigableView"/> to the view/view-model pair created for it,
    /// so that previously visited navigables are not recreated.
    /// </summary>
    private readonly Dictionary<NavigableView, KeyValuePair<IViewFor, IViewModelBase>> dico = [];

    private IViewFor _currentView = null!;

    /// <summary>
    /// Gets or sets the view currently being displayed. Setting this property raises
    /// <see cref="CurrentViewChanged"/> and a property-changed notification.
    /// </summary>
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

    private NavigableView? _selectedNavigable;

    /// <summary>
    /// Gets or sets the currently selected navigable item. Setting this property triggers navigation
    /// to the corresponding view via <see cref="OnSelectedNavigableChanged"/>.
    /// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigableViewModelBase"/> class.
    /// </summary>
    /// <param name="router">The router used to manage navigation between views.</param>
    /// <param name="initialize">
    /// If <see langword="true"/>, <see cref="SelectedNavigable"/> is set to the first available
    /// navigable item on construction.
    /// </param>
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
    /// Gets the collection of navigable items available to this view model, lazily populated
    /// via <see cref="GetNavigables"/> on first access.
    /// </summary>
    public ObservableCollection<NavigableView> Navigables { get { return _navigables ??= GetNavigables(); } }

    /// <summary>
    /// When implemented in a derived class, returns the collection of navigable items this view model exposes.
    /// </summary>
    /// <returns>The collection of available <see cref="NavigableView"/> items.</returns>
    protected abstract ObservableCollection<NavigableView> GetNavigables();

    /// <summary>
    /// Handles a change to <see cref="SelectedNavigable"/> by resolving (or creating) the associated
    /// view/view-model pair, updating <see cref="CurrentView"/>, and recording navigation history.
    /// </summary>
    /// <param name="value">The newly selected navigable item, or <see langword="null"/> if none is selected.</param>
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

    /// <summary>
    /// Navigates to the view associated with the specified navigable item, creating its view model
    /// if one is not already assigned.
    /// </summary>
    /// <param name="value">The navigable item to navigate to.</param>
    /// <param name="viewModel">
    /// When this method returns, contains the view model used for navigation—either <paramref name="value"/>'s
    /// existing view model, or a newly created one.
    /// </param>
    /// <returns>The view resolved for the navigation target.</returns>
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