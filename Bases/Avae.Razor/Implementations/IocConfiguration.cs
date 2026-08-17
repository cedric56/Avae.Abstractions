using Append.Blazor.Notifications;
using Avae.Abstractions;
using Avae.Razor.Components;
using Avae.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using NavigationContext = Avae.Abstractions.NavigationContext;

namespace Avae.Razor
{
    public class IocConfiguration(IServiceProvider serviceProvider, Action<IIocContainer>? configure = null) :
            IIocConfiguration, ITaskDialogService, IContentDialogService, 
            IDialogService,
            ISystemNotificationService,
            Services.INotificationService,
            IRequestedThemeService
    {
        public EventHandler<RequestedTheme>? RequestedThemeChanged;

        public bool IsDarkMode { get; set; } = true;

        public static MudBlazor.IDialogService MudDialogService { get; set; } = default!;
        public static MudBlazor.ISnackbar SnackbarService { get; set; } = default!;

        IocContainer? _container = null;
        IocContainer Container { get => _container ??= (IocContainer)serviceProvider.GetRequiredService<IIocContainer>(); }

        public void Configure(IIocContainer container)
        {
            configure?.Invoke(container);
        }

        public void Configure(IServiceCollection services)
        {
            services.AddMudServices();
        }

        public void Configure(IServiceProvider provider)
        {
            
        }

        public async Task<ISystemNotification?> CreateNotification(string action, string title, string message, SystemNotificationAction[] actions)
        {
            var service = Container.Provider.GetRequiredService<Append.Blazor.Notifications.INotificationService>();
            if (await service.IsSupportedByBrowserAsync())
            {
                if (service.PermissionStatus != PermissionType.Granted)
                    await service.RequestPermissionAsync();
                return new N(service, title);
            }
            return null;
        }

        class N(Append.Blazor.Notifications.INotificationService service, string title) : ISystemNotification
        {
            public event EventHandler<SystemNotificationEventArgs>? NotificationCompleted;

            public void Close()
            {
                throw new NotImplementedException();
            }

            public async void Show()
            {
                NotificationCompleted?.Invoke(this, new SystemNotificationEventArgs());
                await service.CreateAsync(title, new NotificationOptions()
                {
                    //Body = message,
                    //Data = action,
                });
            }
        }

        public IViewFor? GetContextFor(string key, NavigationContext context)
        {
            return Container.GetView(key, context is null ? [] : [context]) as IViewFor;
        }

        public IViewFor<TViewModel>? GetContextFor<TViewModel>(NavigationContext context) where TViewModel : IViewModelBase
        {
            return Container.GetView(typeof(TViewModel).Name, context is null ? [] : [context]) as IViewFor<TViewModel>;
        }

        public IModalFor<TViewModel, TResult>? GetModalFor<TViewModel, TResult>(NavigationContext context) where TViewModel : ICloseableViewModel<TResult>
        {
            return Container.GetView(typeof(TViewModel).Name, context is null ? [] : [context]) as IModalFor<TViewModel, TResult>;
        }

        public object? GetView(string key, params object[] @params)
        {
            return Container.GetView(key, @params);
        }

        public void Request(RequestedTheme theme)
        {
            IsDarkMode = theme == RequestedTheme.Dark;
            RequestedThemeChanged?.Invoke(this, theme);
        }

        public void Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Action? onClick = null, Action? onClose = null)
        {
            var snack = SnackbarService.Add(new MarkupString($"<h5>{title}</h5>{message}"), type switch
            {
                NotificationType.Information => MudBlazor.Severity.Info,
                NotificationType.Success => MudBlazor.Severity.Success,
                NotificationType.Warning => MudBlazor.Severity.Warning,
                NotificationType.Error => MudBlazor.Severity.Error,
                _ => MudBlazor.Severity.Normal
            }, config =>
            {
                config.RequireInteraction = expiration is null;
                if (expiration.HasValue)
                    config.VisibleStateDuration = (int)expiration.Value.TotalMilliseconds;
                config.OnClick = snackbar =>
                {
                    onClick?.Invoke();
                    return Task.CompletedTask;
                };
            });
            snack?.OnClose += (snackbar) =>
            {
                onClose?.Invoke();
            };
        }

