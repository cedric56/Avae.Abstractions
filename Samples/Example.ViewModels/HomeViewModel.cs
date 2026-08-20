using Avae.Essentials;
using Avae.Services;
using Avae.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using System.Diagnostics;

namespace Example.ViewModels;

public partial class HomeViewModel(
    IDialogService dialogService,
    IContentDialogService contentDialogService,
    ITaskDialogService taskDialogService,
    IIocConfiguration iocConfiguration,
    INotificationService notificationManager,
    ISystemNotificationService systemNotificationService,
    IRequestedThemeService requestedTheme,
    ITextToSpeech textToSpeech,
    IShare share,
    IFilePicker filePicker,
    IMediaPicker mediaPicker) : ObservableObject, IViewModelBase
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
        notificationManager.Show(
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
            var notification = await systemNotificationService.CreateNotification(
                "action",
                "Hello",
                "World",
                [new SystemNotificationAction("caption", "tag"), new SystemNotificationAction("Test", "test"),]
                );

            if (notification != null)
            {
                notification.NotificationCompleted += OnNotificationCompleted;
                notification.Show();
            }
            void OnNotificationCompleted(object? sender, SystemNotificationEventArgs e)
            {
                notification.NotificationCompleted -= OnNotificationCompleted;
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

    [RelayCommand]
    public Task Speak()
    {
        return textToSpeech.SpeakAsync("Done");
    }

    [RelayCommand]
    public async Task ShowShare()
    {
        var files = await filePicker.PickMultipleAsync();
        await share.RequestAsync("hello", files ?? []);
    }

    [RelayCommand]
    public async Task CaptureVideo()
    {
        await mediaPicker.CaptureVideoAsync();
    }
}
