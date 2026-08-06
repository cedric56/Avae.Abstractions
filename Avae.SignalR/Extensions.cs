using Avae.Abstractions;
using Avae.DAL;
using Avae.DAL.Interfaces;
using System.Diagnostics;

namespace Avae.SignalR
{
    public static class Extensions
    {
        public static SignalRService AddSignalR<TObject>(this SqlMonitor<TObject> monitor, string url, out Action unsuscribe)
            where TObject : class, new()
        {
            var signalRService = new SignalRService(url);
            signalRService.On<Record<TObject>>(Messages.DBMessage, record=>
            {
                if (signalRService.Hub.ConnectionId != null && record.ConnectionId.Contains(signalRService.Hub.ConnectionId))
                    return;

                monitor.Changed(record);
            });
            monitor.OnChanged += Monitor_OnChanged;
            Task.Run(async () =>
            {
                try
                {
                    await signalRService.StartAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            });

            unsuscribe = () =>
            {
                monitor.OnChanged -= Monitor_OnChanged;
            };

            return signalRService;

            async void Monitor_OnChanged(object? sender, IRecord<TObject> e)
            {
                if (e is Record<TObject> record && signalRService.Hub.ConnectionId != null)
                    record.ConnectionId.Add(signalRService.Hub.ConnectionId);

                await signalRService.InvokeAsync(nameof(SqlHub<TObject>.SendMessage), e);

                //Inside server, we notify clients
                var hub = ServiceLocator.GetService<SqlHub<TObject>>();
                if (hub is not null)
                    hub.SendMessage(e);
            }
        }
    }
}
