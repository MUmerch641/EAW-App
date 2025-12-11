using System;

namespace EatWork.Mobile.Models
{
    public class NotificationModel
    {
        public DateTime ActionDateTime { get; set; }
        public string Image { get; set; }
        public int MenuType { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public int NotificationState { get; set; }
        public int Total { get; set; }
        public long TransactionId { get; set; }
        public string TransactionName { get; set; }
        public long TransactionType { get; set; }
        public string Url { get; set; }
        public long WorkflowNotificationTaskId { get; set; }

        public bool IsRead { get; set; }
    }
}