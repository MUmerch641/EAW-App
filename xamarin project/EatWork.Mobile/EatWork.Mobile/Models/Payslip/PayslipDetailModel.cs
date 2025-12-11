using System;

namespace EatWork.Mobile.Models.Payslip
{
    public class PayslipDetailModel
    {
        public long ProfileId { get; set; }
        public long PaysheetHeaderId { get; set; }
        public long PaysheetHeaderDetailId { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public string ReferenceNumber { get; set; }
        public string PayrollType { get; set; }
        public decimal GrossPay { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal NetPay { get; set; }
        public decimal BasicPay { get; set; }
        public decimal BasicRate { get; set; }
        public decimal SSS { get; set; }
        public decimal PhilHealth { get; set; }
        public decimal PAGIBIG { get; set; }
        public decimal WHT { get; set; }
        public decimal Loan { get; set; }
        public decimal OtherDeduction { get; set; } /*DEDUCTION PLUS OTHER DEDUCTION*/

        public string IssuedDateDisplay { get; set; }
        public string PeriodDate { get; set; }
        public string GrossPayDisplay { get; set; }
        public string TotalDeductionDisplay { get; set; }
        public string NetPayDisplay { get; set; }
        public string BasicPayDisplay { get; set; }
        public string BasicRateDisplay { get; set; }
        public string SSSDisplay { get; set; }
        public string PhilHealthDisplay { get; set; }
        public string PAGIBIGDisplay { get; set; }
        public string WHTDisplay { get; set; }
        public string LoanDisplay { get; set; }
        public string OtherDeductionDisplay { get; set; }
    }

    public class PaysheetDetailDto
    {
        public long PaySheetHeaderDetailId { get; set; }
        public string Description { get; set; }
        public decimal Hours { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public string HoursDisplay { get; set; }
        public string AmountDisplay { get; set; }
    }
}