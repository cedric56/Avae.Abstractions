using Microsoft.Extensions.Logging;

namespace Avae.Abstractions
{
    public class FileLogger : ILogger
    {
        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            private NullScope() { }

            public void Dispose()
            {
                // no-op
            }
        }

        private readonly string filePath;
        private static readonly object _lock = new();
        public FileLogger(string path)
        {
            filePath = path;
            CreateFile(path);
        }

        static void CreateFile(string filePath)
        {
            string fullFilePath = Path.Combine(filePath, DateTime.Now.ToString("yyyy-MM-dd") + "_log.txt");
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            if (!File.Exists(fullFilePath)) File.Create(fullFilePath).Close();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == LogLevel.Error;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if(!IsEnabled(logLevel)) 
                return;

            if (formatter != null)
            {
                lock (_lock)
                {
                    string fullFilePath = Path.Combine(filePath, DateTime.Now.ToString("yyyy-MM-dd") + "_log.txt");
                    var n = Environment.NewLine;
                    string exc = "";
                    if (exception != null) exc = n + exception.GetType() + ": " + exception.Message + n + exception.StackTrace + n;
                    File.AppendAllText(fullFilePath, logLevel.ToString() + ": " + DateTime.Now.ToString() + " " + formatter(state, exception) + n + exc);
                }
            }
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return NullScope.Instance;
        }
    }
    public class FileLoggerProvider(string path) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(path);
        }

        public void Dispose()
        {
        }
    }
}
