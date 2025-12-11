using System;
using System.Collections.Generic;
using System.Text;

namespace EatWork.Mobile.Models.Attendance
{
    public class DetailedAttendanceListModel
    {
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string CutoffDate { get; set; }
        public string AttendanceType { get; set; }
        public long ProfileId { get; set; }
        public long TimeEntryHeaderId { get; set; }
        public int TotalCount { get; set; }
    }
}
