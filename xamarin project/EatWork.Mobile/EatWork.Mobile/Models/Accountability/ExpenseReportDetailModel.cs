using Plugin.FilePicker.Abstractions;
using System;

namespace EatWork.Mobile.Models
{
    public class ExpenseReportDetailModel
    {
        public ExpenseReportDetailModel()
        {
            Amount = 0;
            ORNo = string.Empty;
            VendorId = 0;
            Notes = string.Empty;
            /*FileData = new FileData();*/
            ExpenseDate = DateTime.UtcNow;
            FileName = string.Empty;
        }

        public long ExpenseReportDetailId { get; set; }
        public long? ExpenseReportId { get; set; }
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

        public string IconEquivalent { get; set; }
        public string ExpenseType { get; set; }
        public string IconColor { get; set; }
        public string VendorName { get; set; }
        public string ExpenseDateDisplay { get; set; }
        public string AmountDisplay { get; set; }
        public bool HasAttachment { get; set; }
    }
}