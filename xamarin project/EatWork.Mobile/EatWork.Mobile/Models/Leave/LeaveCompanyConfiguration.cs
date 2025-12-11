using System;
using System.Collections.Generic;
using System.Text;

namespace EatWork.Mobile.Models.Leave
{
    public class LeaveCompanyConfiguration
    {
        public bool automateHalfDayLeave_ { get; set; }
        public string changeApplyToLate_ { get; set; }
        public string changeApplyToUndertime_ { get; set; }
        public string LeaveHrsLabelShort { get; set; }
        public string LeaveHrsLabelLong { get; set; }
        public short DisplayInDays { get; set; }
        public int NoOfHours { get; set; }
        public bool CombineLimitForLeaveConversion { get; set; }
        public short LeaveConflictChecking { get; set; }
        public bool AutomateHalfDayLeave { get; set; }
        public string ChangeApplyToLate { get; set; }
        public string ChangeApplyToUndertime { get; set; }
    }
}
