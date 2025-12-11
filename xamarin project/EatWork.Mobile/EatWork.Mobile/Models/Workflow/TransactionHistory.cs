using System;

namespace EatWork.Mobile.Models
{
    public class TransactionHistory
    {
        public string RequestersName { get; set; }
        public string ApproversName { get; set; }
        public string TransactionType { get; set; }
        public string StageDescription { get; set; }
        public string NextApprovers { get; set; }
        public long ActionTypeId { get; set; }
        public string ActionPastTense { get; set; }
        public string MessageTemplate { get; set; }
        public string DetailedMessage { get; set; }
        public string Remarks { get; set; }
        public long HistoryTypeId { get; set; }
        public DateTime LogDate { get; set; }
        public string CreatedBy { get; set; }
    }
}