using EatWork.Mobile.Contants;
using System;

namespace EatWork.Mobile.Models
{
    public class SpecialWorkScheduleRequestModel
    {
        public SpecialWorkScheduleRequestModel()
        {
            StatusId = RequestStatusValue.Draft;
            ProfileId = 0;
            WorkingHours = (Decimal)0;
            DateFiled = DateTime.Now.Date;
            WorkDate = DateTime.Now.Date;
            RequestType = 0;
            TransactionId = 0;
            RequestType = 0;
            Break1StartTime = Constants.NullDate;
            Break1EndTime = Constants.NullDate;
            Break2StartTime = Constants.NullDate;
            Break2EndTime = Constants.NullDate;
            Break3StartTime = Constants.NullDate;
            Break3EndTime = Constants.NullDate;
            LunchDuration = 0;
            Break1Duration = 0;
            Break2Duration = 0;
            Break3Duration = 0;
            WorkScheduleId = 0;
            StandardWorkingHours = 0;
            OverrideAttendancePolicyRule = 0;
            FlexiTime = 0;
            FlexiTimeLimit = 0;
            EarlyTimeIn = 0;
            EarlyTimeInLimit = 0;
            EarlyTimeInOvertime = 0;
            ForOffsetting = false;
            SpecialNSRates = false;
            OffsettingExpirationDate = Constants.NullDate;
            SourceId = (short)SourceEnum.Mobile;
        }

        public long WorkScheduleRequestId { get; set; }
        public long? ProfileId { get; set; }
        public DateTime? DateFiled { get; set; }
        public DateTime? WorkDate { get; set; }
        public short? RequestType { get; set; }
        public long? TransactionId { get; set; }
        public long? ShiftId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? LunchBreakStartTime { get; set; }
        public DateTime? LunchBreakEndTime { get; set; }
        public DateTime? Break1StartTime { get; set; }
        public DateTime? Break1EndTime { get; set; }
        public DateTime? Break2StartTime { get; set; }
        public DateTime? Break2EndTime { get; set; }
        public DateTime? Break3StartTime { get; set; }
        public DateTime? Break3EndTime { get; set; }
        public decimal? LunchDuration { get; set; }
        public decimal? Break1Duration { get; set; }
        public decimal? Break2Duration { get; set; }
        public decimal? Break3Duration { get; set; }
        public decimal? WorkingHours { get; set; }
        public string Reason { get; set; }
        public string ApproverRemarks { get; set; }
        public long? WorkScheduleId { get; set; }
        public decimal? StandardWorkingHours { get; set; }
        public long? StatusId { get; set; }
        public byte? OverrideAttendancePolicyRule { get; set; }
        public byte? FlexiTime { get; set; }
        public decimal? FlexiTimeLimit { get; set; }
        public byte? EarlyTimeIn { get; set; }
        public decimal? EarlyTimeInLimit { get; set; }
        public byte? EarlyTimeInOvertime { get; set; }
        public short? SourceId { get; set; }
        public long? CreateId { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? LastUpdateId { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? ForOffsetting { get; set; }
        public DateTime? OffsettingExpirationDate { get; set; }
        public bool? SpecialNSRates { get; set; }
    }
}