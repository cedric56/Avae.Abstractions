#nullable disable
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Reflection;

namespace Avae.Abstractions
{
    /// <summary>
    /// A simple container for registering and accessing pages in various frameworks such
    /// as Windows, Windows Phone, Android, iOS etc.
    /// </summary>
    public class IocContainer : IIocContainer
    {
        /// <summary>
        /// A dictionary that contains all the registered factories for the views.
        /// </summary>
        readonly Dictionary<string, ViewFactory> _factories = [];
        private readonly Dictionary<string, Type> _pagesByKey = [];

        public IocContainer(IIocConfiguration configuration)
        {
            var services = new ServiceCollection();

            configuration.Configure(services);
            configuration.Configure(this);
            configuration.Configure(services.BuildServiceProvider());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel name as key</typeparam>
        /// <typeparam name="TView">View as Type</typeparam>
        public void Register<TViewModel, TView>()
        {
            Register(typeof(TViewModel).Name, typeof(TView));
        }

        /// <summary>
        /// Register a IViewFor<TViewModel> ViewModel name as key and IViewFor as Type
        /// </summary>
        /// <typeparam name="TViewFor">IViewFor</typeparam>
        public void Register<TViewFor>() where TViewFor : IContextFor
        {
            var viewForType = typeof(TViewFor);
            var viewModelType = viewForType.GetInterface("IContextFor`1")?.GetGenericArguments().FirstOrDefault();
            var name = viewModelType?.Name ?? viewForType.Name;

            Register(name, viewForType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">The registered key</param>
        /// <param name="type"></param>
        public void Register(string key, Type type)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            ArgumentNullException.ThrowIfNull(type);

            if (!_pagesByKey.TryAdd(key, type))
            {
                _pagesByKey[key] = type;
            }
        }

        /// <summary>
        /// Obtain the construtor corresponding on parameters
        /// </summary>
        /// <param name="key">The registered key</param>
        /// <param name="parameters">The parameters as list</param>
        /// <param name="parameter">The passing parameter</param>
        /// <returns>The corresponding constructor</returns>
        private ConstructorInfo GetConstructor(string key, object[] args, out object[] finalArgs)
        {
            if (!_pagesByKey.TryGetValue(key, out var type))
                throw new KeyNotFoundException($"No view registered with key '{key}'.");

            var constructors = type.GetTypeInfo().DeclaredConstructors;

            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();

                if (args.Length > parameters.Length)
                    continue; // Too many arguments

                bool matches = true;
                var resolvedArgs = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (i < args.Length)
                    {
                        var arg = args[i];
                        var paramType = parameters[i].ParameterType;

                        if (arg == null)
                        {
                            if (paramType.IsValueType && Nullable.GetUnderlyingType(paramType) == null)
                            {
                                matches = false;
                                break;
                            }
                        }
                        else if (!paramType.IsAssignableFrom(arg.GetType()))
                        {
                            matches = false;
                            break;
                        }

                        resolvedArgs[i] = arg;
                    }
                    else
                    {
                        // Missing argument: must be optional
                        if (parameters[i].IsOptional)
                        {
                            resolvedArgs[i] = parameters[i].DefaultValue;
                        }
                        else
                        {
                            matches = false;
                            break;
                        }
                    }
                }

                if (matches)
                {
                    finalArgs = resolvedArgs;
                    return constructor;
                }
            }

            throw new InvalidOperationException(
                $"No matching constructor found for view '{key}' with {args.Length} arguments.");
        }

        public Type GetType(string key)
        {
            return _pagesByKey.TryGetValue(key, out var type) ? type : throw new KeyNotFoundException($"No view registered with key '{key}'.");
        }

        /// <summary>
        /// Obtain the view based on his key and parameters
        /// </summary>
        /// <param name="key">The registered key</param>
        /// <param name="parameter">The parameters</param>
        /// <returns>The view</returns>
        public object GetView(string key, object[] @params)
        {
            object view = null;

            var parameters = GetParameters(out int count, @params);
            if (_factories.TryGetValue(key, out var factory) && factory != null)
                view = factory([.. parameters]);
            else
            {
                var constructor = GetConstructor(key, [.. parameters], out object[] finalParameters);
                view = constructor?.Invoke([.. finalParameters]);
            }

            return view ?? throw new Exception("No such page. Did you forget to implement Configure ?");
        }

        /// <summary>
        /// Retreived the parameters from passing parameter
        /// </summary>
        /// <param name="count">The parameters count</param>
        /// <param name="parameter">The passing parameters</param>
        /// <returns>The parameters as list</returns>
        public static List<object> GetParameters(out int count, object parameter = null)
        {
            var parameters = new List<object>();
            if (parameter is IEnumerable enumerable)
            {
                parameters.AddRange(enumerable.Cast<object>().Where(x => x != null).Select(x => x));
            }
            else if (parameter != null)
            {
                parameters.Add(parameter);
            }
            count = parameters.Count;
            return parameters;
        }

        /// <summary>
        /// Registers the ViewModel factory for the specified view type.
        /// </summary>
        /// <typeparam name="T">The View</typeparam>
        /// <param name="factory">The ViewModel factory.</param>
        public void Register<T>(ViewFactory factory = null)
        {
            var typeName = typeof(T).Name;
            if (factory != null)
            {
                RegisterFactory(typeName, factory);
            }
            else
            {
                Register(typeName, typeof(T));
            }
        }

        /// <summary>
        /// Registers the ViewModel factory for the specified view type name.
        /// </summary>
        /// <param name="viewTypeName">The name of the view type.</param>
        /// <param name="factory">The ViewModel factory.</param>
        public void RegisterFactory(string viewTypeName, ViewFactory factory)
        {
            if (string.IsNullOrEmpty(viewTypeName))
            {
                throw new ArgumentException("View type name cannot be null or empty", nameof(viewTypeName));
            }

            _factories[viewTypeName] = factory ?? throw new ArgumentNullException(nameof(factory), "Factory cannot be null");
        }

        public void Register(string key, Func<object[], object> action)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action), "Action cannot be null");
            }

            RegisterFactory(key, new ViewFactory(action));
        }

        public static IEnumerable<IDisposable> GetDisposablesServices()
        {
            var services = SimpleProvider.Services;
            foreach (var service in services)
            {
                if (service is IDisposable disposable)
                {
                    yield return disposable;
                }
            }
        }
    }
}
