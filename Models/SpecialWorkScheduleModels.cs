using System;
using System.Collections.Generic;

namespace MauiHybridApp.Models
{
    public class SpecialWorkScheduleRequestModel
    {
        public long WorkScheduleRequestId { get; set; }
        public long? ProfileId { get; set; }
        public DateTime? WorkDate { get; set; }
        public long ShiftId { get; set; }
        public string Reason { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? LunchBreakStartTime { get; set; }
        public DateTime? LunchBreakEndTime { get; set; }
        public bool? ForOffsetting { get; set; }
        public DateTime? OffsettingExpirationDate { get; set; }
        public long? StatusId { get; set; }
        public short? SourceId { get; set; }
        
        // Helper properties for UI binding
        public double WorkingHours { get; set; }
        public double LunchDuration { get; set; }
    }

    public class SpecialWorkScheduleListModel
    {
        public long WorkScheduleRequestId { get; set; }
        public long? ProfileId { get; set; }
        public string RequestNo { get; set; }
        public DateTime WorkDate { get; set; }
        public string ShiftCode { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public long? StatusId { get; set; }
        
        // Display properties
        public string WorkDateDisplay => WorkDate.ToString("MMM dd, yyyy");
    }

    public class ShiftModel
    {
        public long ShiftId { get; set; }
        public string Code { get; set; }
        public string WorkSchedule { get; set; } // e.g., "09:00 AM - 06:00 PM"
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? LunchBreakStartTime { get; set; }
        public DateTime? LunchBreakEndTime { get; set; }
        
        public string DisplayText => ShiftId < 0 ? Code : $"{Code} ({WorkSchedule})";

        // Properties added for ChangeWorkSchedule parity
        public string Description { get; set; } = string.Empty;
        public string DaysOfWeek { get; set; } = string.Empty;
        public short? StartTimePreviousDay { get; set; }
        public short? EndTimeNextDay { get; set; }
        public decimal? Break1Duration { get; set; }
        public decimal? Break2Duration { get; set; }
        public decimal? Break3Duration { get; set; }
        public decimal? LunchDuration { get; set; }
        public decimal? WorkingHours { get; set; }
        public bool? SpecialNSRates { get; set; }
    }

    public class ShiftDto : ShiftModel
    {
         public string LunchSchedule { get; set; } = string.Empty;
         // WorkSchedule is already in base ShiftModel, removed from here to avoid hiding warning
    }

    public class SpecialWorkScheduleListResponseWrapper
    {
        public List<SpecialWorkScheduleListModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
        public int TotalListCount { get; set; }
    }
    
    public class ShiftListResponseWrapper
    {
        public List<ShiftModel> ListData { get; set; }
    }
    
    public class ShiftDetailResponseWrapper
    {
        public ShiftModel Shift { get; set; }
    }
}
