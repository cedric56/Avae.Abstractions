using Avae.Abstractions;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Fluent;
using FluentAvalonia.Styling;
using Microsoft.Extensions.DependencyInjection;

namespace Avae.Implementations
{
    public abstract class AvaeApplication : Application, IIocConfiguration
    {
        public abstract string IconUrl { get; }
        public abstract bool IsStandard {  get; }

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
                return IsStandard ? new DialogService(IconUrl)
                : new ContentDialogService();
            });
            services.AddSingleton<IContentDialogService, ContentDialogService>();
            services.AddSingleton<ITaskDialogService, TaskDialogService>();
        }

        public void Configure(IServiceProvider provider)
        {
            SimpleProvider.ConfigureServices(provider);
        }

        public object? GetView(string key, params IParameter[] @params)
        {
            return Container!.GetView(key, @params);
        }

        public IContextFor? GetContextFor(string key, params IParameter[] @params)
        {
            return Container!.GetView(key, @params) as IContextFor;
        }

        /// <summary>
        /// Obtain the view by the viewModel association
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="params"></param>
        /// <returns></returns>
        public IContextFor<TViewModel>? GetContextFor<TViewModel>(params IParameter[] @params) where TViewModel : IViewModelBase
        {
            return Container!.GetView(typeof(TViewModel).Name, @params) as IContextFor<TViewModel>;
        }

        /// <summary>
        /// Obtain the view by the modalViewModel association
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="params"></param>
        /// <returns></returns>
        public IModalFor<TViewModel, TResult>? GetModalFor<TViewModel, TResult>(params IParameter[] @params) where TViewModel : IViewModelBase
        {
            return Container!.GetView(typeof(TViewModel).Name, @params) as IModalFor<TViewModel, TResult>;
        }

        public override void OnFrameworkInitializationCompleted()
        {
            base.OnFrameworkInitializationCompleted();

            Styles.Add(new StyleInclude(SimpleProvider.Default)
            {
                Source = new Uri("avares://Avae.Implementations/Modal/ModalStyle.axaml")
            });
            Styles.Add(new FluentTheme());
            Styles.Add(new FluentAvaloniaTheme());
        }
    }
}
