using System;

namespace EatWork.Mobile.Models
{
    public class ChangeRestDayDetailList
    {
        public long RowId { get; set; }
        public DateTime? RestDayDate { get; set; }
        public DateTime? RequestDate { get; set; }
        public string DayOfWeek { get; set; }
        public string DayOfWeekRequest { get; set; }
        public int HasAttendance { get; set; }
    }
}