using Avae.Abstractions;
using Avae.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using NavigationContext = Avae.Abstractions.NavigationContext;

namespace Avae.Razor
{
    public class IocConfiguration(IServiceProvider serviceProvider, Action<IIocContainer>? configure = null) :
            IIocConfiguration, ITaskDialogService, IContentDialogService, 
            Avae.Services.IDialogService,
            ISystemNotificationService,
            INotificationManager,
            IRequestedTheme
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

        public ISystemNotification CreateNotification(string action, string title, string message, SystemNotificationAction[] actions)
        {
            throw new NotImplementedException();
        }

        public IContextFor? GetContextFor(string key, NavigationContext context)
        {
            throw new NotImplementedException();
        }

        public IContextFor<TViewModel>? GetContextFor<TViewModel>(NavigationContext context) where TViewModel : IViewModelBase
        {
            throw new NotImplementedException();
        }

        public IModalFor<TViewModel, TResult>? GetModalFor<TViewModel, TResult>(NavigationContext context) where TViewModel : ICloseableViewModel<TResult>
        {
            throw new NotImplementedException();
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

        public Task<ContentDialogResult> ShowAsync(ContentDialogParams @params)
        {
            throw new NotImplementedException();
        }

        public Task<TaskDialogStandardResult> ShowAsync(TaskDialogParams @params, params TaskDialogStandardResult[] results)
        {
            throw new NotImplementedException();
        }

        public async Task ShowErrorAsync(Exception ex, string title = "Error")
        {
            await MudDialogService.ShowMessageBoxAsync(new MudBlazor.MessageBoxOptions()
            {
                Title = title,
                Message = ex.Message
            });
        }

        public async Task<bool> ShowOkAbortAsync(string message, string title = "Title")
        {
            return await MudDialogService.ShowMessageBoxAsync(new MudBlazor.MessageBoxOptions()
            {
                Title = title,
                Message = message,
                CancelText = "Abort",

            }) ?? false;
        }

        public async Task ShowOkAsync(string message, string title = "Title")
        {
            await MudDialogService.ShowMessageBoxAsync(new MudBlazor.MessageBoxOptions()
            {
                 Title = title,
                 Message = message
            });
        }

        public async Task<bool> ShowOkCancelAsync(string message, string title = "Title")
        {
            return await MudDialogService.ShowMessageBoxAsync(new MudBlazor.MessageBoxOptions()
            {
                Title = title,
                Message = message,
                CancelText = "Cancel",

            }) ?? false;
        }

        public async Task<int> ShowYesNoAbortAsync(string message, string title = "Title")
        {
            var result = await MudDialogService.ShowMessageBoxAsync(new MudBlazor.MessageBoxOptions()
            {
                Title = title,
                Message = message,
                YesText = "Yes",
                NoText = "No",
                CancelText = "Abort",

            });
            return result switch
            {
                true => 0,
                false => 1,
                _ => 2
            };
        }

        public async Task<bool> ShowYesNoAsync(string message, string title = "Title")
        {
            return await MudDialogService.ShowMessageBoxAsync(new MudBlazor.MessageBoxOptions()
            {
                Title = title,
                Message = message,
                CancelText = "No",

            }) ?? false;
        }

        public async Task<int> ShowYesNoCancelAsync(string message, string title = "Title")
        {
            var result = await MudDialogService.ShowMessageBoxAsync(new MudBlazor.MessageBoxOptions()
            {
                Title = title,
                Message = message,
                YesText = "Yes",
                NoText = "No",
                CancelText = "Cancel",
            });
            return result switch
            {
                true => 0,
                false => 1,
                _ => 2
            };
        }

        private Type GetViewType(string viewModelName, NavigationContext? context = null)
        {
            return (Type)GetView(viewModelName, context is null ? [] : [context])!;
        }

        async Task<TResult?> Avae.Services.IDialogService.ShowModalAsync<TViewModel, TResult>(NavigationContext? context)
            where TResult : default
        {
            var tcs = new TaskCompletionSource<TResult?>();
            var viewModel = serviceProvider.GetViewModel<TViewModel>(context);
            var type = GetViewType(typeof(TViewModel).Name, context);            
            var dialog = await MudDialogService.ShowAsync(type, viewModel.Title, new MudBlazor.DialogParameters()
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
    }
}
