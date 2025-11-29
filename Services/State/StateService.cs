using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MauiHybridApp.Services.State;

/// <summary>
/// Application state management service
/// </summary>
public class StateService : IStateService
{
    private readonly ILogger<StateService> _logger;
    private readonly ConcurrentDictionary<string, object> _state;

    public StateService(ILogger<StateService> logger)
    {
        _logger = logger;
        _state = new ConcurrentDictionary<string, object>();
    }

    public event EventHandler<StateChangedEventArgs>? StateChanged;

    public T? GetState<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            _logger.LogWarning("Attempted to get state with null or empty key");
            return default;
        }

        try
        {
            if (_state.TryGetValue(key, out var value))
            {
                if (value is T directValue)
                {
                    return directValue;
                }

                // Try to deserialize if it's a JSON string
                if (value is string jsonString)
                {
                    return JsonSerializer.Deserialize<T>(jsonString);
                }

                // Try to convert the value
                return (T)Convert.ChangeType(value, typeof(T));
            }

            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting state for key: {Key}", key);
            return default;
        }
    }

    public void SetState<T>(string key, T value)
    {
        if (string.IsNullOrEmpty(key))
        {
            _logger.LogWarning("Attempted to set state with null or empty key");
            return;
        }

        try
        {
            var oldValue = _state.TryGetValue(key, out var existing) ? existing : null;
            
            // Store complex objects as JSON strings for better serialization
            object storeValue = value is string or int or long or double or float or bool or DateTime 
                ? value! 
                : JsonSerializer.Serialize(value);

            _state.AddOrUpdate(key, storeValue, (k, v) => storeValue);

            // Fire state changed event
            StateChanged?.Invoke(this, new StateChangedEventArgs
            {
                Key = key,
                OldValue = oldValue,
                NewValue = value
            });

            _logger.LogDebug("State updated for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting state for key: {Key}", key);
        }
    }

    public void RemoveState(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            _logger.LogWarning("Attempted to remove state with null or empty key");
            return;
        }

        try
        {
            if (_state.TryRemove(key, out var oldValue))
            {
                StateChanged?.Invoke(this, new StateChangedEventArgs
                {
                    Key = key,
                    OldValue = oldValue,
                    NewValue = null
                });

                _logger.LogDebug("State removed for key: {Key}", key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing state for key: {Key}", key);
        }
    }

    public void ClearState()
    {
        try
        {
            var keys = _state.Keys.ToList();
            _state.Clear();

            foreach (var key in keys)
            {
                StateChanged?.Invoke(this, new StateChangedEventArgs
                {
                    Key = key,
                    OldValue = null,
                    NewValue = null
                });
            }

            _logger.LogInformation("All state cleared");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing state");
        }
    }

    public bool HasState(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        return _state.ContainsKey(key);
    }
}
