namespace MauiHybridApp.Services.SignalR;

/// <summary>
/// Interface for SignalR real-time communication service
/// </summary>
public interface ISignalRDataService
{
    /// <summary>
    /// Start the SignalR connection
    /// </summary>
    Task StartConnectionAsync();

    /// <summary>
    /// Stop the SignalR connection
    /// </summary>
    Task StopConnectionAsync();

    /// <summary>
    /// Check if the connection is active
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Send a message to all connected clients
    /// </summary>
    Task SendMessageAsync(string method, object message);

    /// <summary>
    /// Register a handler for incoming messages
    /// </summary>
    void RegisterHandler<T>(string methodName, Action<T> handler);

    /// <summary>
    /// Event fired when connection state changes
    /// </summary>
    event EventHandler<bool> ConnectionStateChanged;
}
