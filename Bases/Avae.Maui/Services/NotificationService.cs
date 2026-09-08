using Avae.Services;
using UXDivers.Popups.Maui.Controls;
using UXDivers.Popups.Services;

namespace Avae.Maui.Services;

internal class NotificationService(Helper helper) : INotificationService
{
    /// <summary>
    /// Displays a transient popup notification with a type-colored icon, optionally auto-dismissing
    /// after <paramref name="expiration"/> and invoking callbacks on tap or close.
    /// </summary>
    /// <param name="title">The notification title.</param>
    /// <param name="message">The notification message.</param>
    /// <param name="type">The notification type, used to color the icon. Defaults to <see cref="NotificationType.Information"/>.</param>
    /// <param name="expiration">Optional duration after which the notification is automatically dismissed if not already closed.</param>
    /// <param name="onClick">Optional callback invoked when the notification is tapped.</param>
    /// <param name="onClose">Optional callback invoked once the notification has been dismissed.</param>
    public async void Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Action? onClick = null, Action? onClose = null)
    {
        helper.Ensure();

        var pop = new FloaterPopup()
        {
            Text = message,
            Title = title
        };

        if (Application.Current?.RequestedTheme == AppTheme.Light)
        {
            pop.PopupBackground = Colors.White;
            //pop.Background = Color.FromArgb("#ffb2b2b2");
        }

        pop.IconColor = type switch
        {
            NotificationType.Success => Colors.Green,
            NotificationType.Warning => Colors.Orange,
            NotificationType.Information => Colors.Blue,
            NotificationType.Error => Colors.Red,
            _ => Colors.Blue
        };

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += Tapped;
        pop.GestureRecognizers.Add(tapGesture);

        bool isClosed = false;
        if (expiration.HasValue)
        {
            _ = Task.Run(Delay);
        }

        await IPopupService.Current.PushAsync(pop);
        isClosed = true;
        onClose?.Invoke();

        async void Tapped(object? sender, TappedEventArgs e)
        {
            onClick?.Invoke();
            await IPopupService.Current.PopAsync(pop);
        }

        async Task Delay()
        {
            await Task.Delay((int)expiration.Value.TotalMilliseconds);
            if (!isClosed)
                await IPopupService.Current.PopAsync(pop);
        }
    }
}
