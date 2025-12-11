using MauiHybridApp.Utils;

namespace MauiHybridApp.Models; // Namespace yehi rehne dena

public class OfficialBusinessModel
{
    public OfficialBusinessModel()
    {
        // Defaults
        OfficialBusinessId = 0;
        ProfileId = 0;
        StatusId = RequestStatusValue.Submitted;
        SourceId = (short)SourceEnum.Mobile;
        
        DateFiled = DateTime.UtcNow;
        OfficialBusinessDate = DateTime.UtcNow.Date;
        
        // Time Defaults
        StartTime = DateTime.UtcNow;
        EndTime = DateTime.UtcNow.AddHours(1);
        
        NoOfHours = 0;
        
        // String Defaults
        Remarks = string.Empty; 
        Reason = string.Empty;
        ApproverRemarks = string.Empty;
        
        // UI Defaults
        Destination = string.Empty;
        Purpose = string.Empty;
        Transportation = string.Empty;
        
        // Booleans
        StartTimePreviousDay = false;
        EndTimeNextDay = false;
    }

    // --- API Fields ---
    public long OfficialBusinessId { get; set; }
    public long ProfileId { get; set; }
    public long TransactionTypeId { get; set; } // Added for Time Off Request reuse
    public DateTime DateFiled { get; set; }
    public DateTime OfficialBusinessDate { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal NoOfHours { get; set; }
    
    public string Remarks { get; set; } // Details
    public string Reason { get; set; }  // Purpose
    public string ApproverRemarks { get; set; }
    
    public long StatusId { get; set; }
    public short SourceId { get; set; }
    
    public bool StartTimePreviousDay { get; set; }
    public bool EndTimeNextDay { get; set; }

    // --- UI Only Fields ---
    public DateTime? StartDate { get; set; } 
    public DateTime? EndDate { get; set; }
    public string Destination { get; set; }
    public string Purpose { get; set; }
    
    // Ye fields pehle missing thin, ab add kar di hain:
    public string Transportation { get; set; }
    public bool? WithAllowance { get; set; }
    public decimal? EstimatedCost { get; set; }
}

public class OBSummary
{
    public int TotalDays { get; set; }
    public decimal TotalHours { get; set; }
}