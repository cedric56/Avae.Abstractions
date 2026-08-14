using Avae.Abstractions;
using Avae.DAL;
using System.Diagnostics;

namespace Avae.SignalR
{
    public static class Extensions
    {
        public const string DBMessage = "DBChanged";

        public static async Task<Func<Task>> AddSignalR<TObject>(
            this IDBMonitor<TObject> monitor, 
            string url)
            where TObject : class, new()
        {
            var signalRService = new SignalRService(url);
            signalRService.On<Record<TObject>>(DBMessage, record=>
            {
                //we stop propagating to avoid stackoverflow
                if (record.Contains(signalRService.Hub.ConnectionId))
                    return;

                monitor.OnChanged(record);
            });
            monitor.Restart = TryConnect;
            monitor.OnRecordChanged += OnRecordChanged;
            signalRService.Closed += Closed;
            signalRService.Reconnected += Reconnected;
            await TryConnect();
            return async () =>
            {
                try
                {
                    await signalRService.StopAsync();
                    await signalRService.DisposeAsync();
                }
                finally
                {
                    monitor.OnRecordChanged -= OnRecordChanged;
                    signalRService.Closed -= Closed;
                    signalRService.Reconnected -= Reconnected;
                }
            };

            async Task TryConnect()
            {
                try
                {
                    using var cts = new CancellationTokenSource(
                          OperatingSystem.IsBrowser() ?
                          TimeSpan.FromSeconds(2) :
                          TimeSpan.FromSeconds(1));

                    HttpResponseMessage response;                    
                    if (ServiceLocator.GetService<HttpClient>() is { } client)
                    {
                        response = await client.PostAsync($"{url}/negotiate", null, cts.Token);
                    }
                    else
                    {
                        using var httpClient = new HttpClient();
                        response = await httpClient.PostAsync($"{url}/negotiate", null, cts.Token);
                    }
                    if (response.IsSuccessStatusCode)
                        await signalRService.StartAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    monitor.IsRunning = signalRService.Connected;
                }
            }

            Task Reconnected(string? value)
            {
                monitor.IsRunning = signalRService.Connected;
                return Task.CompletedTask;
            }

            Task Closed(Exception ex)
            {
                monitor.IsRunning = signalRService.Connected;
                return Task.CompletedTask;
            }

            async void OnRecordChanged(object? sender, Record<TObject> e)
            {
                e.Add(signalRService.Hub.ConnectionId);

                if (signalRService.Connected)
                    await signalRService.InvokeAsync(nameof(SignalRHub<TObject>.OnRecordChanged), signalRService, e);

                //If an embedded server, we notify clients
                var hub = ServiceLocator.GetService<SignalRHub<TObject>>();
                if (hub is not null)
                    hub.OnRecordChanged(signalRService, e);
            }
        }
    }
}
