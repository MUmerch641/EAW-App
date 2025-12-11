using System;
using System.Collections.Generic;
using System.Text;

namespace EatWork.Mobile.Models
{
    public class MyExpenseReportsList
    {
        public long ExpenseReportId { get; set; }
        public long? ProfileId { get; set; }
        public string ReportNo { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public decimal Amount { get; set; }
        public long? StatusId { get; set; }
        public string Status { get; set; }
        public int TotalCount { get; set; }
        public string ExpenseDateDisplay { get; set; }
        public string AmountDisplay { get; set; }
    }
}
