using MauiHybridApp.Utils;

namespace MauiHybridApp.Models.Leave;

public class LeaveRequestModel
{
    public LeaveRequestModel()
    {
        // Set Default Values so API doesn't get NULL
        PartialDayLeave = 0; 
        PartialDayApplyTo = 0; 
        Planned = 0;
        NoOfDays = 0;
        NoOfHours = 0;
        TotalNoOfHours = 0;
        RemainingHours = 0;
        CompanyId = 0;
        StatusId = RequestStatusValue.Submitted;
        SourceId = (short)SourceEnum.Mobile;
        LeaveRequestHeaderId = 0; // Initialize to 0
        
        DateFiled = DateTime.UtcNow;
        InclusiveStartDate = DateTime.UtcNow.Date;
        InclusiveEndDate = DateTime.UtcNow.Date;
    }

    public long LeaveRequestId { get; set; }
    public long ProfileId { get; set; }
    public long? LeaveTypeId { get; set; }
    public long? CompanyId { get; set; }
    public decimal? RemainingHours { get; set; }
    public DateTime? InclusiveStartDate { get; set; }
    public DateTime? InclusiveEndDate { get; set; }
    public decimal? NoOfHours { get; set; }
    
    // 🔥 CHANGE: Remove '?' to prevent NULL (Make them short)
    public short PartialDayLeave { get; set; } 
    public short PartialDayApplyTo { get; set; } 
    
    public string Reason { get; set; } = string.Empty; // String empty rakho
    public long StatusId { get; set; }
    public DateTime? DateFiled { get; set; }
    public decimal? TotalNoOfHours { get; set; }
    public short? NoOfDays { get; set; }
    
    // 🔥 CHANGE: Remove '?'
    public short Planned { get; set; } 
    
    // 🔥 CHANGE: Remove '?' (Make it long, not long?)
    public long LeaveRequestHeaderId { get; set; } 
    
    public short? SourceId { get; set; }
}
