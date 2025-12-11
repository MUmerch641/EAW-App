using System;

namespace EatWork.Mobile.Models.Accountability
{
    public class CashAdvanceRequestList
    {
        public long CashAdvanceId { get; set; }
        public long? ProfileId { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime DateIssued { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public long? StatusId { get; set; }
        public int TotalCount { get; set; }
        public string AmountDisplay { get; set; }
        public string DateRequestedDisplay { get; set; }
        public string DateIssuedDisplay { get; set; }
    }
}