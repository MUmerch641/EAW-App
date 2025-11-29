using Microsoft.Extensions.Logging;

namespace MauiHybridApp.Services.SignalR;

/// <summary>
/// SignalR service implementation for real-time communication
/// </summary>
public class SignalRDataService : ISignalRDataService
{
    private readonly ILogger<SignalRDataService> _logger;
    private readonly Dictionary<string, List<Delegate>> _handlers;
    private bool _isConnected;

    public SignalRDataService(ILogger<SignalRDataService> logger)
    {
        _logger = logger;
        _handlers = new Dictionary<string, List<Delegate>>();
        _isConnected = false;
    }

    public bool IsConnected => _isConnected;

    public event EventHandler<bool>? ConnectionStateChanged;

    public async Task StartConnectionAsync()
    {
        try
        {
            _logger.LogInformation("Starting SignalR connection...");
            
            // TODO: Implement actual SignalR connection logic
            // For now, simulate a successful connection
            await Task.Delay(100);
            
            _isConnected = true;
            ConnectionStateChanged?.Invoke(this, true);
            
            _logger.LogInformation("SignalR connection started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start SignalR connection");
            _isConnected = false;
            ConnectionStateChanged?.Invoke(this, false);
            throw;
        }
    }

    public async Task StopConnectionAsync()
    {
        try
        {
            _logger.LogInformation("Stopping SignalR connection...");
            
            // TODO: Implement actual SignalR disconnection logic
            await Task.Delay(100);
            
            _isConnected = false;
            ConnectionStateChanged?.Invoke(this, false);
            
            _logger.LogInformation("SignalR connection stopped");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping SignalR connection");
            throw;
        }
    }

    public async Task SendMessageAsync(string method, object message)
    {
        if (!_isConnected)
        {
            _logger.LogWarning("Cannot send message - SignalR not connected");
            return;
        }

        try
        {
            _logger.LogDebug("Sending SignalR message: {Method}", method);
            
            // TODO: Implement actual message sending logic
            await Task.Delay(10);
            
            _logger.LogDebug("SignalR message sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SignalR message: {Method}", method);
            throw;
        }
    }

    public void RegisterHandler<T>(string methodName, Action<T> handler)
    {
        if (!_handlers.ContainsKey(methodName))
        {
            _handlers[methodName] = new List<Delegate>();
        }

        _handlers[methodName].Add(handler);
        _logger.LogDebug("Registered SignalR handler for method: {Method}", methodName);
    }

    // Simulate receiving a message (for testing purposes)
    protected virtual void OnMessageReceived<T>(string methodName, T message)
    {
        if (_handlers.TryGetValue(methodName, out var handlers))
        {
            foreach (var handler in handlers.OfType<Action<T>>())
            {
                try
                {
                    handler.Invoke(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in SignalR message handler for method: {Method}", methodName);
                }
            }
        }
    }
}
