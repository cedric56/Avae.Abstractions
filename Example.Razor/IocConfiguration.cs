using Avae.Abstractions;
using Avae.Services;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using NavigationContext = Avae.Abstractions.NavigationContext;

namespace Example.Razor
{
    internal class IocConfiguration(IServiceProvider serviceProvider, Action<IIocContainer>? configure = null) :
            IIocConfiguration, ITaskDialogService, IContentDialogService, 
            Avae.Services.IDialogService,
            ISystemNotificationService,
            INotificationManager,
            IRequestedTheme
    {
        public static MudBlazor.IDialogService MudDialogService { get; set; } = default!;

        IocContainer? _container = null;
        IocContainer Container { get => _container ??= (IocContainer)serviceProvider.GetRequiredService<IIocContainer>(); }
        public int MaxItems => throw new NotImplementedException();

        public NotificationPosition Position => throw new NotImplementedException();

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
            throw new NotImplementedException();
        }

        public void Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Action? onClick = null, Action? onClose = null)
        {
            throw new NotImplementedException();
        }

        public Task<ContentDialogResult> ShowAsync(ContentDialogParams @params)
        {
            throw new NotImplementedException();
        }

        public Task<TaskDialogStandardResult> ShowAsync(TaskDialogParams @params, params TaskDialogStandardResult[] results)
        {
            throw new NotImplementedException();
        }

        public Task ShowErrorAsync(Exception ex, string title = "Error")
        {
            throw new NotImplementedException();
        }

        public Task<bool> ShowOkAbortAsync(string message, string title = "Title")
        {
            throw new NotImplementedException();
        }

        public async Task ShowOkAsync(string message, string title = "Title")
        {
            await MudDialogService.ShowMessageBoxAsync(new MudBlazor.MessageBoxOptions()
            {
                 Title = title,
                 Message = message
            });
        }

        public Task<bool> ShowOkCancelAsync(string message, string title = "Title")
        {
            throw new NotImplementedException();
        }

        public Task<int> ShowYesNoAbortAsync(string message, string title = "Title")
        {
            throw new NotImplementedException();
        }

        public Task<bool> ShowYesNoAsync(string message, string title = "Title")
        {
            throw new NotImplementedException();
        }

        public Task<int> ShowYesNoCancelAsync(string message, string title = "Title")
        {
            throw new NotImplementedException();
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
