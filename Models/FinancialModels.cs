using System;

namespace MauiHybridApp.Models
{
    public class CashAdvanceModel
    {
        public long CashAdvanceId { get; set; }
        public long? ProfileId { get; set; }
        public string RequestNo { get; set; }
        public DateTime? RequestedDate { get; set; }
        public DateTime? DateNeeded { get; set; }
        public decimal? Amount { get; set; }
        public string Reason { get; set; }
        public long? CostCenterId { get; set; }
        public string ChargeCode { get; set; }
        public long? StatusId { get; set; }
        public string Status { get; set; }
        public DateTime? DateIssued { get; set; }
        
        // Display properties
        public string AmountDisplay => Amount.HasValue ? $"₱{Amount.Value:N2}" : "₱0.00";
        public string DateRequestedDisplay => RequestedDate.HasValue ? RequestedDate.Value.ToString("MMM dd, yyyy") : "";
    }

    public class LoanRequestModel
    {
        public long LoanRequestId { get; set; }
        public long? ProfileId { get; set; }
        public long? LoanTypeSetupId { get; set; }
        public string LoanType { get; set; } // Display name
        public string LoanRequestNumber { get; set; }
        public DateTime? DateRequest { get; set; }
        public decimal? RequestedAmount { get; set; }
        public decimal? LoanAmount { get; set; }
        public decimal? PrincipalAmount { get; set; }
        public decimal? TotalAmountDue { get; set; }
        public decimal? Balance { get; set; }
        public string Purpose { get; set; }
        public long? StatusId { get; set; }
        public string Status { get; set; }
        
        // Display properties
        public string AmountDisplay => RequestedAmount.HasValue ? $"₱{RequestedAmount.Value:N2}" : "₱0.00";
        public string DateRequestDisplay => DateRequest.HasValue ? DateRequest.Value.ToString("MMM dd, yyyy") : "";
    }

    public class LoanTypeModel
    {
        public long LoanTypeSetupId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class CostCenterModel
    {
        public long CostCenterId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class FinancialListResponseWrapper<T>
    {
        public System.Collections.Generic.List<T> ListData { get; set; }
        public bool IsSuccess { get; set; }
        public int TotalListCount { get; set; }
    }
}
