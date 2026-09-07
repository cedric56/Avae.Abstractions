using Microsoft.Extensions.DependencyInjection;

namespace Avae.ViewModels;

/// <summary>
/// Class initially copied from https://github.com/eten-tech/bible-well/blob/main/src/BibleWell.App/Router.cs
/// </summary>
public partial class Router(IServiceProvider provider)
{
    private int _currentIndex = -1;
    private List<IViewModelBase> _history = [];
    private const uint MaxHistorySize = 20;

    /// <summary>
    /// Gets a value indicating whether there is history to navigate back to.
    /// </summary>
    public bool CanGoBack => _currentIndex > 0;

    /// <summary>
    /// Gets a value indicating whether there is history to navigate forward to.
    /// </summary>
    public bool CanGoForward => _history.Count > 0 && _currentIndex < _history.Count - 1;

    /// <summary>
    /// Gets the view model currently at the front of navigation history, or <see langword="null"/> if history is empty.
    /// </summary>
    public IViewModelBase? Current => _currentIndex < 0 ? null : _history[_currentIndex];

    /// <summary>
    /// Occurs whenever the current view model changes, whether via <see cref="Back"/>, <see cref="Forward"/>,
    /// or one of the <c>GoTo</c> overloads.
    /// </summary>
    public event Action<IViewModelBase>? CurrentViewModelChanged;

    /// <summary>
    /// Clears all navigation history and resets the current position.
    /// </summary>
    public void EraseHistory()
    {
        _currentIndex = -1;
        _history.Clear();
    }

    /// <summary>
    /// Moves back one step in navigation history, if possible.
    /// </summary>
    /// <returns>The view model now current after moving back, or <see langword="null"/> if <see cref="CanGoBack"/> was <see langword="false"/>.</returns>
    public IViewModelBase? Back()
    {
        if (!CanGoBack)
        {
            return null;
        }

        _currentIndex--;
        CurrentViewModelChanged?.Invoke(Current!);
        return Current;
    }

    /// <summary>
    /// Moves forward one step in navigation history, if possible.
    /// </summary>
    /// <returns>The view model now current after moving forward, or <see langword="null"/> if <see cref="CanGoForward"/> was <see langword="false"/>.</returns>
    public IViewModelBase? Forward()
    {
        if (!CanGoForward)
        {
            return null;
        }

        _currentIndex++;
        CurrentViewModelChanged?.Invoke(Current!);
        return Current;
    }

    /// <summary>
    /// Navigates to the view associated with the specified view model type.
    /// If you directly know the type of the view model at compile time, use <see cref="GoTo{T}()"/> instead.
    /// </summary>
    /// <typeparam name="TBaseType">The base type of the view model.</typeparam>
    /// <param name="viewModelType">The view model type.</param>
    /// <returns>The created view model cast to the <typeparamref name="TBaseType"/>.</returns>        
    public IViewFor GoTo(Type viewModelType, out IViewModelBase viewModel, NavigableContext? context = null)
    {
        viewModel = provider.GetViewModel(viewModelType, context);
        AddHistory(viewModel);
        CurrentViewModelChanged?.Invoke(viewModel);
        return GetViewFor(viewModel, context);
    }

    /// <summary>
    /// Navigates to the view associated with the specified view model type, without exposing the created view model.
    /// </summary>
    /// <param name="viewModelType">The view model type to navigate to.</param>
    /// <param name="context">Optional navigation context supplying parameters for the view model, view, and factory.</param>
    /// <returns>The view resolved for the created view model.</returns>
    public IViewFor GoTo(Type viewModelType, NavigableContext? context = null)
    {
        return GoTo(viewModelType, out var _, context);
    }

    /// <summary>
    /// Navigates to the view associated with an already-created view model instance.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <param name="viewModel">The existing view model instance to navigate to.</param>
    /// <param name="context">Optional navigation context supplying parameters for the view.</param>
    /// <returns>The view resolved for <paramref name="viewModel"/>.</returns>
    public IViewFor GoTo<TViewModel>(TViewModel viewModel, NavigableContext? context = null) where TViewModel : IViewModelBase
    {
        AddHistory(viewModel);
        CurrentViewModelChanged?.Invoke(viewModel);
        return GetViewFor(viewModel, context);
    }

    /// <summary>
    /// Navigates to the view associated with the specified view model type.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <returns>The created view model.</returns>
    public IViewFor GoTo<TViewModel>(out TViewModel viewModel, NavigableContext? context = null) where TViewModel : class, IViewModelBase
    {
        viewModel = provider.GetViewModel<TViewModel>(context);
        AddHistory(viewModel);
        CurrentViewModelChanged?.Invoke(viewModel);
        return GetViewFor(viewModel, context);
    }

    /// <summary>
    /// Appends a view model to navigation history and makes it current, truncating any "forward" history
    /// beyond the current position and trimming the oldest entry if <see cref="MaxHistorySize"/> is exceeded.
    /// </summary>
    /// <param name="item">The view model to add to history.</param>
    public void AddHistory(IViewModelBase item)
    {
        // After navigating back the current index may not be the most forward position.
        // Delete all "forward" items in the history when this happens.
        if (CanGoForward)
        {
            _history = [.. _history.Take(_currentIndex + 1)];
        }

        // add the item and recalculate the index
        _history.Add(item);

        // history exceeded the max size
        if (_history.Count > MaxHistorySize)
        {
            _history.RemoveAt(0);
        }

        _currentIndex = _history.Count - 1;
    }

    /// <summary>
    /// Resolves the view associated with the specified view model via the registered <see cref="IIocConfiguration"/>,
    /// and assigns the view model as the view's context.
    /// </summary>
    /// <param name="viewModel">The view model to resolve a view for.</param>
    /// <param name="context">Optional navigation context; an empty context is used if not supplied.</param>
    /// <returns>The resolved view, with <paramref name="viewModel"/> assigned as its context.</returns>
    /// <exception cref="NotImplementedException">Thrown if no view is registered for the view model's type.</exception>
    private IViewFor GetViewFor(IViewModelBase viewModel, NavigableContext? context = null)
    {
        var name = viewModel.GetType().Name;
        var configuration = provider.GetRequiredService<IIocConfiguration>();
        var viewFor = configuration.GetContextFor(name, context ?? new NavigableContext());
        viewFor?.Context = viewModel;
        return viewFor ?? throw new NotImplementedException($"Unable to find view for {name}");
    }
}