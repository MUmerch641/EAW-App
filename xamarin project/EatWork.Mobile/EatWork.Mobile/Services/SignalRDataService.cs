using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using Microsoft.AspNetCore.SignalR.Client;
using Plugin.LocalNotification;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EatWork.Mobile.Services
{
    public class SignalRDataService : ISignalRDataService
    {
        private HubConnection _connection;

        public SignalRDataService()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(ApiConstants.SignalRServiceUrl) 
                .WithAutomaticReconnect()
                .Build();

            // Event when receiving a message from SignalR
            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ShowLocalNotification(user, message);
                });
            });
        }

        public void ShowLocalNotification(string title, string message)
        {
            try
            {
                var notification = new NotificationRequest
                {
                    Title = title,
                    Description = message,
                    NotificationId = new Random().Next(1000),
                    ReturningData = "Message received",
                };

                NotificationCenter.Current.Show(notification);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
            }
        }

        public async Task StartConnectionAsync()
        {
            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
            }
        }

        public async Task StopConnectionAsync()
        {
            try
            {
                await _connection.StopAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
            }
        }
    }
}