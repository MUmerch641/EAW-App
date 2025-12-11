using SQLite;

namespace EatWork.Mobile.Models
{
    [Table("ExpenseReportDetailList")]
    public class ExpenseReportDetailListDataModel : ExpenseReportDetailModel
    {
        [PrimaryKey, AutoIncrement]
        public long ID { get; set; }

        public string ExpenseDateString { get; set; }
        public string TINNo { get; set; }
        public string Address { get; set; }
        public string Icon { get; set; }
        public bool ShowDownload { get; set; }
        public bool IsSaved { get; set; }
    }
}