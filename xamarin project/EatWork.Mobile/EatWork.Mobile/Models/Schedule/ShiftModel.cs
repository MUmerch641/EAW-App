using System;

namespace EatWork.Mobile.Models
{
    public class ShiftModel
    {
        public long ShiftId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string DaysOfWeek { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public short? StartTimePreviousDay { get; set; }
        public short? EndTimeNextDay { get; set; }
        public DateTime? Break1StartTime { get; set; }
        public DateTime? Break1EndTime { get; set; }
        public decimal? Break1Duration { get; set; }
        public DateTime? Break2StartTime { get; set; }
        public DateTime? Break2EndTime { get; set; }
        public decimal? Break2Duration { get; set; }
        public DateTime? Break3StartTime { get; set; }
        public DateTime? Break3EndTime { get; set; }
        public decimal? Break3Duration { get; set; }
        public DateTime? LunchBreakStartTime { get; set; }
        public DateTime? LunchBreakEndTime { get; set; }
        public decimal? LunchDuration { get; set; }
        public decimal? WorkingHours { get; set; }
        public string ShiftGroup { get; set; }
        public byte? FlexiBreakTime { get; set; }
        public long? CreateId { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? LastUpdateId { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? SpecialNSRates { get; set; }
    }

    public class ShiftDto : ShiftModel
    {
        public string WorkSchedule { get; set; }
        public string LunchSchedule { get; set; }
    }
}