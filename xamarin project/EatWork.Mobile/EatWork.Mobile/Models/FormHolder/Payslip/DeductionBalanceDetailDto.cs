using DTO = EAW.API.DataContracts.Models;

namespace EatWork.Mobile.Models.FormHolder.Payslip
{
    public class DeductionBalanceDetailDto : DTO.DeductionBalanceDetailDto
    {
        public string OriginalAmountDisplay { get; set; }

        public string PreviousBalanceDisplay { get; set; }

        public string PaymentDisplay { get; set; }

        public string RemainingBalanceDisplay { get; set; }
    }
}