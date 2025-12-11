using EatWork.Mobile.Contants;
using System;

namespace EatWork.Mobile.Models
{
    public class MyExpenseListModel
    {
        public MyExpenseListModel()
        {
            ExpenseReportId = 0;
            StatusId = RequestStatusValue.Draft;
        }

        public long ExpenseReportId { get; set; }
        public DateTime ExpenseDate { get; set; }
        public DateTime ExpenseDateDisplay { get; set; }
        public string PreparedBy { get; set; }
        public string ReportNo { get; set; }
        public long? StatusId { get; set; }
        public long? ProfileId { get; set; }
        public string Status { get; set; }
        public decimal? Amount { get; set; }
    }
}