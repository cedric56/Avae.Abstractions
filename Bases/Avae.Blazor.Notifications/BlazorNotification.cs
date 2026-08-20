using Avae.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avae.Blazor.Notifications;

class BlazorNotification : ISystemNotification
{
    private static uint s_currentId = 0;
    Func<uint, Task> show;

    public BlazorNotification(Func<uint, Task> show)
    {
        this.show = show;

        Id = GetNextId();
    }

    public uint Id { get; }
    public event EventHandler<SystemNotificationEventArgs>? NotificationCompleted;

    public void Close()
    {
        throw new NotImplementedException();
    }

    public async void Show()
    {
        await show(Id);
    }

    public void RaiseCompleted(SystemNotificationEventArgs e)
    {
        NotificationCompleted?.Invoke(this, e);
    }

    private static uint GetNextId()
    {
        return Interlocked.Increment(ref s_currentId);
    }
}
