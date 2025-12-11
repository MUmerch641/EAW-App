using System;

namespace EatWork.Mobile.Models
{
    public class MyRequestListModel
    {
        public MyRequestListModel()
        {
            TransactionId = 0;
        }

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
        public string ItemName { get; set; }

        //==custom fields
        public bool DisplayItemName { get; set; }

        public bool IsDocumentRequest { get; set; }

        public bool IsChangeRestday { get; set; }
        public bool IsLoanRequest { get; set; }
        public bool IsScheduleRequest { get; set; }
        public bool IsTimeEntryLogRequest { get; set; }
        public bool IsLeaveReqeust { get; set; }
        public bool IsTravelRequest { get; set; }

        public decimal RequestedHoursNumber { get; set; }
        public string RequestedHoursSuffixDisplay { get; set; }
        public DateTime? SelectedDate { get; set; }
    }
}