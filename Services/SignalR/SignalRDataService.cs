using MauiHybridApp.Services.SignalR;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.SignalR
{
    public class SignalRDataService : ISignalRDataService
    {
        private HubConnection? _hubConnection;
        public event Action<string>? OnNotificationReceived;
        public event EventHandler<bool>? ConnectionStateChanged;

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public async Task StartConnectionAsync()
        {
            try
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl("https://api.everythingatwork.com/notificationHub")
                    .WithAutomaticReconnect()
                    .Build();

                _hubConnection.On<string>("ReceiveNotification", (message) =>
                {
                    OnNotificationReceived?.Invoke(message);
                });

                _hubConnection.Closed += (error) => 
                {
                    ConnectionStateChanged?.Invoke(this, false);
                    return Task.CompletedTask;
                };
                
                _hubConnection.Reconnected += (connectionId) =>
                {
                    ConnectionStateChanged?.Invoke(this, true);
                    return Task.CompletedTask;
                };

                await _hubConnection.StartAsync();
                ConnectionStateChanged?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SignalR connection error: {ex.Message}");
                ConnectionStateChanged?.Invoke(this, false);
            }
        }

        public async Task StopConnectionAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                ConnectionStateChanged?.Invoke(this, false);
            }
        }

        public async Task SendMessageAsync(string method, object message)
        {
            if (IsConnected && _hubConnection != null)
            {
                await _hubConnection.InvokeAsync(method, message);
            }
        }

        public void RegisterHandler<T>(string methodName, Action<T> handler)
        {
            if (_hubConnection != null)
            {
                _hubConnection.On<T>(methodName, handler);
            }
        }
    }
}
