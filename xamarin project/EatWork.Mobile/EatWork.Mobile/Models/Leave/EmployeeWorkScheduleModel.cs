using System;
using System.Collections.Generic;
using System.Text;

namespace EatWork.Mobile.Models.Leave
{
    public class EmployeeWorkScheduleModel
    {
        public long WorkScheduleId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime WorkDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime LunchBreakStartTime { get; set; }
        public DateTime LunchBreakEndTime { get; set; }
        public long ShiftId { get; set; }
        public int DayOfWeek { get; set; }
        public byte FixSchedule { get; set; }
        public decimal WorkingHours { get; set; }
        public string ShiftCode { get; set; }
        public decimal LunchDuration { get; set; }
        public string Remarks { get; set; }
    }
}
