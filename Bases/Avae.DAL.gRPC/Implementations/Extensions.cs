namespace Avae.DAL.gRPC;

public static class Extensions
{
    public static async Task<TResult> TimeoutAfter<TResult>(this ValueTask<TResult> task, TimeSpan timeout)
    {
        using var timeoutCancellationTokenSource = new CancellationTokenSource();

        var t = task.AsTask();
        var completedTask = await Task.WhenAny(t, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
        if (completedTask == t)
        {
            timeoutCancellationTokenSource.Cancel();
            return await t;  // Very important in order to propagate exceptions
        }
        else
        {
            throw new TimeoutException("The operation has timed out.");
        }
    }
}
