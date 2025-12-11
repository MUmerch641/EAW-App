using System.Collections.Generic;

namespace EatWork.Mobile.Models
{
    public class ClockworkConfigurationModel
    {
        public string LunchInLabel { get; set; }
        public string LunchOutLabel { get; set; }
        public string LunchInColor { get; set; }
        public string LunchOutColor { get; set; }
        public bool ShowBreakInOut2 { get; set; }
        public string BreakInLabel2 { get; set; }
        public string BreakOutLabel2 { get; set; }
        public string BreakInColor2 { get; set; }
        public bool ShowLunchInOut { get; set; }
        public string BreakOutColor2 { get; set; }
        public bool DisableButtonUponTimeIn { get; set; }
        public bool EnableTimeIn { get; set; }
        public bool DisableButtonUponBreak1 { get; set; }
        public bool DisableButtonUponLunch { get; set; }
        public bool DisableButtonUponBreak2 { get; set; }
        public decimal DisableBreak2Mins { get; set; }
        public bool DisableButtonCustomLogs { get; set; }

        //public List<ClockworkOtherTypeModel> OtherTypeList { get; set; }
        public bool ShowEnableOut { get; set; }

        public string EmployeeIds { get; set; }
        public bool CheckEmployeeLocationAssignment { get; set; }
        public List<string> IPWhiteList { get; set; }
        public long ClockworkConfigId { get; set; }
        public bool ShowBreakInOut { get; set; }
        public string TimeInColor { get; set; }
        public string TimeOutColor { get; set; }
        public string BreakInColor { get; set; }
        public string BreakOutColor { get; set; }
        public short MessageDuration { get; set; }
        public bool ClearEmployeeNo { get; set; }
        public short AskLateReason { get; set; }
        public short AskUndertimeReason { get; set; }
        public bool AllowImageCapture { get; set; }
        public bool AllowLocationCapture { get; set; }
        public bool BaseTimeEntryOnLocation { get; set; }
        public short MobileImageSize { get; set; }
        public short DesktopImageSize { get; set; }
        public string TimeInLabel { get; set; }
        public string TimeOutLabel { get; set; }
        public string BreakInLabel { get; set; }
        public string BreakOutLabel { get; set; }

        //public LeaveCompanyConfiguration LeaveCompanyConfiguration { get; set; }
        public bool EnableNewCWForm { get; set; }
    }
}