        public async Task<ContentDialogResult> ShowAsync(ContentDialogParams @params)
        {
            var dialog = await MudDialogService.ShowAsync<ContentDialog>(@params.Title, 
            new MudBlazor.DialogParameters()
            {
                { "Parameters", @params }

            }, new MudBlazor.DialogOptions()
            {
                BackdropClick = true
            });
            var result = await dialog.Result;
            return result?.Data is ContentDialogResult cdr ? cdr : ContentDialogResult.None;
        }

        public async Task<TaskDialogStandardResult> ShowAsync(TaskDialogParams @params, params TaskDialogStandardResult[] results)
        {
            var dialog = await MudDialogService.ShowAsync<TaskDialog>(@params.Title,
            new MudBlazor.DialogOptions()
            {
                BackdropClick = true
            });
            var result = await dialog.Result;
            return result?.Data is TaskDialogStandardResult cdr ? cdr : TaskDialogStandardResult.None;
        }

        public async Task ShowErrorAsync(Exception ex, string title = "Error")
        {
            await MudDialogService.ShowMessageBoxAsync(
                title,
                new MarkupString(ex.Message.Replace(Environment.NewLine, "<br/>")));
        }

        public async Task<bool> ShowOkAbortAsync(string message, string title = "Title")
        {
            return await MudDialogService.ShowMessageBoxAsync(
                title,
                new MarkupString(message.Replace(Environment.NewLine, "<br/>")),
                cancelText: "Abort") ?? false;
        }

        public async Task ShowOkAsync(string message, string title = "Title")
        {
            await MudDialogService.ShowMessageBoxAsync(
                title,
                new MarkupString(message.Replace(Environment.NewLine, "<br/>")));
        }

        public async Task<bool> ShowOkCancelAsync(string message, string title = "Title")
        {
            return await MudDialogService.ShowMessageBoxAsync(
                title,
                new MarkupString(message.Replace(Environment.NewLine, "<br/>")),
                cancelText: "Cancel") ?? false;
        }

        public async Task<int> ShowYesNoAbortAsync(string message, string title = "Title")
        {
            var result = await MudDialogService.ShowMessageBoxAsync(
                title,
                new MarkupString(message.Replace(Environment.NewLine, "<br/>")),
                yesText: "Yes",
                noText: "No",
                cancelText: "Abort");
            return result switch
            {
                true => 0,
                false => 1,
                _ => 2
            };
        }

        public async Task<bool> ShowYesNoAsync(string message, string title = "Title")
        {
            return await MudDialogService.ShowMessageBoxAsync(
                title,
                new MarkupString(message.Replace(Environment.NewLine, "<br/>")),
                yesText: "Yes",
                cancelText: "No") ?? false;
        }

        public async Task<int> ShowYesNoCancelAsync(string message, string title = "Title")
        {
            var result = await MudDialogService.ShowMessageBoxAsync(
                title,
                new MarkupString(message.Replace(Environment.NewLine, "<br/>")),
                yesText: "Yes",
                noText: "No",
                cancelText: "Cancel");
            return result switch
            {
                true => 0,
                false => 1,
                _ => 2
            };
        }

        async Task<TResult?> Avae.Services.IDialogService.ShowModalAsync<TViewModel, TResult>(NavigationContext? context)
            where TResult : default
        {
            var tcs = new TaskCompletionSource<TResult?>();
            var viewModel = serviceProvider.GetViewModel<TViewModel>(context);
            var contextFor = GetContextFor(typeof(TViewModel).Name, context ?? new NavigationContext());
            if (contextFor is ComponentView view)
            {
                var dialog = await MudDialogService.ShowAsync(view.Type, viewModel.Title, new MudBlazor.DialogParameters()
                {
                    { "ViewModel", viewModel }
                });
                viewModel.CloseRequested += CloseRequestedHandler;
                void CloseRequestedHandler(object? sender, TResult? e)
                {
                    viewModel.CloseRequested -= CloseRequestedHandler;
                    tcs.SetResult(e);
                    MudDialogService.Close(dialog);
                }
                return await tcs.Task;
            }

            throw new InvalidOperationException("View must be ComponentView");
        }
    }
}
