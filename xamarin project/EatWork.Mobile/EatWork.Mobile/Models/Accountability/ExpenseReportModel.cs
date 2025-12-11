using EatWork.Mobile.Contants;
using System;

namespace EatWork.Mobile.Models
{
    public class ExpenseReportModel
    {
        public ExpenseReportModel()
        {
            StatusId = RequestStatusValue.Draft;
            AmountReimbursment = 0;
            AmountIssued = 0;
            Amount = 0;
            Balance = 0;
            ReportNo = string.Empty;
            AgreeDate = Constants.NullDate;
            SourceId = (short)SourceEnum.Mobile;
        }

        public long ExpenseReportId { get; set; }
        public string ReportNo { get; set; }
        public DateTime? ReportDate { get; set; }
        public long? ProfileId { get; set; }
        public decimal? AmountIssued { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Balance { get; set; }
        public decimal? AmountReimbursment { get; set; }
        public bool? SalaryDeduction { get; set; }
        public DateTime? AgreeDate { get; set; }
        public long? StatusId { get; set; }
        public short? SourceId { get; set; }
    }
}