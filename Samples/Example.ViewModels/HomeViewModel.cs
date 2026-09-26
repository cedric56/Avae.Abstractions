using Avae.DAL;
using Avae.Services;
using Avae.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using Example.Models;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Diagnostics;

namespace Example.ViewModels;

public partial class HomeViewModel(
    IDialogService dialogService,
    IContentDialogService contentDialogService,
    ITaskDialogService taskDialogService,
    IServiceProvider provider,
    INotificationService notificationService,
    IRequestedThemeService requestedTheme,
    IDBMonitor<Person> monitor) :
    ReactiveUI.ReactiveObject,
    //IRoutableViewModel,
    //ObservableObject,
    IEquatable<HomeViewModel>,
    IDisposable,
    INavigable
{
    [ReactiveCommand]
    public void Messenger()
    {
        WeakReferenceMessenger.Default.Send("Hello from HomeViewModel", this);
    }


    [ReactiveCommand]
    public async Task ShowModal()
    {
        string? result = string.Empty;
        try
        {
            result = await provider.ShowModalAsync<ModalViewModel, string?>();
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }
        finally
        {
            await dialogService.ShowYesNoAbortAsync(result ?? string.Empty, "Result");
        }
    }

    public const string TaskDialogKey = "TaskDialog";

    [ReactiveCommand]
    public async Task ShowTaskDialog()
    {
        await taskDialogService.ShowAsync(new TaskDialogParams()
        {
            Header = "Header",
            Footer = provider.GetView(TaskDialogKey, ["Footer"]),
            IconSource = provider.GetView(TaskDialogKey, ["IconSource"]),
            Title = "Title",
            SubHeader = "SubHeader",
            Content = provider.GetView(TaskDialogKey, ["Content"]),
            FooterVisibility = TaskDialogFooterVisibility.Auto
        },
        TaskDialogStandardResult.OK,
        TaskDialogStandardResult.Cancel);
    }

    [ReactiveCommand]
    public async Task ShowContentDialog()
    {
        await contentDialogService.ShowAsync(new ContentDialogParams()
        {
            Title = "Title",
            CloseButtonText = "Close",
            Content = provider.GetView(TaskDialogKey, ["Content"]),
        });
    }

    [ReactiveCommand]
    public async Task ShowNotification()
    {
        notificationService.Show(
            "Hello",
            "World",
            NotificationType.Success,
            TimeSpan.FromSeconds(2),
            () =>
            {
                notificationService.Show("Clicked", "Click", NotificationType.Information);
            },
            () =>
            {
                notificationService.Show("Closed", "Close", NotificationType.Information);
            });
    }

    [ReactiveCommand]
    public async Task ShowSystemNotification()
    {
        try
        {
            var systemNotificationService = provider.GetService<ISystemNotificationService>();
            if (systemNotificationService == null)
                return;

            systemNotificationService.NotificationCompleted -= OnNotificationCompleted;
            systemNotificationService.NotificationCompleted += OnNotificationCompleted;

            var notification = await systemNotificationService.CreateNotification(null);
            if (notification != null)
            {
                //notification.Vibrate = [200,100,200,100];
                notification.Title = "Hello";
                notification.Message = "World";
                //notification.Expiration = TimeSpan.FromSeconds(1);
                notification.SetActions([new SystemNotificationAction("caption", "reply"), new SystemNotificationAction("Test", "test"),]);
                notification.ReplyActionTag = "reply";//must match action tag for an input
                await notification.Show();
            }
            void OnNotificationCompleted(object? sender, SystemNotificationEventArgs e)
            {
                var actives = systemNotificationService.ActiveNotifications();
                var current = actives.FirstOrDefault(a => a.Key == e.NotificationId);

                notificationService.Show(
                    $"Notification {e.NotificationId.ToString() ?? string.Empty}",
                    $"Cancelled:{e.IsCancelled} Activated:{e.IsActivated} ActionTag:{e.ActionTag} UserData:{e.UserData}");

                current.Value?.Close();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    RequestedTheme? actual = null;

    [ReactiveCommand]
    public async Task ShowRequestedTheme()
    {
        var theme = actual switch
        {
            RequestedTheme.Light => RequestedTheme.Dark,
            RequestedTheme.Dark => RequestedTheme.Light,
            _ => RequestedTheme.Light
        };
        actual = theme;
        requestedTheme.Request(actual.Value);
    }

    public bool Equals(HomeViewModel? other)
    {
        return this == other;
    }

    public void Dispose()
    {
        unsuscribe?.Invoke();
    }

    private bool _isfirstLoad = true;
    Func<Task>? unsuscribe;
    public async Task OnNavigatedTo(NavigableContext context)
    {
        if(_isfirstLoad)
        {
            _isfirstLoad = false;
            var http = provider.GetService<HttpMessageHandler>();
            unsuscribe = await monitor.AddStreamingHub(Constants.MagicHubUrl, provider.GetRequiredService<IDBFactory>(), http);

            //Func<HttpMessageHandler, HttpMessageHandler> factory = null!;
            //if (http != null)
            //    factory = _ => http;

            //unsuscribe = await monitor.AddSignalR(Constants.SignalHubUrl, factory: factory);
        }
    }
}