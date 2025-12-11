using System;

namespace MauiHybridApp.Models
{
    public class MyScheduleListModel
    {
        public DateTime? WorkDate { get; set; }
        public string WorkDateDisplay { get; set; }
        public string WorkSchedule { get; set; }
        public bool IsRestday { get; set; }
        public bool IsHoliday { get; set; }
        public bool HasSchedule { get; set; }
        
        // Overtime
        public string ASOTDuration { get; set; }
        public string PSOTDuration { get; set; }
        public string OTSchedule { get; set; }
        
        // Official Business
        public string OBDuration { get; set; }
        public string OBReason { get; set; }
        
        // Time Off
        public string TODuration { get; set; }
        public string TOReason { get; set; }
        
        // Undertime
        public string UTDuration { get; set; }
        public string UTReason { get; set; }
    }

    public class ScheduleRequestModel
    {
        public string ProfileId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
    }

    public class ChangeWorkScheduleRequest : ScheduleRequestModel
    {
        public string NewSchedule { get; set; }
    }

    public class ChangeRestDayRequest : ScheduleRequestModel
    {
        public DateTime NewRestDay { get; set; }
    }
}
