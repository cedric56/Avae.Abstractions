using Avae.Services;
using Microsoft.AspNetCore.Components;

namespace Avae.Razor.Services;

internal class NotificationService : INotificationService
{
    public static MudBlazor.ISnackbar SnackbarService { get; set; } = default!;

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
}
