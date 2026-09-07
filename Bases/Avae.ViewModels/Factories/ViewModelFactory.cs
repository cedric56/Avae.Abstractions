using Microsoft.Extensions.DependencyInjection;

namespace Avae.ViewModels;

/// <summary>
/// Base class for a factory that creates <see cref="IViewModelBase"/> instances for a given view model type.
/// </summary>
public abstract class ViewModelFactory : IViewModelBaseFactory
{
    /// <summary>
    /// Creates (or returns a cached instance of) the view model for the specified type.
    /// </summary>
    /// <param name="viewModelType">The type of view model to create.</param>
    /// <param name="parameters">Additional constructor parameters to supply, beyond those resolved from the service provider.</param>
    /// <returns>The created or cached view model instance, or <see langword="null"/> if creation fails.</returns>
    public abstract IViewModelBase? Create(Type viewModelType, params object[] parameters);
}

/// <summary>
/// Factory that creates and caches a single instance of the view model type <typeparamref name="T"/>,
/// resolving its dependencies from the provided <see cref="IServiceProvider"/>.
/// </summary>
/// <typeparam name="T">The view model type this factory creates.</typeparam>
/// <param name="provider">The service provider used to resolve constructor dependencies for <typeparamref name="T"/>.</param>
public class ViewModelFactory<T>(IServiceProvider provider) : ViewModelFactory, IViewModelBaseFactory<T> where T : IViewModelBase
{
    /// <summary>
    /// The cached view model instance, created on first use and reused on subsequent calls.
    /// </summary>
    private T? viewModel = default;

    /// <summary>
    /// Creates the view model on first call via <see cref="ActivatorUtilities.CreateInstance(IServiceProvider, Type, object[])"/>,
    /// caching it for subsequent calls regardless of the <paramref name="viewModelType"/> or <paramref name="parameters"/> passed in.
    /// </summary>
    /// <param name="viewModelType">The concrete type to instantiate on first call.</param>
    /// <param name="parameters">Additional constructor parameters to supply, beyond those resolved from the service provider.</param>
    /// <returns>The cached <typeparamref name="T"/> instance, as an <see cref="IViewModelBase"/>.</returns>
    public override IViewModelBase? Create(Type viewModelType, params object[] parameters)
        => (IViewModelBase?)(viewModel ??= (T)ActivatorUtilities.CreateInstance(provider, viewModelType, [.. parameters]));
}