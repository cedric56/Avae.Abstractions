using Avae.Abstractions;
using Avae.DAL;
using CommunityToolkit.Maui;
using Example.Maui.Views;
using Example.Models;
using Example.ViewModels;
using MagicOnion;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;

namespace Example.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiCommunityToolkit()
                .ConfigureViews<App>(container =>
                {
                    container.Register<MainPage>();
                    container.Register<HomeView>();
                    container.Register<MenuView>();
                    container.Register<ModalViewModel>((sp, context) => new ModalView(sp.GetRequiredService<ICurrentPage>()));
                })
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<MenuViewModel>();
            builder.Services.AddSingleton<ModalViewModel>();
            
            builder.Services.UseDbLayer<IDBLayer>(sp => new DBSqlLayer(sp));
            builder.Services.AddTransient<IDbConnection>(sp =>
            {
                var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var dbPath = Path.Combine(folder, "database.db");
                var connectionString = $"Data Source={dbPath};Foreign Keys=True";
                return new SqliteConnection(connectionString);
            });
#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            ServiceLocator.SetDefault(app.Services);
            return app;
        }
    }

    public interface ICurrentPage
    {
        Page Current { get; }
    }

    public static class AvaeExtensions
    {
        class IocConfiguration(IServiceProvider serviceProvider, Func<IocContainer> getContainer, Action<IIocContainer>? configure = null) : 
            IIocConfiguration, ITaskDialogService, IContentDialogService, IDialogService,
            ICurrentPage
        {
            IocContainer? _container = null;
            IocContainer Container { get => _container ??= getContainer(); }

            public Page Current => Application.Current?.Windows.FirstOrDefault(w => w.IsActivated)?.Page ?? Shell.Current;

            public void Configure(IIocContainer container)
            {
                configure?.Invoke(container);
            }

            public void Configure(IServiceCollection services)
            {

            }

            public void Configure(IServiceProvider provider)
            {
                
            }

            public object? GetView(string key, params object[] @params)
            {
                return Container.GetView(key, @params);
            }

            public IContextFor? GetContextFor(string key, NavigationContext context)
            {
                return Container.GetView(key, [context]) as IContextFor;
            }

            public IContextFor<TViewModel>? GetContextFor<TViewModel>(NavigationContext context) where TViewModel : IViewModelBase
            {
                return Container.GetView(typeof(TViewModel).Name, [context]) as IContextFor<TViewModel>;
            }

            public IModalFor<TViewModel, TResult>? GetModalFor<TViewModel, TResult>(NavigationContext context) where TViewModel : IViewModelBase
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

            public Task<TaskDialogStandardResult> ShowAsync(TaskDialogParams @params, params TaskDialogStandardResult[] results)
            {
                throw new NotImplementedException();
            }

            public Task<ContentDialogResult> ShowAsync(ContentDialogParams @params)
            {
                throw new NotImplementedException();
            }

            public Task ShowErrorAsync(Exception ex, string title = "Error")
            {
                throw new NotImplementedException();
            }

            public Task ShowOkAsync(string message, string title = "Title")
            {
                return Current.DisplayAlertAsync(title, message, "Ok");
            }

            public Task<bool> ShowYesNoAsync(string message, string title = "Title")
            {
                throw new NotImplementedException();
            }

            public Task<bool> ShowOkCancelAsync(string message, string title = "Title")
            {
                throw new NotImplementedException();
            }

            public Task<bool> ShowOkAbortAsync(string message, string title = "Title")
            {
                throw new NotImplementedException();
            }

            public Task<int> ShowYesNoCancelAsync(string message, string title = "Title")
            {
                throw new NotImplementedException();
            }

            public Task<int> ShowYesNoAbortAsync(string message, string title = "Title")
            {
                throw new NotImplementedException();
            }

            Task<TResult?> IDialogService.ShowModalAsync<TViewModel, TResult>(NavigationContext? context) where TResult : default
            {
                var viewModel = serviceProvider.GetViewModel<TViewModel>(context);
                var view = GetModalFor<TViewModel, TResult>(context ?? new NavigationContext()) ?? throw new InvalidOperationException($"Unable to create view for {typeof(TViewModel).Name}.  Ensure that it is registered in the container.");
                view.Context = viewModel;
                return view.ShowModalAsync();                
            }
        }

        public static MauiAppBuilder ConfigureViews<TApp>(this MauiAppBuilder builder,
            Action<IIocContainer>? configure = null)
            where TApp : Application

        {            
            builder.Services.AddSingleton<IIocContainer>(sp => new IocContainer(GetConfiguration(sp), false));
            builder.Services.AddSingleton<IIocConfiguration>(sp => new IocConfiguration(sp, () => (IocContainer)sp.GetRequiredService<IIocContainer>(), configure));
            builder.Services.AddSingleton<Router>(sp => new Router(sp));
            builder.Services.AddSingleton<IDialogService>(GetConfiguration);
            builder.Services.AddSingleton<IContentDialogService>(GetConfiguration);
            builder.Services.AddSingleton<ITaskDialogService>(GetConfiguration);
            builder.Services.AddTransient<ICurrentPage>(GetConfiguration);
            builder.Services.AddSingleton<ILogger>(LoggerFactory.Create(builder =>
            {

            }).CreateLogger<TApp>());
            return builder;

            IocConfiguration GetConfiguration(IServiceProvider provider)
            {
                return (IocConfiguration)provider.GetRequiredService<IIocConfiguration>();
            }
        }
    }       
}
