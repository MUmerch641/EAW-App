using System;

namespace MauiHybridApp.Models
{
    public class MyRequestListModel
    {
        public MyRequestListModel()
        {
            TransactionId = 0;
            Status = string.Empty;
            Details = string.Empty;
            TransactionType = string.Empty;
        }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeNo { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string TransactionType { get; set; }
        public long TransactionTypeId { get; set; }
        public long TransactionId { get; set; }
        public DateTime DateFiled { get; set; }
        public DateTime DateFiledDisplay { get; set; }
        public string Details { get; set; }
        public string Status { get; set; }
        public string RequestedDate { get; set; } = string.Empty;
        public string RequestedTime { get; set; } = string.Empty;
        public string RequestedHours { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;

        // Custom fields
        public bool DisplayItemName { get; set; }
        public bool IsDocumentRequest { get; set; }
        public bool IsChangeRestday { get; set; }
        public bool IsLoanRequest { get; set; }
        public bool IsScheduleRequest { get; set; }
        public bool IsTimeEntryLogRequest { get; set; }
        public bool IsLeaveReqeust { get; set; }
        public bool IsTravelRequest { get; set; }

        public decimal RequestedHoursNumber { get; set; }
        public string RequestedHoursSuffixDisplay { get; set; } = string.Empty;
        public DateTime? SelectedDate { get; set; }
    }
}
