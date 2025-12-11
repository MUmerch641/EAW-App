using System;

namespace EatWork.Mobile.Models.Expense
{
    public class AppExpenseReportDetail
    {
        public long AppExpenseReportDetailId { get; set; }
        public long? ProfileId { get; set; }
        public long? ExpenseSetupId { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string ORNo { get; set; }
        public long? VendorId { get; set; }
        public decimal? Amount { get; set; }
        public string Notes { get; set; }
        public string Attachment { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileUpload { get; set; }
        public long? CreateId { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? LastUpdateId { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string VendorName { get; set; }
        public string ExpenseType { get; set; }
        public string Icon { get; set; }
        public string AmountDisplay { get; set; }
        public string ExpenseDateDisplay { get; set; }
    }
}