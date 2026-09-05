using Avae.Core;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Avae.DAL.SignalR;

public static class Extensions
{
    public const string DBMessage = "DBChanged";

    public static async Task<Func<Task>> AddSignalR<TObject>(
        this IDBMonitor<TObject> monitor, 
        string url, 
        IRetryPolicy? retryPolicy  =null,
        Func<HttpMessageHandler, HttpMessageHandler>? factory = null,
        ILogger? logger = null)
        where TObject : class, new()
    {
        var hub = new HubConnectionBuilder()
            .AddMessagePackProtocol()
             //.WithServerTimeout(TimeSpan.FromSeconds(5))
             .WithUrl(url, options =>
             {
                 if (OperatingSystem.IsBrowser())
                 {
                     options.Transports = HttpTransportType.WebSockets;
                     options.SkipNegotiation = false; // keep negotiate unless you're sure WS-only is safe
                 }

                 // ✅ FIX: Configure HttpClient for Android SSL validation
                 if (factory != null)
                     options.HttpMessageHandlerFactory = factory;
             })
            .WithAutomaticReconnect(retryPolicy ?? new FiveSecondsReconnectPolicy())
            .Build();

        hub.On<Record<TObject>>(DBMessage, record=>
        {
            //we stop propagating to avoid stackoverflow
            if (record.Contains(hub.ConnectionId))
                return;

            monitor.OnChanged(record);
        });
        //monitor.Restart = TryConnect;
        monitor.OnRecordChanged += OnRecordChanged;
        hub.Closed += Closed;
        hub.Reconnected += Reconnected;
        await TryConnect();
        return async () =>
        {
            hub.Closed -= Closed;
            hub.Reconnected -= Reconnected;

            try
            {
                await hub.StopAsync();
                await hub.DisposeAsync();
            }
            finally
            {
                monitor.OnRecordChanged -= OnRecordChanged;
            }
        };

        async void OnRecordChanged(object? sender, Record<TObject> e)
        {
            e.Add(hub.ConnectionId);

            if (hub.State == HubConnectionState.Connected)
                await hub.InvokeAsync(nameof(SignalRHub<TObject>.OnRecordChanged), e);

            //If an embedded server, we notify clients
            var signal = ServiceLocator.GetService<SignalRHub<TObject>>();
            if (signal is not null)
                signal.OnRecordChanged(e);
        }

        async Task TryConnect()
        {
            try
            {
                //using var cts = new CancellationTokenSource(
                //      TimeSpan.FromSeconds(2));

                //HttpResponseMessage response;
                //if (ServiceLocator.GetService<HttpClient>() is { } client)
                //{
                //    response = await client.PostAsync($"{url}/negotiate", null, cts.Token);
                //}
                //else
                //{
                //    using var httpClient = factory != null ? new HttpClient(factory.Invoke(null!)) : new HttpClient();
                //    response = await httpClient.PostAsync($"{url}/negotiate", null, cts.Token);
                //}
                //if (response.IsSuccessStatusCode)
                //    await hub.StartAsync();
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                await hub.StartAsync(cts.Token);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.Message);
            }
            finally
            {
                //monitor.IsRunning = hub.State == HubConnectionState.Connected;
                //if (monitor.IsRunning)
                IDBLayer.Sessions.Add(typeof(TObject), hub.ConnectionId ?? throw new InvalidOperationException("Connection must be known"));
            }
        }

        Task Reconnected(string? value)
        {
            //monitor.IsRunning = hub.State == HubConnectionState.Connected;
            return Task.CompletedTask;
        }

        Task Closed(Exception? ex)
        {
            //monitor.IsRunning = hub.State == HubConnectionState.Connected;
            return Task.CompletedTask;
        }
    }
}
