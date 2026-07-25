using Microsoft.Extensions.DependencyInjection;

namespace Avae.Abstractions
{
    /// <summary>
    /// Class initially copied from https://github.com/eten-tech/bible-well/blob/main/src/BibleWell.App/Router.cs
    /// </summary>
    public partial class Router(IServiceProvider provider)
    {
        private int _currentIndex = -1;
        private List<IViewModelBase> _history = [];
        private const uint MaxHistorySize = 20;

        public bool CanGoBack => _currentIndex > 0;
        public bool CanGoForward => _history.Count > 0 && _currentIndex < _history.Count - 1;

        public IViewModelBase? Current => _currentIndex < 0 ? null : _history[_currentIndex];

        public event Action<IViewModelBase>? CurrentViewModelChanged;

        public void EraseHistory()
        {
            _currentIndex = -1;
            _history.Clear();
        }

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
        public IContextFor GoTo(Type viewModelType, out IViewModelBase viewModel, NavigationContext? context = null)
        {
            viewModel = provider.GetViewModel(viewModelType, context);
            AddHistory(viewModel);
            CurrentViewModelChanged?.Invoke(viewModel);
            return GetContext(viewModel, context);
        }

        public IContextFor GoTo(Type viewModelType, NavigationContext? context = null)
        {
            return GoTo(viewModelType, out var _, context);
        }

        public IContextFor GoTo<TViewModel>(TViewModel viewModel, NavigationContext? context = null) where TViewModel : IViewModelBase
        {
            AddHistory(viewModel);
            CurrentViewModelChanged?.Invoke(viewModel);
            return GetContext(viewModel, context);
        }

        /// <summary>
        /// Navigates to the view associated with the specified view model type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns>The created view model.</returns>
        public IContextFor GoTo<TViewModel>(out TViewModel viewModel, NavigationContext? context = null) where TViewModel : class, IViewModelBase
        {
            viewModel = provider.GetViewModel<TViewModel>(context);
            AddHistory(viewModel);
            CurrentViewModelChanged?.Invoke(viewModel);
            return GetContext(viewModel, context);
        }

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

        private IContextFor GetContext(IViewModelBase viewModel, NavigationContext? context = null)
        {
            var configuration = provider.GetRequiredService<IIocConfiguration>();
            var contextFor = configuration.GetContextFor(viewModel.GetType().Name, context ?? new NavigationContext());
            //Avoid binding error due to propagating context
            if (contextFor != null)
            {
                contextFor.Context = null;
                contextFor.Context = viewModel;
            }

            return contextFor ?? throw new NotImplementedException($"Unable to find view for {viewModel.GetType().Name}");
        }
    }
}
