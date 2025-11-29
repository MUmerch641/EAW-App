namespace MauiHybridApp.Services.State;

/// <summary>
/// Interface for application state management
/// </summary>
public interface IStateService
{
    /// <summary>
    /// Get a state value by key
    /// </summary>
    T? GetState<T>(string key);

    /// <summary>
    /// Set a state value by key
    /// </summary>
    void SetState<T>(string key, T value);

    /// <summary>
    /// Remove a state value by key
    /// </summary>
    void RemoveState(string key);

    /// <summary>
    /// Clear all state values
    /// </summary>
    void ClearState();

    /// <summary>
    /// Check if a state key exists
    /// </summary>
    bool HasState(string key);

    /// <summary>
    /// Event fired when state changes
    /// </summary>
    event EventHandler<StateChangedEventArgs> StateChanged;
}

/// <summary>
/// Event arguments for state change events
/// </summary>
public class StateChangedEventArgs : EventArgs
{
    public string Key { get; set; } = string.Empty;
    public object? OldValue { get; set; }
    public object? NewValue { get; set; }
}
