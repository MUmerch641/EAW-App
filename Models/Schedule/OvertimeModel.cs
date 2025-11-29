using MauiHybridApp.Utils;

namespace MauiHybridApp.Models.Schedule;

public class OvertimeModel
{
    public OvertimeModel()
    {
        // Defaults
        OvertimeId = 0;
        ProfileId = 0;
        StatusId = RequestStatusValue.Submitted;
        SourceId = (short)SourceEnum.Mobile;
        
        DateFiled = DateTime.UtcNow;
        OvertimeDate = DateTime.UtcNow.Date;
        
        // Default Times
        StartTime = DateTime.UtcNow;
        EndTime = DateTime.UtcNow.AddHours(1);
        
        // Numeric Defaults (Important for API)
        OROTHrs = 0;
        NSOTHrs = 0;
        ApprovedOROTHrs = 0;
        ApprovedNSOTHrs = 0;
        ComputeHour = 0;
        PreShiftOT = 0;
        
        ForOffsetting = false;
        Reason = string.Empty;
        Remarks = string.Empty;
        ApproverRemarks = string.Empty;
    }

    public long OvertimeId { get; set; }
    public long ProfileId { get; set; }
    public DateTime DateFiled { get; set; }
    public DateTime OvertimeDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public decimal OROTHrs { get; set; }
    public decimal NSOTHrs { get; set; }
    
    public string Reason { get; set; }
    public string Remarks { get; set; }
    
    public decimal ApprovedOROTHrs { get; set; }
    public decimal ApprovedNSOTHrs { get; set; }
    public string ApproverRemarks { get; set; }
    
    public short ComputeHour { get; set; }
    public short PreShiftOT { get; set; }
    
    public long StatusId { get; set; }
    public short SourceId { get; set; }
    
    public bool ForOffsetting { get; set; }
    public DateTime? OffsettingExpirationDate { get; set; }
}