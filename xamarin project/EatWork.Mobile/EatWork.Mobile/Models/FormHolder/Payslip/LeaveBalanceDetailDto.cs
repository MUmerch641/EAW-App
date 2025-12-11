using DTO = EAW.API.DataContracts.Models;

namespace EatWork.Mobile.Models.FormHolder.Payslip
{
    public class LeaveBalanceDetailDto : DTO.LeaveBalanceDetailDto
    {
        public string EarnedHoursDisplay { get; set; }
        public string UsedHoursDisplay { get; set; }
        public string CurrentBalanceDisplay { get; set; }
    }
}