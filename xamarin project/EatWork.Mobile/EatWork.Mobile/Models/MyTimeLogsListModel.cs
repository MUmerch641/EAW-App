using System;

namespace EatWork.Mobile.Models
{
    public class MyTimeLogsListModel
    {
        public MyTimeLogsListModel()
        {
        }

        public long TimeEntryLogId { get; set; }
        public long ProfileId { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Branch { get; set; }
        public string Division { get; set; }
        public long StatusId { get; set; }
        public string Status { get; set; }
        public DateTime? TimeEntry { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
        public string Username { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Location { get; set; }
        public string IPAddress { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int TotalCount { get; set; }
        public string WorkDateDisplay { get; set; }
    }
}