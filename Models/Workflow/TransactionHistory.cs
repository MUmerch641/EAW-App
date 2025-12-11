using System;

namespace MauiHybridApp.Models
{
    public class TransactionHistory
    {
        public string RequestersName { get; set; } = string.Empty;
        public string ApproversName { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public string StageDescription { get; set; } = string.Empty;
        public string NextApprovers { get; set; } = string.Empty;
        public long ActionTypeId { get; set; }
        public string ActionPastTense { get; set; } = string.Empty;
        public string MessageTemplate { get; set; } = string.Empty;
        public string DetailedMessage { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public long HistoryTypeId { get; set; }
        public DateTime LogDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
