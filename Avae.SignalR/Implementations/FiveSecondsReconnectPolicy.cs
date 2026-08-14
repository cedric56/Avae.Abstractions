using Microsoft.AspNetCore.SignalR.Client;

namespace Avae.SignalR;

public class FiveSecondsReconnectPolicy : IRetryPolicy
{
    public TimeSpan? NextRetryDelay(RetryContext retryContext)
    {
        return TimeSpan.FromSeconds(5);
    }
}
