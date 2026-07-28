using Avae.Abstractions;
using Avae.Abstractions.Interfaces;
using Avae.DAL;
using Avae.DAL.Interfaces;
using Avalonia.Controls.Maui;
using Example.Maui.Views;
using Example.Models;
using Example.ViewModels;
using MagicOnion;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Example.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
#if !NET11_0
                .UseMauiCommunityToolkit()
#endif
                .UseAvaloniaApp()
                .ConfigureContainer<App>(container =>
                {
                    container.Register<MainPage>();
                    container.Register<HomeView>();
                    container.Register<MenuView>();
                    container.Register<ModalView>((sp, context) => new ModalView(sp.GetRequiredService<ICurrentPage>()));
                    //container.Register<FormView>();
                    container.Register<FormViewModel>((sp, context) =>
                    {
                        if (context.FactoryParameters.OfType<string>().Any(p => p == FormViewModel.KEY))
                        {
                            return new Label() { Text = "Hello "};
                        }
                        return new FormView();
                    });
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
            builder.Services.AddSingleton<FormViewModel>();

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbPath = Path.Combine(folder, "database.db");
            var connectionString = $"Data Source={dbPath};Foreign Keys=True";

            builder.Services.UseDbLayer<IDBLayer>(sp => new DBSqlLayer(sp));
            builder.Services.UseSqlMonitors<SqliteConnection>(connectionString, (factory) =>
            {
                var monitor = factory.AddDbMonitor<Person>();
                monitor.AddSignalR("http://localhost:5001/PersonHub");
                builder.Services.AddSingleton<ISqlMonitor<Person>>(sp =>
                {
                    return monitor;
                });
            });
            builder.Services.AddTransient<IDbConnection>(sp =>
            {
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
            ICurrentPage, ISystemNotificationService
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
                var view = Container.GetView(key, [context]);
                if (view is not null && view is not IContextFor)
                    throw new InvalidOperationException("View must implement IContextFor");
                return view as IContextFor;
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
                return Current.DisplayAlertAsync(title, ex.Message, "Ok");
            }

            public Task ShowOkAsync(string message, string title = "Title")
            {
                return Current.DisplayAlertAsync(title, message, "Ok");
            }

            public Task<bool> ShowYesNoAsync(string message, string title = "Title")
            {
                return Current.DisplayAlertAsync(title, message, "Yes", "No");
            }

            public Task<bool> ShowOkCancelAsync(string message, string title = "Title")
            {
                return Current.DisplayAlertAsync(title, message, "Ok", "Cancel");
            }

            public Task<bool> ShowOkAbortAsync(string message, string title = "Title")
            {
                return Current.DisplayAlertAsync(title, message, "Ok", "Abort");
            }

            public Task<int> ShowYesNoCancelAsync(string message, string title = "Title")
            {
                return DisplayThreeButtons(message, title, "Yes", "No", "Cancel");
            }

            public Task<int> ShowYesNoAbortAsync(string message, string title = "Title")
            {
                return DisplayThreeButtons(message, title, "Yes", "No", "Abort");
            }

            Task<TResult?> IDialogService.ShowModalAsync<TViewModel, TResult>(NavigationContext? context) where TResult : default
            {
                var viewModel = serviceProvider.GetViewModel<TViewModel>(context);
                var view = GetModalFor<TViewModel, TResult>(context ?? new NavigationContext()) ?? throw new InvalidOperationException($"Unable to create view for {typeof(TViewModel).Name}.  Ensure that it is registered in the container.");
                view.Context = viewModel;
                return view.ShowModalAsync();
            }

            async Task<int> DisplayThreeButtons(string message, string title, string button1, string button2, string button3)
            {
                var taskCompletionSource = new TaskCompletionSource<int>();
#if ANDROID                
                var alertBuilder = new Android.App.AlertDialog.Builder(Platform.CurrentActivity);

                alertBuilder.SetTitle(title);
                alertBuilder.SetMessage(message);

                alertBuilder.SetPositiveButton(button1, (senderAlert, args) =>
                {
                    taskCompletionSource.SetResult(0);
                });

                alertBuilder.SetNegativeButton(button2, (senderAlert, args) =>
                {
                    taskCompletionSource.SetResult(1);
                });

                alertBuilder.SetNeutralButton(button3, (senderAlery, args) =>
                {
                    taskCompletionSource.SetResult(2);
                });

                var alertDialog = alertBuilder.Create();
                alertDialog?.Show();

                return await taskCompletionSource.Task;
#elif IOS
                var alertController = UIKit.UIAlertController.Create(title, message, UIKit.UIAlertControllerStyle.Alert);
                alertController.AddAction(UIKit.UIAlertAction.Create(button1, UIKit.UIAlertActionStyle.Default, _ =>
                {
                    taskCompletionSource.SetResult(0);
                }));
                alertController.AddAction(UIKit.UIAlertAction.Create(button2, UIKit.UIAlertActionStyle.Default, _ =>
                {
                    taskCompletionSource.SetResult(1);
                }));
                alertController.AddAction(UIKit.UIAlertAction.Create(button3, UIKit.UIAlertActionStyle.Default, _ =>
                {
                    taskCompletionSource.SetResult(2);
                }));
                var rootViewController = UIKit.UIApplication.SharedApplication.KeyWindow?.RootViewController;
                rootViewController?.PresentViewController(alertController, true, null);
                return await taskCompletionSource.Task;
#elif WINDOWS
                var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
                {
                    Title = title,
                    Content = message,
                    PrimaryButtonText = button1,
                    SecondaryButtonText = button2,
                    CloseButtonText = button3,
                    XamlRoot = Current.ToPlatform() is Microsoft.UI.Xaml.UIElement page ? page.XamlRoot : null
                };
                dialog.PrimaryButtonClick += (s, e) => taskCompletionSource.SetResult(0);
                dialog.SecondaryButtonClick += (s, e) => taskCompletionSource.SetResult(1);
                dialog.CloseButtonClick += (s, e) => taskCompletionSource.SetResult(2);
                await dialog.ShowAsync();
                return await taskCompletionSource.Task;
#elif MACCATALYST
                var alert = new UIKit.UIAlertController
                {
                    Title = title,
                    Message = message,
                    //PreferredStyle = UIKit.UIAlertControllerStyle.Alert
                };
                alert.AddAction(UIKit.UIAlertAction.Create(button1, UIKit.UIAlertActionStyle.Default, _ =>
                {
                    taskCompletionSource.SetResult(0);
                }));
                alert.AddAction(UIKit.UIAlertAction.Create(button2, UIKit.UIAlertActionStyle.Default, _ =>
                {
                    taskCompletionSource.SetResult(1);
                }));
                alert.AddAction(UIKit.UIAlertAction.Create(button3, UIKit.UIAlertActionStyle.Default, _ =>
                {
                    taskCompletionSource.SetResult(2);
                }));
                var rootViewController = UIKit.UIApplication.SharedApplication.KeyWindow?.RootViewController;
                rootViewController?.PresentViewController(alert, true, null);
                return await taskCompletionSource.Task;
#endif
                throw new NotImplementedException();
            }

            public ISystemNotification CreateNotification(string action, string title, string message, SystemNotificationAction[] actions)
            {
                throw new NotImplementedException();
            }
        }

        public static MauiAppBuilder ConfigureContainer<TApp>(this MauiAppBuilder builder,
            Action<IIocContainer>? configure = null, Action<ILoggingBuilder>? build =null )
            where TApp : Application

        {            
            builder.Services.AddSingleton<IIocContainer>(sp => new IocContainer(GetConfiguration(sp), false));
            builder.Services.AddSingleton<IIocConfiguration>(sp => new IocConfiguration(sp, () => (IocContainer)sp.GetRequiredService<IIocContainer>(), configure));
            builder.Services.AddSingleton<Router>(sp => new Router(sp));
            builder.Services.AddSingleton<IDialogService>(GetConfiguration);
            builder.Services.AddSingleton<IContentDialogService>(GetConfiguration);
            builder.Services.AddSingleton<ITaskDialogService>(GetConfiguration);
            builder.Services.AddTransient<ICurrentPage>(GetConfiguration);
            builder.Services.AddSingleton<ISystemNotificationService>(GetConfiguration);
            builder.Services.AddSingleton<ILogger>(LoggerFactory.Create(builder =>
            {
                build?.Invoke(builder);

            }).CreateLogger<TApp>());
            return builder;

            IocConfiguration GetConfiguration(IServiceProvider provider)
            {
                return (IocConfiguration)provider.GetRequiredService<IIocConfiguration>();
            }
        }
    }       
}
