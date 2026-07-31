using Avae.Abstractions;
using Avae.Implementations.Services;
using Avae.Services;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Fluent;
using FluentAvalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Avae.Implementations
{
    public enum TypeDialog
    {
        Fluent,
        Box
    }

    public abstract class AvaeApplication : Application, IIocConfiguration, IDisposable
    {

        protected virtual string Logs { get; } = "";
        public abstract string IconUrl { get; }
        public abstract TypeDialog TypeDialog {  get; }

        public AvaeApplication()
        {
            Container = new IocContainer(this);
        }

        public IocContainer Container { get; private set; }

        public virtual void Configure(IIocContainer container)
        {

        }

        protected virtual void ConfigureLogging(ILoggingBuilder builder)
        {

        }

        public virtual void Configure(IServiceCollection services)
        {            
            services.AddSingleton<IBrokerService, BrokerService>();
            services.AddSingleton<IIocConfiguration>(this);
            services.AddSingleton<IDialogService>(sp =>
            {
                return TypeDialog== TypeDialog.Box ? new DialogService(sp, IconUrl) :
                             sp.GetRequiredService<IContentDialogService>() as ContentDialogService ??
                             throw new InvalidOperationException("Failed to resolve IContentDialogService.");
            });
            services.AddTransient<INotificationManager, NotificationService>();
            services.AddSingleton<IContentDialogService>(sp => new ContentDialogService(sp));
            services.AddSingleton<ITaskDialogService, TaskDialogService>();
            services.AddSingleton<ILogger>(LoggerFactory.Create(ConfigureLogging).CreateLogger<AvaeApplication>());
        }

        public void Configure(IServiceProvider provider)
        {
            ServiceLocator.SetDefault(provider);
        }

        public object? GetView(string key, params object[] @params)
        {
            return Container.GetView(key, @params);
        }

        public IContextFor? GetContextFor(string key, NavigationContext context)
        {
            return Container.GetView(key, [context]) as IContextFor;
        }

        /// <summary>
        /// Obtain the view by the viewModel association
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="params"></param>
        /// <returns></returns>
        public IContextFor<TViewModel>? GetContextFor<TViewModel>(NavigationContext context) where TViewModel : IViewModelBase
        {
            return Container.GetView(typeof(TViewModel).Name, [context]) as IContextFor<TViewModel>;
        }

        /// <summary>
        /// Obtain the view by the modalViewModel association
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="params"></param>
        /// <returns></returns>
        public IModalFor<TViewModel, TResult>? GetModalFor<TViewModel, TResult>(NavigationContext context) where TViewModel : ICloseableViewModel<TResult>
        {
            var view = Container.GetView(typeof(TViewModel).Name, [context]);
            
            // Now verify the TResult type matches
            Type viewType = view.GetType();
            Type[] interfaces = viewType.GetInterfaces();

            // Find the IModalFor<,> interface implementation
            var modalInterface = interfaces.FirstOrDefault(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IModalFor<,>));

            if (modalInterface != null)
            {
                Type[] genericArgs = modalInterface.GetGenericArguments();
                Type viewModelType = genericArgs[0];
                Type resultType = genericArgs[1];

                // Check if the TResult matches
                if (resultType != typeof(TResult))
                {
                    throw new InvalidOperationException(
                        $"The view associated with view model {typeof(TViewModel).Name} expects result type {resultType.Name}, " +
                        $"but {typeof(TResult).Name} was requested.");
                }
            }

            if (view is not IModalFor<TViewModel, TResult> modal)
                throw new InvalidOperationException($"The view associated with the view model {typeof(TViewModel).Name} is not a modal view.");

            return view as IModalFor<TViewModel, TResult>;
        }

        public override void OnFrameworkInitializationCompleted()
        {
            base.OnFrameworkInitializationCompleted();

            Styles.Add(new StyleInclude(Container.Provider)
            {
                Source = new Uri("avares://Avae.Implementations/Modal/ModalStyle.axaml")
            });
            Styles.Add(new FluentTheme());
            Styles.Add(new FluentAvaloniaTheme());

            TopLevelStateManager.Initialize();

            if(this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime classic)
                classic.Exit += Classic_Exit;
        }

        private void Classic_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime classic)
                classic.Exit -= Classic_Exit;

            Dispose();
        }

        public void Dispose()
        {
            if (Container.Provider is IDisposable disposable)
                disposable.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
