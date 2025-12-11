using System;

namespace EatWork.Mobile.Models
{
    public class MyApprovalModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNo { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string TransactionType { get; set; }
        public long TransactionTypeId { get; set; }
        public long TransactionId { get; set; }
        public DateTime DateFiled { get; set; }
        public DateTime DateFiledDisplay { get; set; }
        public string Details { get; set; }
        public string Status { get; set; }
        public string RequestedDate { get; set; }
        public string RequestedTime { get; set; }
        public string RequestedHours { get; set; }
        public bool IsVisible { get; set; }
    }
}