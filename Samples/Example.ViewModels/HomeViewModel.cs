using Avae.Services;
using Avae.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace Example.ViewModels;

public partial class HomeViewModel(
    IDialogService dialogService,
    IContentDialogService contentDialogService,
    ITaskDialogService taskDialogService,
    IIocConfiguration iocConfiguration,
    INotificationService notificationService,
    ISystemNotificationService systemNotificationService,
    IRequestedThemeService requestedTheme) : ObservableObject, IViewModelBase
{
    public static string Title => "Welcome to home";

    [RelayCommand]
    public async Task ShowModal()
    {
        string? result = string.Empty;
        try
        {
            result = await dialogService.ShowModalAsync<ModalViewModel, string?>();
        }
        catch(Exception ex)
        {
            result = ex.Message;
        }
        finally
        {
            await dialogService.ShowOkCancelAsync(result ?? string.Empty, "Result");
        }
    }

    public const string TaskDialogKey = "TaskDialog";

    [RelayCommand]
    public async Task ShowTaskDialog()
    {
        await taskDialogService.ShowAsync(new TaskDialogParams()
        {
            Header = "Header",
            Footer = iocConfiguration.GetView(TaskDialogKey, "Footer"),
            IconSource = iocConfiguration.GetView(TaskDialogKey, "IconSource"),
            Title = "Title",
            SubHeader = "SubHeader",
            Content = iocConfiguration.GetView(TaskDialogKey, "Content"),
            FooterVisibility = TaskDialogFooterVisibility.Auto
        },
        TaskDialogStandardResult.OK,
        TaskDialogStandardResult.Cancel);
    }

    [RelayCommand]
    public async Task ShowContentDialog()
    {
        await contentDialogService.ShowAsync(new ContentDialogParams()
        {
            Title = "Title",
            CloseButtonText = "Close",
            Content = iocConfiguration.GetView(TaskDialogKey, "Content"),
        });
    }

    [RelayCommand]
    public async Task ShowNotification()
    {
        notificationService.Show(
            "Hello",
            "World",
            NotificationType.Success,
            TimeSpan.FromSeconds(2));
    }

    [RelayCommand]
    public async Task ShowSystemNotification()
    {
        try
        {
            systemNotificationService.NotificationCompleted -= OnNotificationCompleted;
            systemNotificationService.NotificationCompleted += OnNotificationCompleted;

            var notification = await systemNotificationService.CreateNotification(null);
            if (notification != null)
            {
                //notification.Vibrate = [200,100,200,100];
                notification.Title = "Hello";
                notification.Message = "World";
                notification.Expiration = TimeSpan.FromSeconds(1);
                notification.SetActions([new SystemNotificationAction("caption", "reply"), new SystemNotificationAction("Test", "test"),]);
                notification.ReplyActionTag = "reply";//must match action tag for an input
                notification.Show();
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

    [RelayCommand]
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
}
