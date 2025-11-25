namespace Avae.DAL
{
    public interface ISignalRService
    {
        TimeSpan DefaultServerTimeout { get; }
        TimeSpan DefaultHandshakeTimeout { get; }
        TimeSpan DefaultKeepAliveInterval { get; }
        TimeSpan HandshakeTimeout { get; set; }
        TimeSpan KeepAliveInterval { get; set; }
        TimeSpan ServerTimeout { get; set; }
        bool Connected { get; }
        event Func<Exception, Task> Closed;
        event EventHandler<bool> Reconnected;
        ValueTask DisposeAsync();
        IDisposable On(string methodName, Type[] parameterTypes, Func<object?[], object, Task> handler, object state);
        void Remove(string methodName);
        Task SendCoreAsync(string methodName, object[] args, CancellationToken cancellationToken = default);
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
        Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, CancellationToken cancellationToken = default);
        Task InvokeAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, CancellationToken cancellationToken = default);
        Task InvokeAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, CancellationToken cancellationToken = default);
        Task InvokeAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, CancellationToken cancellationToken = default);
        Task InvokeAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, CancellationToken cancellationToken = default);
        Task InvokeAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, CancellationToken cancellationToken = default);
        Task InvokeAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, CancellationToken cancellationToken = default);
        Task InvokeAsync(string methodName, object arg1, object arg2, object arg3, object arg4, CancellationToken cancellationToken = default);
        Task InvokeAsync(string methodName, object arg1, object arg2, object arg3, CancellationToken cancellationToken = default);
        Task InvokeAsync(string methodName, object arg1, object arg2, CancellationToken cancellationToken = default);
        Task<TResult> InvokeAsync<TResult>(string methodName, CancellationToken cancellationToken = default);
        Task InvokeAsync(string methodName, object arg1, CancellationToken cancellationToken = default);
        Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, CancellationToken cancellationToken = default);
        Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, object arg3, CancellationToken cancellationToken = default);
        Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, object arg3, object arg4, CancellationToken cancellationToken = default);
        Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, CancellationToken cancellationToken = default);
        Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, CancellationToken cancellationToken = default);
        Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, CancellationToken cancellationToken = default);
        Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, CancellationToken cancellationToken = default);
        Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, CancellationToken cancellationToken = default);
        Task InvokeAsync(string methodName, CancellationToken cancellationToken = default);
        Task<TResult> InvokeAsync<TResult>(string methodName, object arg1, CancellationToken cancellationToken = default);
        Task<TResult> InvokeCoreAsync<TResult>(string methodName, object[] args, CancellationToken cancellationToken = default);
        Task InvokeCoreAsync(string methodName, object[] args, CancellationToken cancellationToken = default);
        IDisposable On(string methodName, Action handler);
        IDisposable On<T1, T2, T3, T4, T5, T6, T7, T8>(string methodName, Action<T1, T2, T3, T4, T5, T6, T7, T8> handler);
        IDisposable On<T1, T2, T3, T4, T5, T6, T7>(string methodName, Action<T1, T2, T3, T4, T5, T6, T7> handler);
        IDisposable On<T1, T2, T3, T4, T5, T6>(string methodName, Action<T1, T2, T3, T4, T5, T6> handler);
        IDisposable On<T1, T2, T3, T4, T5>(string methodName, Action<T1, T2, T3, T4, T5> handler);
        IDisposable On<T1, T2, T3, T4>(string methodName, Action<T1, T2, T3, T4> handler);
        IDisposable On<T1, T2, T3>(string methodName, Action<T1, T2, T3> handler);
        IDisposable On<T1, T2>(string methodName, Action<T1, T2> handler);
        IDisposable On<T1>(string methodName, Action<T1> handler);
        IDisposable On(string methodName, Type[] parameterTypes, Func<object?[], Task> handler);
        Task SendAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, CancellationToken cancellationToken = default);
        Task SendAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, CancellationToken cancellationToken = default);
        Task SendAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, CancellationToken cancellationToken = default);
        Task SendAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, CancellationToken cancellationToken = default);
        Task SendAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, CancellationToken cancellationToken = default);
        Task SendAsync(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, CancellationToken cancellationToken = default);
        Task SendAsync(string methodName, object arg1, object arg2, object arg3, CancellationToken cancellationToken = default);
        Task SendAsync(string methodName, object arg1, object arg2, CancellationToken cancellationToken = default);
        Task SendAsync(string methodName, object arg1, CancellationToken cancellationToken = default);
        Task SendAsync(string methodName, CancellationToken cancellationToken = default);
        Task SendAsync(string methodName, object arg1, object arg2, object arg3, object arg4, CancellationToken cancellationToken = default);
    }
}
