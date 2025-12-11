using EatWork.Mobile.Contants;
using System;

namespace EatWork.Mobile.Models
{
    public class TimeEntryLogModel
    {
        public TimeEntryLogModel()
        {
            TimeEntry = DateTime.Now;
            //CreateDate = DateTime.Now;
            StatusId = RequestStatusValue.Draft;
            Source = Constants.SourceOnlineTimeEntry;
            BreakType = 0;
            Location = string.Empty;
            IPType = string.Empty;
            SyncBy = 0;
            IPAddress = string.Empty;
            Latitude = string.Empty;
            Longitude = string.Empty;
            PublicIPAddress = string.Empty;
            CreateId = 0;
            SourceId = (short)SourceEnum.Mobile;
        }

        public long TimeEntryLogId { get; set; }
        public long? ProfileId { get; set; }
        public long? StatusId { get; set; }
        public DateTime? TimeEntry { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
        public string Location { get; set; }
        public string MarkCode { get; set; }
        public string Remark { get; set; }
        public string IPAddress { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string IPType { get; set; }
        public string PublicIPAddress { get; set; }
        public long? CreateId { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? LastUpdateId { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public long? SyncBy { get; set; }
        public short? BreakType { get; set; }
        public short? SourceId { get; set; }
    }
}