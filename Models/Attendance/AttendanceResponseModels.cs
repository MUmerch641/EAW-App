using System.Collections.ObjectModel;

namespace MauiHybridApp.Models.Attendance;

public class AttendanceListResponse
{
    public List<MyAttendanceListModel> ListData { get; set; } = new();
    public long TotalListCount { get; set; }
}

public class MyAttendanceListModel
{
    public DateTime? WorkDate { get; set; }
    public string ActualIn { get; set; }
    public string ActualOut { get; set; }
    public decimal TotalHours { get; set; }
    public string Remarks { get; set; }
    public string HolidayName { get; set; }
    public bool IsRestday { get; set; }
    
    // Status counters
    public decimal Absent { get; set; }
    public decimal Late { get; set; }
    public decimal Undertime { get; set; }
    public decimal ApprovedRegularOT { get; set; }
    public decimal ApprovedNSOT { get; set; }
    public decimal VLHrs { get; set; }
    public decimal SLHrs { get; set; }
    public decimal LWOP { get; set; }
}

public class OnlineTimeEntryInitFormResponse
{
    public bool HasSetup { get; set; }
    public string UserError { get; set; }
    public DateTime ServerTime { get; set; }
    public ClockworkConfigurationModel ClockworkConfigurationModel { get; set; }
}

public class ClockworkConfigurationModel
{
    public string TimeInColor { get; set; }
    public string TimeOutColor { get; set; }
    public string BreakInColor { get; set; }
    public string BreakOutColor { get; set; }
    public bool AllowImageCapture { get; set; }
    public bool AllowLocationCapture { get; set; }
    public bool ClearEmployeeNo { get; set; }
}

public class OnlineTimeEntryHolder
{
    public bool HasSetup { get; set; }
    public string UserError { get; set; }
    public DateTime TimeClock { get; set; }
    public ClockworkConfigurationModel ClockworkConfiguration { get; set; }
    public string TimeInColor { get; set; }
    public string TimeOutColor { get; set; }
    public string BreakInColor { get; set; }
    public string BreakOutColor { get; set; }
    public bool AllowImageCapture { get; set; }
    public bool AllowLocationCapture { get; set; }
    public string IpAddress { get; set; }
    public string EmployeeNumber { get; set; }
    public string AccessCode { get; set; }
    public TimeEntryLogModel TimeEntryLogModel { get; set; } = new();
    
    // UI Properties
    public bool ErrorEmployeeNumber { get; set; }
    public bool ErrorAccessCode { get; set; }
    public string ResponseMesage { get; set; }
    public bool IsSuccess { get; set; }
    public bool LeaveWarningOnly { get; set; }
    public ObservableCollection<string> UserErrorList { get; set; } = new();
}
