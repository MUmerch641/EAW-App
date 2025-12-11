using EatWork.Mobile.Contants;
using System;

namespace EatWork.Mobile.Models
{
    public class LoanRequestModel
    {
        public LoanRequestModel()
        {
            DateRequest = DateTime.Now;
            RequestedAmount = 0;
            StatusId = RequestStatusValue.Submitted;
            BranchId = 0;
            DepartmentId = 0;
            PositionId = 0;
            CoMaker1 = string.Empty;
            CoMaker2 = string.Empty;
            ActualLoanAmount = 0;
            AddOnInterestAmount = 0;
            LoanAmount = 0;
            PenaltyAmount = 0;
            PrincipalAmount = 0;
            RequestedAmount = 0;
            TotalAmountDue = 0;
            ApprovedId = 0;
            TotalAmortization = 0;
            OutstandingLoan = 0;
            OutstandingBal = 0;
            IssuanceMethodId = 0;
            DateOfApproval = Constants.NullDate;
            ResumeDate = Constants.NullDate;
            VoucherDate = Constants.NullDate;
            InterestCalculationId = 0;
            AmountPaid = 0;
            NumberOfPayPeriod = 0;
            PaymentFrequencyId = 0;
            Amortization = 0;
            FirstPaymentDate = Constants.NullDate;
            SourceId = (short)SourceEnum.Mobile;
            MinimumTakeHome = 0;
            PredictedTakeHome = 0;
            PreviousTakeHome = 0;
        }

        public long LoanRequestId { get; set; }
        public long? LoanTypeSetupId { get; set; }
        public long? ProfileId { get; set; }
        public long? BranchId { get; set; }
        public long? DepartmentId { get; set; }
        public long? PositionId { get; set; }
        public string CoMaker1 { get; set; }
        public string CoMaker2 { get; set; }
        public string LoanRequestNumber { get; set; }
        public DateTime? DateRequest { get; set; }
        public long? ApprovedId { get; set; }
        public DateTime? DateOfApproval { get; set; }
        public decimal? RequestedAmount { get; set; }
        public decimal? PrincipalAmount { get; set; }
        public decimal? TotalAmortization { get; set; }
        public decimal? OutstandingLoan { get; set; }
        public long? PaymentStatusId { get; set; }
        public DateTime? ResumeDate { get; set; }
        public long? IssuanceMethodId { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string ChargeSlipNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string Remarks { get; set; }
        public long? InterestCalculationId { get; set; }
        public decimal? LoanAmount { get; set; }
        public decimal? AdvanceInterestPercent { get; set; }
        public decimal? AdvanceInterestAmount { get; set; }
        public decimal? AddOnInterestPercent { get; set; }
        public decimal? AddOnInterestAmount { get; set; }
        public decimal? ActualLoanAmount { get; set; }
        public decimal? TotalAmountDue { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? PenaltyPercent { get; set; }
        public decimal PenaltyAmount { get; set; }
        public decimal? Balance { get; set; }
        public short? NumberOfPayPeriod { get; set; }
        public decimal? Amortization { get; set; }
        public long? PaymentFrequencyId { get; set; }
        public DateTime? FirstPaymentDate { get; set; }
        public string Purpose { get; set; }
        public decimal? OutstandingBal { get; set; }
        public decimal? AmortizationAmt { get; set; }
        public decimal? PreviousTakeHome { get; set; }
        public decimal? PredictedTakeHome { get; set; }
        public decimal? MinimumTakeHome { get; set; }
        public string CapacityRemarks { get; set; }
        public long? StatusId { get; set; }
        public short? SourceId { get; set; }
    }
}