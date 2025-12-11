using System;
using Xamarin.Forms;

namespace EatWork.Mobile.Models
{
    public class LeaveRequestDetailModel
    {
        public LeaveRequestDetailModel()
        {
        }

        public byte isChecked { get; set; }
        public string txtRemarks { get; set; }
        public string Status { get; set; }
        public long StatusId { get; set; }
        public string chkPlanned_String { get; set; }
        public short chkPlanned { get; set; }
        public decimal NoOfDaysDecimal { get; set; }
        public decimal NoOfWorkingHours { get; set; }
        public decimal txtNoOfHours { get; set; }
        public string DayOfWeek { get; set; }
        public string LeaveDate { get; set; }
        public DateTime dtpInclusiveEndDate { get; set; }
        public DateTime dtpInclusiveStartDate { get; set; }
        public long LeaveRequestId { get; set; }
        public long LeaveRequestHeaderId { get; set; }
        public string Disable { get; set; }
        public string Disable_Planned { get; set; }

        public string LeaveDateAndDayOfWeek { get; set; }
        public string SelectedLeaveType { get; set; }
        public string DisplayType { get; set; }
        public string RequestedHours { get; set; }
        public FontAttributes FontAttribute { get; set; }
    }
}