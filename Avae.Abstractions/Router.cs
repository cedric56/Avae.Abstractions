namespace Avae.Abstractions
{
    /// <summary>
    /// Class initially copied from https://github.com/eten-tech/bible-well/blob/main/src/BibleWell.App/Router.cs
    /// </summary>
    public partial class Router
    {
        private readonly object _lock = new object();
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
        public IContextFor GoTo(Type viewModelType, params IParameter[] parameters)
        {
            return GoTo(viewModelType, out var _, parameters);
        }

        public IContextFor GoTo(Type viewModelType, out IViewModelBase viewModel,  params IParameter[] parameters)
        {
            lock (_lock)
            {
                viewModel = SimpleProvider.GetViewModel(viewModelType, parameters);
                AddHistory(viewModel);
                CurrentViewModelChanged?.Invoke(viewModel);
                return GetContext(viewModel, parameters);
            }
        }

        public IContextFor GoTo<TViewModel>(TViewModel viewModel, params IParameter[] parameters) where TViewModel : IViewModelBase
        {
            lock (_lock)
            {
                AddHistory(viewModel);
                CurrentViewModelChanged?.Invoke(viewModel);
                return GetContext(viewModel, parameters);
            }
        }

        /// <summary>
        /// Navigates to the view associated with the specified view model type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns>The created view model.</returns>
        public IContextFor GoTo<TViewModel>(params IParameter[] parameters) where TViewModel : class, IViewModelBase
        {
            return GoTo<TViewModel>(out var _, parameters);
        }

        public IContextFor GoTo<TViewModel>(out TViewModel viewModel, params IParameter[] parameters) where TViewModel : class, IViewModelBase
        {
            lock (_lock)
            {
                viewModel = SimpleProvider.GetViewModel<TViewModel>(parameters);
                AddHistory(viewModel);
                CurrentViewModelChanged?.Invoke(viewModel);
                return GetContext(viewModel, parameters);
            }
        }

        public IContextFor GoTo<TViewModel>(Router router) where TViewModel : PagesViewModelBase
        {
            return GoTo<TViewModel>(router, out var _);
        }

        public IContextFor GoTo<TViewModel>(Router router, out TViewModel viewModel) where TViewModel : PagesViewModelBase
        {
            lock (_lock)
            {
                viewModel = SimpleProvider.GetViewModel<TViewModel>(router.ForViewModel());
                AddHistory(viewModel);
                CurrentViewModelChanged?.Invoke(viewModel);
                return GetContext(viewModel);
            }
        }

        private void AddHistory(IViewModelBase item)
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

        private IContextFor GetContext(IViewModelBase viewModel, params IParameter[] parameters)
        {
            var configuration = SimpleProvider.GetService<IIocConfiguration>();
            var contextFor = configuration!.GetContextFor(viewModel.GetType().Name, parameters);
            //Avoid binding error due to propagating context
            if (contextFor != null)
            {
                contextFor.DataContext = null;
                contextFor.DataContext = viewModel;
            }

            return contextFor ?? throw new NotImplementedException($"Unable to find view for {viewModel.GetType().Name}");
        }
    }
}
