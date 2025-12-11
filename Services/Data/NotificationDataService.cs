using MauiHybridApp.Services.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class NotificationDataService : INotificationDataService
    {
        private readonly IGenericRepository _repository;

        // Mock data to match Xamarin implementation
        public NotificationDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<object>> GetNotificationsAsync()
        {
            await Task.Delay(500); // Simulate network delay
            
            var list = new List<object>();
            
            // Mock data matching Xamarin's WorkflowViewModel logic
            for (int i = 0; i < 5; i++)
            {
                list.Add(new MauiHybridApp.Models.Workflow.NotificationModel
                {
                    IsRead = i > 1, // First 2 unread for demo
                    Message = "Your Bank File has been Approved",
                    ActionDateTime = DateTime.Now.AddHours(-i * 2),
                    TransactionName = "Bank File",
                    Name = "System"
                });
            }
            
            return list;
        }

        public Task MarkAsReadAsync(long notificationId)
        {
            return Task.CompletedTask;
        }
    }
}
