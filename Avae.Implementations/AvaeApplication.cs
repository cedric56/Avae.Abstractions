using Avae.Abstractions;
using Avalonia;
using FluentAvalonia.Styling;
using Microsoft.Extensions.DependencyInjection;

namespace Avae.Implementations
{
    public abstract class AvaeApplication : Application, IIocConfiguration
    {
        public abstract string IconUrl { get; }
        public abstract bool IsStandardDialog {  get; }

        public AvaeApplication()
        {
            Container = new IocContainer(this);
        }

        public IocContainer Container { get; protected set; }

        public virtual void Configure(IIocContainer container)
        {

        }

        public virtual void Configure(IServiceCollection services)
        {
            services.AddSingleton<IIocConfiguration>(this);
            services.AddSingleton<IDialogService>(provider =>
            {
                return IsStandardDialog ? new DialogService(IconUrl)
                : new ContentDialogService();
            });
            services.AddSingleton<IContentDialogService, ContentDialogService>();
            services.AddSingleton<ITaskDialogService, TaskDialogService>();
        }

        public void Configure(IServiceProvider provider)
        {
            SimpleProvider.ConfigureServices(provider);
        }

        public object? GetView(string key, object[] @params)
        {
            return Container!.GetView(key, @params);
        }

        public IContextFor? GetContextFor(string key, params object[] @params)
        {
            return Container!.GetView(key, @params) as IContextFor;
        }

        /// <summary>
        /// Obtain the view by the viewModel association
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="params"></param>
        /// <returns></returns>
        public IContextFor<TViewModel>? GetContextFor<TViewModel>(params object[] @params) where TViewModel : IViewModelBase
        {
            return Container!.GetView(typeof(TViewModel).Name, @params) as IContextFor<TViewModel>;
        }

        /// <summary>
        /// Obtain the view by the modalViewModel association
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="params"></param>
        /// <returns></returns>
        public IModalFor<TViewModel>? GetModalFor<TViewModel>(params object[] @params) where TViewModel : IViewModelBase
        {
            return Container!.GetView(typeof(TViewModel).Name, @params) as IModalFor<TViewModel>;
        }

        public override void OnFrameworkInitializationCompleted()
        {
            base.OnFrameworkInitializationCompleted();

            Styles.Add(new FluentAvaloniaTheme());
        }
    }
}
