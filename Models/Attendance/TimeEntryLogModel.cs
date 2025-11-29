using MauiHybridApp.Utils;

namespace MauiHybridApp.Models.Attendance;

public class TimeEntryLogModel
{
    public TimeEntryLogModel()
    {
        TimeEntry = DateTime.Now;
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
        Type = string.Empty;
        MarkCode = string.Empty;
        Remark = string.Empty;
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

    // Additional properties for UI binding
    public DateTime? TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }
    public DateTime DateCreated { get; set; }
}
