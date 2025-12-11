using System;

namespace EatWork.Mobile.Models.Payslip
{
    public class MyPayslipListModel
    {
        public MyPayslipListModel()
        {
            PayrollType = string.Empty;
            BasicPay = 0;
            NetPay = 0;
            PaysheetHeaderId = 0;
            PaysheetHeaderDetailId = 0;
            ProfileId = 0;
        }

        public string PayrollType { get; set; }
        public DateTime IssuedDate { get; set; }
        public decimal BasicPay { get; set; }
        public decimal NetPay { get; set; }
        public long PaysheetHeaderId { get; set; }
        public long PaysheetHeaderDetailId { get; set; }
        public long ProfileId { get; set; }
        public string AmountDisplay { get; set; }
        public string IssuedDateDisplay { get; set; }
    }
}