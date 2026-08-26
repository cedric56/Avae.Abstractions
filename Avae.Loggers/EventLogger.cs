using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace Avae.Loggers;

[SupportedOSPlatform("windows")]
public class EventLogger : ILogger
{
    private static readonly object _lock = new();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null!;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
    {
        if (formatter != null)
        {
            lock (_lock)
            {
                var n = Environment.NewLine;
                string exc = "";
                if (exception != null) exc = n + exception.GetType() + ": " + exception.Message + n + exception.StackTrace + n;
                using var eventLog = new EventLog("Application");
                eventLog.Source = "Application";

                var type = EventLogEntryType.Information;
                switch (logLevel)
                {
                    case LogLevel.Debug:
                        type = EventLogEntryType.Information;
                        break;
                    case LogLevel.Information:
                        type = EventLogEntryType.Information;
                        break;
                    case LogLevel.Warning:
                        type = EventLogEntryType.Warning;
                        break;
                    case LogLevel.Error:
                        type = EventLogEntryType.Error;
                        break;
                    case LogLevel.Critical:
                        type = EventLogEntryType.Error;
                        break;
                    case LogLevel.Trace:
                        type = EventLogEntryType.Information;
                        break;
                    case LogLevel.None:
                        break;
                }

                eventLog.WriteEntry(logLevel.ToString() + ": " + DateTime.Now.ToString() + " " + formatter(state, exception) + n + exc, type);
            }
        }
    }
}

[SupportedOSPlatform("windows")]
public class EventLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new EventLogger();
    }

    public void Dispose()
    {
    }
}
