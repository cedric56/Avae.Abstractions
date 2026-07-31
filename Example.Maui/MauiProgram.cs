using Avae.Abstractions;
using Avae.DAL;
using Avae.DAL.Interfaces;
using Avae.Services;
using CommunityToolkit.Maui.Extensions;

//using Avalonia.Controls.Maui;
using Example.Maui.Views;
using Example.Models;
using Example.ViewModels;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Platform;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using UXDivers.Popups;
using UXDivers.Popups.Maui;
using UXDivers.Popups.Maui.Controls;
using UXDivers.Popups.Services;
using WindowsToastNotifyApi;

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
#if !WINDOWS
                .UseAvaloniaApp()
#endif
                .ConfigureContainer<App>(container =>
                {
                    container.Register(HomeViewModel.TaskDialogKey, (sp, parameters) =>
                    {
                        return parameters[0] switch
                        {
                            "Footer" => new Label() { Text = "This is a footer" },
                            "IconSource" => null,// new BitmapIconSource() { UriSource = new Uri(IconUrl) },
                            "Content" => new Label() { Text = "Here is content", FontSize = 27 },
                            _ => throw new NotImplementedException()
                        };
                    });
                    container.Register<MainPage>();
                    container.Register<HomeView>();
                    container.Register<MenuView>();
                    container.Register<ModalView>();
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
            builder.Services.AddTransient<ModalViewModel>();
            builder.Services.AddTransient<FormViewModel>();

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

            public async Task<TaskDialogStandardResult> ShowAsync(TaskDialogParams @params, params TaskDialogStandardResult[] results)
            {
                return await DisplayThreeButtons<TaskDialogStandardResult>(
                    @params.Title,
                    @params.Content,
                    results.ElementAtOrDefault(0).ToString(),
                    results.Length > 1 ? results.ElementAtOrDefault(1).ToString() : null,
                    results.Length > 2 ? results.ElementAtOrDefault(2).ToString() : null,
                    results.ElementAtOrDefault(0),
                    results.ElementAtOrDefault(1),
                    results.ElementAtOrDefault(2));
            }

            public async Task<ContentDialogResult> ShowAsync(ContentDialogParams @params)
            {                
                return await DisplayThreeButtons<ContentDialogResult>(
                    @params.Title,
                    @params.Content,
                    @params.PrimaryButtonText,
                    @params.SecondaryButtonText,
                    @params.CloseButtonText,
                    ContentDialogResult.Primary,
                    ContentDialogResult.Secondary,
                    ContentDialogResult.None);
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
                return DisplayThreeButtons(title, message, "Yes", "No", "Cancel", 0, 1, 2);
            }

            public Task<int> ShowYesNoAbortAsync(string message, string title = "Title")
            {
                return DisplayThreeButtons(title, message, "Yes", "No", "Abort", 0, 1, 2);
            }

            async Task<TResult?> IDialogService.ShowModalAsync<TViewModel, TResult>(NavigationContext? context) 
                where TResult : default
            {
                var viewModel = serviceProvider.GetViewModel<TViewModel>(context);
                var view = GetModalFor<TViewModel, TResult>(context ?? new NavigationContext()) ?? throw new InvalidOperationException($"Unable to create view for {typeof(TViewModel).Name}.  Ensure that it is registered in the container.");
                view.Context = viewModel;
                if (view is IDialogView<TViewModel, TResult> dialogView)
                {
                    //var pop = new DialogViewBase<TViewModel, TResult>(dialogView);
                    var modal = new MultipleButtonsPopup<TResult>()
                    {
                        Buttons = viewModel.Commands,
                        Title = dialogView.Title,
                        Content = dialogView as View,
                        Style = Application.Current?.Resources["DefaultMultipleButtonsPopupPopupStyle"] as Style
                    };
                    viewModel.CloseRequested += CloseRequestedHandler;
                    return await IPopupService.Current.PushAsync(modal);

                    async void CloseRequestedHandler(object? sender, TResult? e)
                    {
                        viewModel.CloseRequested -= CloseRequestedHandler;
                        modal.SetResult(e);
                        await IPopupService.Current.PopAsync(modal);
                    }
                }

                throw new InvalidOperationException("Must implement IDialogView");
            }



            async Task<T?> DisplayThreeButtons<T>(
                 string? title, object? content,
                 string? primaryButtonText, string? secondaryButtonText, string? closeButtonText,
                 T primaryResult, T secondaryResult, T closeResult)
            {
                var taskCompletionSource = new TaskCompletionSource<T?>();

                if (content is Element e)
                {
                    content = e.ToPlatform(Current.Handler.MauiContext);
                }
#if ANDROID
                var alertBuilder = new Android.App.AlertDialog.Builder(Platform.CurrentActivity);

                alertBuilder.SetTitle(title);
                if (content is string message)
                    alertBuilder.SetMessage(message);
                else
                    alertBuilder.SetView(content as Android.Views.View);

                if (!string.IsNullOrEmpty(primaryButtonText))
                    alertBuilder.SetPositiveButton(primaryButtonText, (senderAlert, args) =>
                    {
                        taskCompletionSource.SetResult(primaryResult);
                    });
                if (!string.IsNullOrEmpty(secondaryButtonText))
                    alertBuilder.SetNegativeButton(secondaryButtonText, (senderAlert, args) =>
                    {
                        taskCompletionSource.SetResult(secondaryResult);
                    });
                if (!string.IsNullOrEmpty(closeButtonText))
                    alertBuilder.SetNeutralButton(closeButtonText, (senderAlery, args) =>
                    {
                        taskCompletionSource.SetResult(closeResult);
                    });

                var alertDialog = alertBuilder.Create();
                alertDialog?.Show();

                return await taskCompletionSource.Task;
#elif IOS
                var alertController = UIKit.UIAlertController.Create(title, content as string, UIKit.UIAlertControllerStyle.Alert);
                if (!string.IsNullOrEmpty(primaryButtonText))
                    alertController.AddAction(UIKit.UIAlertAction.Create(primaryButtonText, UIKit.UIAlertActionStyle.Default, _ =>
                    {
                        taskCompletionSource.SetResult(primaryResult);
                    }));
                if (!string.IsNullOrEmpty(secondaryButtonText))
                    alertController.AddAction(UIKit.UIAlertAction.Create(secondaryButtonText, UIKit.UIAlertActionStyle.Default, _ =>
                    {
                        taskCompletionSource.SetResult(secondaryResult);
                    }));
                if (!string.IsNullOrEmpty(closeButtonText))
                    alertController.AddAction(UIKit.UIAlertAction.Create(closeButtonText, UIKit.UIAlertActionStyle.Default, _ =>
                    {
                        taskCompletionSource.SetResult(closeResult);
                    }));
                var rootViewController = UIKit.UIApplication.SharedApplication.KeyWindow?.RootViewController;
                rootViewController?.PresentViewController(alertController, true, null);
                return await taskCompletionSource.Task;
#elif WINDOWS

                var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
                {
                    Title = title,
                    Content = content,
                    PrimaryButtonText = primaryButtonText,
                    SecondaryButtonText = secondaryButtonText,
                    CloseButtonText = closeButtonText,
                    XamlRoot = Current.Handler?.PlatformView is Microsoft.UI.Xaml.UIElement page ? page.XamlRoot : null
                };
                dialog.PrimaryButtonClick += (s, e) => taskCompletionSource.SetResult(primaryResult);
                dialog.SecondaryButtonClick += (s, e) => taskCompletionSource.SetResult(secondaryResult);
                dialog.CloseButtonClick += (s, e) => taskCompletionSource.SetResult(closeResult);

                await dialog.ShowAsync();
                return await taskCompletionSource.Task;
#elif MACCATALYST
                var alert = new UIKit.UIAlertController
                {
                    Title = title,
                    Message = content as string,
                    //PreferredStyle = UIKit.UIAlertControllerStyle.Alert
                };
                if (!string.IsNullOrEmpty(primaryButtonText))
                    alert.AddAction(UIKit.UIAlertAction.Create(primaryButtonText, UIKit.UIAlertActionStyle.Default, _ =>
                    {
                        taskCompletionSource.SetResult(primaryResult);
                    }));
                if (!string.IsNullOrEmpty(secondaryButtonText))
                    alert.AddAction(UIKit.UIAlertAction.Create(secondaryButtonText, UIKit.UIAlertActionStyle.Default, _ =>
                    {
                        taskCompletionSource.SetResult(secondaryResult);
                    }));
                if (!string.IsNullOrEmpty(closeButtonText))
                    alert.AddAction(UIKit.UIAlertAction.Create(closeButtonText, UIKit.UIAlertActionStyle.Default, _ =>
                    {
                        taskCompletionSource.SetResult(closeResult);
                    }));
                var rootViewController = UIKit.UIApplication.SharedApplication.KeyWindow?.RootViewController;
                rootViewController?.PresentViewController(alert, true, null);
                return await taskCompletionSource.Task;
#endif
                throw new NotImplementedException();
            }

            public ISystemNotification CreateNotification(string action, string title, string message, SystemNotificationAction[] actions)
            {
                return new Notification(action, title, message, actions);
            }

            class Notification(string action, string title, string message, SystemNotificationAction[] actions) : ISystemNotification
            {
                public event EventHandler<SystemNotificationEventArgs>? NotificationCompleted;

                public void Close()
                {
                    
                }

                public void Show()
                {
                    
                }
            }
        }

        public static MauiAppBuilder ConfigureContainer<TApp>(this MauiAppBuilder builder,
            Action<IIocContainer>? configure = null, Action<ILoggingBuilder>? build =null )
            where TApp : Application
        {
            builder.UseUXDiversPopups();

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

    internal class MultipleButtonsPopupBase : PopupPage
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(MultipleButtonsPopupBase),
        null);

        /// <summary>
        /// Gets or sets the title text displayed in the popup.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly BindableProperty ButtonsProperty = BindableProperty.Create(
        nameof(Buttons),
        typeof(ObservableCollection<NamedCommand>),
        typeof(MultipleButtonsPopupBase),
        null);

        public ObservableCollection<NamedCommand> Buttons
        {
            get { return (ObservableCollection<NamedCommand>)GetValue(ButtonsProperty); }
            set { SetValue(ButtonsProperty, value); }
        }
    }

    internal partial class MultipleButtonsPopup<TResult> : MultipleButtonsPopupBase, IPopupResultPage<TResult?>
    {
        public TResult? Result { get; set; }

        public void SetResult(TResult? result)
        {
            Result = result;
        }
    }
}
