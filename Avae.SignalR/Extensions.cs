using Avae.Abstractions;
using Avae.DAL;
using System.Diagnostics;

namespace Avae.SignalR
{
    public static class Extensions
    {
        public const string DBMessage = "DBChanged";

        public static SignalRService AddSignalR<TObject>(this DBMonitor<TObject> monitor, string url, out Action unsuscribe)
            where TObject : class, new()
        {
            var signalRService = new SignalRService(url);
            signalRService.On<Record<TObject>>(DBMessage, record=>
            {
                //we stop propagating to avoid stackoverflow
                if (signalRService.Hub.ConnectionId != null && 
                    record.Connections.Contains(signalRService.Hub.ConnectionId))
                    return;

                monitor.OnChanged(record);
            });
            monitor.OnRecordChanged += OnRecordChanged;
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
                monitor.OnRecordChanged -= OnRecordChanged;
            };

            return signalRService;

            async void OnRecordChanged(object? sender, Record<TObject> e)
            {
                if (signalRService.Hub.ConnectionId != null)
                    e.Connections.Add(signalRService.Hub.ConnectionId);

                if (signalRService.Connected)
                    await signalRService.InvokeAsync(nameof(SqlHub<TObject>.OnRecordChanged), signalRService, e);

                //If an embedded server, we notify clients
                var hub = ServiceLocator.GetService<SqlHub<TObject>>();
                if (hub is not null)
                    hub.OnRecordChanged(signalRService, e);
            }
        }
    }
}
