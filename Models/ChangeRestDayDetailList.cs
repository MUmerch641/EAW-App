using System;

namespace MauiHybridApp.Models
{
    public class ChangeRestDayDetailList
    {
        public long RowId { get; set; }
        public DateTime? RestDayDate { get; set; }
        public DateTime? RequestDate { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string DayOfWeekRequest { get; set; } = string.Empty;
        public int HasAttendance { get; set; }
    }
}
