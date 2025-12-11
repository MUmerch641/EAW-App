using System;

namespace EatWork.Mobile.Models
{
    public class MyScheduleListModel
    {
        public MyScheduleListModel()
        {
            HasSchedule = false;
            ActualTimeOut = string.Empty;
            ActualTimeIn = string.Empty;
            IsRestday = false;
            IsHoliday = false;
            HolidayName = string.Empty;
            LeaveCode = string.Empty;
            LeaveDescription = string.Empty;
            OTSchedule = string.Empty;
        }

        public long? ProfileId { get; set; }
        public string ScheduleType { get; set; }
        public DateTime? WorkDate { get; set; }
        public string WeekDayName { get; set; }
        public string WorkSchedule { get; set; }
        public DateTime? WSStartTime { get; set; }
        public DateTime? WSEndTime { get; set; }
        public decimal? WorkHours { get; set; }
        public string LunchSchedule { get; set; }
        public string PSOTDuration { get; set; }
        public string ASOTDuration { get; set; }
        public decimal? OTTotalHours { get; set; }
        public string TOEffect { get; set; }
        public decimal? TOHours { get; set; }
        public decimal? LunchHours { get; set; }
        public decimal? Break1Minutes { get; set; }
        public decimal? Break2Minutes { get; set; }
        public string ShiftCode { get; set; }
        public string ShiftDescription { get; set; }
        public string OBDuration { get; set; }
        public string OBReason { get; set; }
        public string TODuration { get; set; }
        public string TOReason { get; set; }
        public string UTDuration { get; set; }
        public string UTReason { get; set; }
        public string LeaveCode { get; set; }
        public string LeaveDescription { get; set; }
        public bool PartialDayLeave { get; set; }
        public string PartialApplyTo { get; set; }
        public bool IsRestday { get; set; }
        public bool IsHoliday { get; set; }
        public string HolidayName { get; set; }
        public string ActualTimeIn { get; set; }
        public string ActualTimeOut { get; set; }
        public int? TotalCount { get; set; }

        public string WorkDateDisplay { get; set; }
        public bool HasSchedule { get; set; }

        public string OTSchedule { get; set; }
    }
}