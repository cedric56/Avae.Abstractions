using Avae.DAL.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace Avae.DAL
{
    public class FiveSecondsReconnectPolicy : IRetryPolicy
    {
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            return TimeSpan.FromSeconds(5);
        }
    }
    public class SignalRService : ISignalRService, IDisposable
    {
        private HubConnection Hub { get; }

        public SignalRService(string url, IRetryPolicy? retryPolicy = null)
        {
            Hub = new HubConnectionBuilder()
                //.WithServerTimeout(TimeSpan.FromSeconds(5))
                .WithUrl(url)
                .WithAutomaticReconnect(retryPolicy ?? new FiveSecondsReconnectPolicy())
                .Build();
            Hub.Closed += OnClosedAsync;
            Hub.Reconnecting += OnReconnectingAsync;
            Hub.Reconnected += OnReconnectedAsync;
        }

        private Task OnReconnectedAsync(string? arg)
        {
            Reconnected?.Invoke(this, true);
            return Task.CompletedTask;
        }

        private Task OnReconnectingAsync(Exception? arg)
        {
            Reconnected?.Invoke(this, true);
            return Task.CompletedTask;
        }

        private Task OnClosedAsync(Exception? arg)
        {
            Reconnected?.Invoke(this, true);
            return Task.CompletedTask;
        }

        public TimeSpan DefaultServerTimeout => HubConnection.DefaultServerTimeout;

        public TimeSpan DefaultHandshakeTimeout => HubConnection.DefaultHandshakeTimeout;

        public TimeSpan DefaultKeepAliveInterval => HubConnection.DefaultKeepAliveInterval;

        public TimeSpan HandshakeTimeout { get => Hub.HandshakeTimeout; set => Hub.HandshakeTimeout = value; }
        public TimeSpan KeepAliveInterval { get => Hub.KeepAliveInterval; set => Hub.KeepAliveInterval = value; }
        public TimeSpan ServerTimeout { get => Hub.ServerTimeout; set => Hub.ServerTimeout = value; }

        public bool Connected => Hub.State == HubConnectionState.Connected;

        public event EventHandler<bool>? Reconnected;

        public ValueTask DisposeAsync()
        {
            return Hub.DisposeAsync();
        }

        public Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync<TResult>(methodName, arg1, arg2, arg3, arg4, arg5, arg6, cancellationToken);
        }

        public Task InvokeAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync(methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, cancellationToken);
        }

        public Task InvokeAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync(methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, cancellationToken);
        }

        public Task InvokeAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync(methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, cancellationToken);
        }

        public Task InvokeAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync(methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, cancellationToken);
        }

        public Task InvokeAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync(methodName, arg1, arg2, arg3, arg4, arg5, arg6, cancellationToken);
        }

        public Task InvokeAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync(methodName, arg1, arg2, arg3, arg4, arg5, cancellationToken);
        }

        public Task InvokeAsync(string methodName, object arg1, object arg2, object arg3, object arg4, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync(methodName, arg1, arg2, arg3, arg4, cancellationToken);
        }

        public Task InvokeAsync(string methodName, object arg1, object arg2, object arg3, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync(methodName, arg1, arg2, arg3, cancellationToken);
        }

        public Task InvokeAsync(string methodName, object arg1, object arg2, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync(methodName, arg1, arg2, cancellationToken);
        }

        public Task<TResult> InvokeAsync<TResult>(string methodName, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync<TResult>(methodName, cancellationToken);
        }

        public Task InvokeAsync(string methodName, object arg1, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync(methodName, arg1, cancellationToken);
        }

        public Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync<TResult>(methodName, arg1, arg2, cancellationToken);
        }

        public Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, object arg3, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync<TResult>(methodName, arg1, arg2, arg3, cancellationToken);
        }

        public Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, object arg3, object arg4, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync<TResult>(methodName, arg1, arg2, arg3, arg4, cancellationToken);
        }

        public Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync<TResult>(methodName, arg1, arg2, arg3, arg4, arg5, cancellationToken);
        }

        public Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync<TResult>(methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, cancellationToken);
        }

        public Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync<TResult>(methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, cancellationToken);
        }

        public Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync<TResult>(methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, cancellationToken);
        }

        public Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync<TResult>(methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, cancellationToken);
        }

        public Task InvokeAsync(string methodName, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync(methodName, cancellationToken);
        }

        public Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeAsync<TResult>(methodName, arg1, cancellationToken);
        }

        public Task<TResult> InvokeCoreAsync<TResult>(string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeCoreAsync<TResult>(methodName, args, cancellationToken);
        }

        public Task InvokeCoreAsync(string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            return Hub.InvokeCoreAsync(methodName, args, cancellationToken);
        }

        public IDisposable On(string methodName, Type[] parameterTypes, Func<object?[], object, Task> handler, object state)
        {
            return Hub.On(methodName, parameterTypes, handler, state);
        }

        public IDisposable On(string methodName, Action handler)
        {
            return Hub.On(methodName, handler);
        }

        public IDisposable On<T1, T2, T3, T4, T5, T6, T7, T8>(string methodName, Action<T1, T2, T3, T4, T5, T6, T7, T8> handler)
        {
            return Hub.On(methodName, handler);
        }

        public IDisposable On<T1, T2, T3, T4, T5, T6, T7>(string methodName, Action<T1, T2, T3, T4, T5, T6, T7> handler)
        {
            return Hub.On(methodName, handler);
        }

        public IDisposable On<T1, T2, T3, T4, T5, T6>(string methodName, Action<T1, T2, T3, T4, T5, T6> handler)
        {
            return Hub.On(methodName, handler);
        }

        public IDisposable On<T1, T2, T3, T4, T5>(string methodName, Action<T1, T2, T3, T4, T5> handler)
        {
            return Hub.On(methodName, handler);
        }

        public IDisposable On<T1, T2, T3, T4>(string methodName, Action<T1, T2, T3, T4> handler)
        {
            return Hub.On(methodName, handler);
        }

        public IDisposable On<T1, T2, T3>(string methodName, Action<T1, T2, T3> handler)
        {
            return Hub.On(methodName, handler);
        }

        public IDisposable On<T1, T2>(string methodName, Action<T1, T2> handler)
        {
            return Hub.On(methodName, handler);
        }

        public IDisposable On<T1>(string methodName, Action<T1> handler)
        {
            return Hub.On(methodName, handler);
        }

        public IDisposable On(string methodName, Type[] parameterTypes, Func<object?[], Task> handler)
        {
            return Hub.On(methodName, parameterTypes, handler);
        }

        public void Remove(string methodName)
        {
            Hub.Remove(methodName);
        }

        public Task SendAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, CancellationToken cancellationToken = default)
        {
            return Hub.SendAsync(methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, cancellationToken);
        }

        public Task SendAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, CancellationToken cancellationToken = default)
        {
            return Hub.SendAsync(methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, cancellationToken);
        }

        public Task SendAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, CancellationToken cancellationToken = default)
        {
            return Hub.SendAsync(methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, cancellationToken);
        }

        public Task SendAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, CancellationToken cancellationToken = default)
        {
            return Hub.SendAsync(methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, cancellationToken);
        }

        public Task SendAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, CancellationToken cancellationToken = default)
        {
            return Hub.SendAsync(methodName, arg1, arg2, arg3, arg4, arg5, cancellationToken);
        }

        public Task SendAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, CancellationToken cancellationToken = default)
        {
            return Hub.SendAsync(methodName, arg1, arg2, arg3, arg4, arg5, arg6, cancellationToken);
        }

        public Task SendAsync(string methodName, object arg1, object arg2, object arg3, CancellationToken cancellationToken = default)
        {
            return Hub.SendAsync(methodName, arg1, arg2, arg3, cancellationToken);
        }

        public Task SendAsync(string methodName, object arg1, object arg2, CancellationToken cancellationToken = default)
        {
            return Hub.SendAsync(methodName, arg1, arg2, cancellationToken);
        }

        public Task SendAsync(string methodName, object arg1, CancellationToken cancellationToken = default)
        {
            return Hub.SendAsync(methodName, arg1, cancellationToken);
        }

        public Task SendAsync(string methodName, CancellationToken cancellationToken = default)
        {
            return Hub.SendAsync(methodName, cancellationToken);
        }

        public Task SendAsync(string methodName, object arg1, object arg2, object arg3, object arg4, CancellationToken cancellationToken = default)
        {
            return Hub.SendAsync(methodName, arg1, arg2, arg3, arg4, cancellationToken);
        }

        public Task SendCoreAsync(string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            return Hub.SendCoreAsync(methodName, args, cancellationToken);
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            return Hub.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Hub.StopAsync(cancellationToken);
        }

        public event Func<Exception, Task> Closed
        {
            add
            {
                Hub.Closed += value;
            }
            remove
            {
                Hub.Closed -= value;
            }
        }

        public void Dispose()
        {
            Hub.Closed -= OnClosedAsync;
            Hub.Reconnecting -= OnReconnectingAsync;
            Hub.Reconnected -= OnReconnectedAsync;

            GC.SuppressFinalize(this);
        }
    }
}
