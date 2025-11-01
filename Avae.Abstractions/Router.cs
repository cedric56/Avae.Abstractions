using System.Collections.Generic;

namespace Avae.Abstractions
{
    /// <summary>
    /// Class initially copied from https://github.com/sandreas/Avalonia.SimpleRouter/blob/main/Avalonia.SimpleRouter/HistoryRouter.cs
    /// but heavily modified.
    /// </summary>
    public sealed class Router
    {
        public Router()
        {

        }

        private  readonly object _lock = new object();
        public Guid Id { get; } = Guid.NewGuid();
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
            OnCurrentViewModelChanged(Current!);
            return Current;
        }

        public IViewModelBase? Forward()
        {
            if (!CanGoForward)
            {
                return null;
            }

            _currentIndex++;
            OnCurrentViewModelChanged(Current!);
            return Current;
        }

        /// <summary>
        /// Navigates to the view associated with the specified view model type.
        /// If you directly know the type of the view model at compile time, use <see cref="GoTo{T}()"/> instead.
        /// </summary>
        /// <typeparam name="TBaseType">The base type of the view model.</typeparam>
        /// <param name="viewModelType">The view model type.</param>
        /// <returns>The created view model cast to the <typeparamref name="TBaseType"/>.</returns>
        public TBaseType GoTo<TBaseType>(Type viewModelType, params object[] parameters) where TBaseType : IViewModelBase
        {
            lock (_lock)
            {
                var destination = ViewModelFactory.Create<TBaseType>(viewModelType, parameters);
                AddHistory(destination);                
                OnCurrentViewModelChanged(Current!);
                return destination;
            }
        }

        public TBaseType Create<TBaseType>(Type viewModelType, bool addHystory, params object[] parameters) where TBaseType : IViewModelBase
        {
            lock (_lock)
            {
                var viewModel = ViewModelFactory.Create<TBaseType>(viewModelType, parameters);
                AddHistory(viewModel);
                return viewModel;
            }
        }

        /// <summary>
        /// Navigates to the view associated with the specified view model type.
        /// </summary>
        /// <typeparam name="T">The type of the view model.</typeparam>
        /// <returns>The created view model.</returns>
        public T GoTo<T>(params object[] parameters) where T : IViewModelBase
        {
            lock (_lock)
            {
                var destination = ViewModelFactory.Create<T>(parameters);
                AddHistory(destination);
                OnCurrentViewModelChanged(Current!);
                return destination;
            }
        }

        private void OnCurrentViewModelChanged(IViewModelBase viewModel)
        {
            CurrentViewModelChanged?.Invoke(viewModel);
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
    }

    public static class ViewModelFactory
    {
        /// <summary>
        /// Use <see cref="Router.GoTo{T}()"/> to navigate to a new view based upon a view model type.
        /// This method is only used to create a view model instance of the specified type.
        /// This can be useful for components where you don't need to navigate to a new view.
        /// </summary>
        /// <typeparam name="T">The type to return (the type or base type of the view model).</typeparam>
        /// <returns>The created view model.</returns>
        public static T Create<T>(params object[] parameters) where T : IViewModelBase
        {
            return Create<T>(typeof(T), parameters);
        }

        /// <summary>
        /// Use <see cref="Router.GoTo{TBaseType}(Type)"/> to navigate to a new view based upon a view model type.
        /// If you directly know the type of the view model at compile time, use <see cref="Create{T}()"/> instead.
        /// This method is only used to create a view model instance of the specified type.
        /// This can be useful for components where you don't need to navigate to a new view.
        /// </summary>
        /// <typeparam name="TBaseType">The base type of the view model.</typeparam>
        /// <param name="viewModelType">The type of the view model.</param>
        /// <returns>The created view model cast to the <typeparamref name="TBaseType"/>.</returns>
        public static TBaseType Create<TBaseType>(Type viewModelType, params object[] parameters) where TBaseType : IViewModelBase
        {
            var type = typeof(ViewModelBaseFactory<>).MakeGenericType(viewModelType);
            var factory = SimpleProvider.GetService(type) as IViewModelBaseFactory;

            type = typeof(SingletonFactory<>).MakeGenericType(viewModelType);
            factory ??= SimpleProvider.GetService(type) as IViewModelBaseFactory;

            if (factory != null)
            {
                var viewModel = factory.Create(viewModelType, parameters);
                if (viewModel is TBaseType vm)
                {
                    return vm;
                }
                throw new InvalidOperationException($"Unable to create {viewModelType.Name}.  Ensure that it is registered with the service provider.");
            }

            if (parameters.Length > 0)
            {
                throw new Exception("You must register a factory for view models with parameters.");
            }

            return (TBaseType?)SimpleProvider.GetService(viewModelType) ?? throw new InvalidOperationException($"Unable to create {viewModelType.Name}.  Ensure that it is registered with the service provider and it derives from {typeof(IViewModelBase).FullName}.");
        }
    }
}
