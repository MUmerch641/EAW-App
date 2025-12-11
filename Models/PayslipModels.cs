namespace MauiHybridApp.Models;

// 1. List Item (Card ke liye)
public class MyPayslipListModel
{
    public long PaysheetHeaderId { get; set; }
    public long PaysheetHeaderDetailId { get; set; }
    public long ProfileId { get; set; }
    public string PayrollType { get; set; }
    public DateTime IssuedDate { get; set; }
    public decimal BasicPay { get; set; }
    public decimal NetPay { get; set; }
}

// 2. List Response Wrapper (API sends listData)
public class PayslipListResponse
{
    public List<MyPayslipListModel> ListData { get; set; } = new();
    public bool IsSuccess { get; set; }
}

// 3. Detail Model (Breakdown ke liye)
public class PayslipDetailModel
{
    // Header Info
    public string PayrollType { get; set; }
    public string ReferenceNumber { get; set; }
    public DateTime? IssuedDate { get; set; }
    public DateTime? PeriodStartDate { get; set; }
    public DateTime? PeriodEndDate { get; set; }
    
    // Totals
    public decimal GrossPay { get; set; }
    public decimal TotalDeduction { get; set; }
    public decimal NetPay { get; set; }
    
    // Specifics
    public decimal BasicPay { get; set; }
    public decimal SSS { get; set; }
    public decimal PhilHealth { get; set; }
    public decimal PAGIBIG { get; set; }
    public decimal WHT { get; set; }
    public decimal Loan { get; set; }
    
    // LISTS (In Xamarin these were in separate holders, we are combining them here)
    public List<PaysheetDetailDto> Earnings { get; set; } = new();
    public List<PaysheetDetailDto> Deductions { get; set; } = new();
    public List<PaysheetDetailDto> OvertimeDetails { get; set; } = new();
    public List<PaysheetDetailDto> LoanPayments { get; set; } = new();
    
    // NEW Properties
    public List<PaysheetDetailDto> Allowances { get; set; } = new();
    public List<PaysheetDetailDto> YTDs { get; set; } = new();
}

// 4. Detail Item (Row Item)
public class PaysheetDetailDto
{
    public string Description { get; set; }
    public decimal Hours { get; set; }
    public decimal Amount { get; set; }
    public string Remarks { get; set; }
}