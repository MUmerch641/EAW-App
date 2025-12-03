using System.Diagnostics;

namespace MauiHybridApp.Services.Common;

/// <summary>
/// Interface for application logging
/// </summary>
public interface ILoggingService
{
    void LogInfo(string message, string? category = null);
    void LogWarning(string message, string? category = null);
    void LogError(string message, Exception? exception = null, string? category = null);
    void LogDebug(string message, string? category = null);
}

/// <summary>
/// Implementation of logging service with console and debug output
/// </summary>
public class LoggingService : ILoggingService
{
    private readonly bool _isDebugEnabled;

    public LoggingService(bool isDebugEnabled = true)
    {
        _isDebugEnabled = isDebugEnabled;
    }

    public void LogInfo(string message, string? category = null)
    {
        var logMessage = FormatMessage("INFO", message, category);
        Console.WriteLine(logMessage);
        Debug.WriteLine(logMessage);
    }

    public void LogWarning(string message, string? category = null)
    {
        var logMessage = FormatMessage("WARN", message, category);
        Console.WriteLine(logMessage);
        Debug.WriteLine(logMessage);
    }

    public void LogError(string message, Exception? exception = null, string? category = null)
    {
        var logMessage = FormatMessage("ERROR", message, category);
        Console.WriteLine(logMessage);
        Debug.WriteLine(logMessage);

        if (exception != null)
        {
            Console.WriteLine($"Exception: {exception.Message}");
            Console.WriteLine($"Stack Trace: {exception.StackTrace}");
            Debug.WriteLine($"Exception: {exception.Message}");
            Debug.WriteLine($"Stack Trace: {exception.StackTrace}");
        }
    }

    public void LogDebug(string message, string? category = null)
    {
        if (!_isDebugEnabled) return;

        var logMessage = FormatMessage("DEBUG", message, category);
        Debug.WriteLine(logMessage);
    }

    private string FormatMessage(string level, string message, string? category)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var categoryPart = string.IsNullOrEmpty(category) ? "" : $" [{category}]";
        return $"[{timestamp}] [{level}]{categoryPart} {message}";
    }
}
