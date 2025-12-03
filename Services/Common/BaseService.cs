using System.Diagnostics;

namespace MauiHybridApp.Services.Common;

/// <summary>
/// Base service class providing common functionality for all services
/// </summary>
public abstract class BaseService
{
    protected readonly IGenericRepository _repository;
    private readonly string _serviceName;

    protected BaseService(IGenericRepository repository, string serviceName)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _serviceName = serviceName;
    }

    /// <summary>
    /// Executes an async operation with standardized error handling and logging
    /// </summary>
    protected async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, string operationName, T defaultValue = default!)
    {
        try
        {
            LogOperation($"{operationName} - Started");
            var result = await operation();
            LogOperation($"{operationName} - Completed successfully");
            return result;
        }
        catch (HttpRequestException httpEx)
        {
            LogError($"{operationName} - HTTP Error", httpEx);
            throw new ServiceException($"Network error in {operationName}", httpEx);
        }
        catch (TaskCanceledException timeoutEx)
        {
            LogError($"{operationName} - Timeout", timeoutEx);
            throw new ServiceException($"Request timed out in {operationName}", timeoutEx);
        }
        catch (Exception ex)
        {
            LogError($"{operationName} - Unexpected Error", ex);
            return defaultValue;
        }
    }

    /// <summary>
    /// Executes an async operation that returns a SaveResult with error handling
    /// </summary>
    protected async Task<SaveResult> ExecuteSaveAsync(Func<Task<SaveResult>> operation, string operationName)
    {
        try
        {
            LogOperation($"{operationName} - Started");
            var result = await operation();
            
            if (result.Success)
            {
                LogOperation($"{operationName} - Completed successfully");
            }
            else
            {
                LogWarning($"{operationName} - Failed: {result.ErrorMessage}");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            LogError($"{operationName} - Exception", ex);
            return new SaveResult 
            { 
                Success = false, 
                ErrorMessage = $"An error occurred: {ex.Message}" 
            };
        }
    }

    /// <summary>
    /// Validates required field is not null or empty
    /// </summary>
    protected bool ValidateRequired(string? value, string fieldName, out string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errorMessage = $"{fieldName} is required";
            LogWarning($"Validation failed: {errorMessage}");
            return false;
        }
        errorMessage = null;
        return true;
    }

    /// <summary>
    /// Validates date range
    /// </summary>
    protected bool ValidateDateRange(DateTime? startDate, DateTime? endDate, out string? errorMessage)
    {
        if (!startDate.HasValue || !endDate.HasValue)
        {
            errorMessage = "Start date and end date are required";
            LogWarning($"Validation failed: {errorMessage}");
            return false;
        }

        if (endDate < startDate)
        {
            errorMessage = "End date cannot be before start date";
            LogWarning($"Validation failed: {errorMessage}");
            return false;
        }

        errorMessage = null;
        return true;
    }

    /// <summary>
    /// Logs informational operation message
    /// </summary>
    protected void LogOperation(string message)
    {
        Debug.WriteLine($"[{_serviceName}] {message}");
        Console.WriteLine($"[{_serviceName}] {message}");
    }

    /// <summary>
    /// Logs warning message
    /// </summary>
    protected void LogWarning(string message)
    {
        Debug.WriteLine($"[{_serviceName}] WARNING: {message}");
        Console.WriteLine($"[{_serviceName}] WARNING: {message}");
    }

    /// <summary>
    /// Logs error with exception details
    /// </summary>
    protected void LogError(string message, Exception ex)
    {
        Debug.WriteLine($"[{_serviceName}] ERROR: {message}");
        Debug.WriteLine($"[{_serviceName}] Exception: {ex.Message}");
        Debug.WriteLine($"[{_serviceName}] Stack Trace: {ex.StackTrace}");
        
        Console.WriteLine($"[{_serviceName}] ERROR: {message}");
        Console.WriteLine($"[{_serviceName}] Exception: {ex.Message}");
    }

    /// <summary>
    /// Gets profile ID from secure storage
    /// </summary>
    protected async Task<long?> GetProfileIdAsync()
    {
        try
        {
            var profileIdString = await SecureStorage.GetAsync("profile_id");
            if (long.TryParse(profileIdString, out long profileId))
            {
                return profileId;
            }
            LogWarning("Profile ID not found in secure storage");
            return null;
        }
        catch (Exception ex)
        {
            LogError("Error retrieving profile ID", ex);
            return null;
        }
    }

    /// <summary>
    /// Normalizes date to midnight (removes time component)
    /// </summary>
    protected DateTime NormalizeDate(DateTime date)
    {
        return date.Date;
    }
}

/// <summary>
/// Custom exception for service layer errors
/// </summary>
public class ServiceException : Exception
{
    public ServiceException(string message) : base(message) { }
    public ServiceException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Result model for save operations
/// </summary>
public class SaveResult
{
    public bool Success { get; set; }
    public long Id { get; set; }
    public string? ErrorMessage { get; set; }
}
