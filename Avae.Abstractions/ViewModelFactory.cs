namespace Avae.Abstractions
{
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
                throw new InvalidOperationException("You must register a factory for view models with parameters.");
            }

            return (TBaseType?)SimpleProvider.GetService(viewModelType) ?? throw new InvalidOperationException($"Unable to create {viewModelType.Name}.  Ensure that it is registered with the service provider and it derives from {typeof(IViewModelBase).FullName}.");
        }
    }
}
