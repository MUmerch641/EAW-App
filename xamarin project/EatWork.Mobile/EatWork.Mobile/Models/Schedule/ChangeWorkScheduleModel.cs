using EatWork.Mobile.Contants;
using System;

namespace EatWork.Mobile.Models
{
    public class ChangeWorkScheduleModel
    {
        public ChangeWorkScheduleModel()
        {
            DateFiled = DateTime.Now.Date;
            WorkDate = DateTime.Now.Date;
            StartTimePreviousDay = false;
            EndTimeNextDay = false;
            WorkingHours = (Decimal)0;
            StatusId = RequestStatusValue.Draft;
            SwapWithProfileId = 0;
            SourceId = (short)SourceEnum.Mobile;
        }

        public long ChangeWorkScheduleId { get; set; }
        public long? ProfileId { get; set; }
        public long? ShiftId { get; set; }
        public DateTime? DateFiled { get; set; }
        public DateTime? WorkDate { get; set; }
        public DateTime? WorkScheduleStartTime { get; set; }
        public DateTime? WorkScheduleEndTime { get; set; }
        public long? SwapWithProfileId { get; set; }
        public DateTime? RequestedStartTime { get; set; }
        public DateTime? RequestedEndTime { get; set; }
        public short? Reason { get; set; }
        public string ApproverRemarks { get; set; }
        public long? OriginalShiftId { get; set; }
        public long? StatusId { get; set; }
        public long? CreateId { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? LastUpdateId { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string WorkScheduleId { get; set; }
        public bool? StartTimePreviousDay { get; set; }
        public bool? EndTimeNextDay { get; set; }
        public bool? SpecialNSRates { get; set; }
        public decimal? WorkingHours { get; set; }
        public DateTime? LunchBreakStartTime { get; set; }
        public DateTime? LunchBreakEndTime { get; set; }
        public decimal? LunchDuration { get; set; }
        public DateTime? Break1StartTime { get; set; }
        public DateTime? Break1EndTime { get; set; }
        public decimal? Break1Duration { get; set; }
        public DateTime? Break2StartTime { get; set; }
        public DateTime? Break2EndTime { get; set; }
        public decimal? Break2Duration { get; set; }
        public DateTime? Break3StartTime { get; set; }
        public DateTime? Break3EndTime { get; set; }
        public decimal? Break3Duration { get; set; }
        public string Details { get; set; }
        public short? SourceId { get; set; }
    }
}