using Microsoft.Extensions.Logging;

namespace DataAnalysis.Infrastructure.Logging;
public sealed class MaskingLogger : ILogger
{
    private readonly ILogger _innerLogger;

    public MaskingLogger(ILogger innerLogger)
    {
        _innerLogger = innerLogger;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _innerLogger.BeginScope(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _innerLogger.IsEnabled(logLevel);
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var originalMessage = formatter(state, exception);

        var maskedMessage = PiiMasker.MaskAllEmails(originalMessage);

        _innerLogger.Log(
            logLevel,
            eventId,
            maskedMessage,
            exception,
            (s, e) => s?.ToString() ?? string.Empty);
    }
}

public sealed class MaskingLogger<T> : ILogger<T>
{
    private readonly MaskingLogger _logger;

    public MaskingLogger(ILoggerFactory factory)
    {
        var innerLogger = factory.CreateLogger<T>();
        _logger = new MaskingLogger(innerLogger);
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _logger.BeginScope(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _logger.IsEnabled(logLevel);
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        _logger.Log(logLevel, eventId, state, exception, formatter);
    }
}