using SQLite;

namespace MauiHybridApp.Models.DataAccess;

[Table("Login")]
public class LoginDataModel
{
    [PrimaryKey, AutoIncrement]
    public long ID { get; set; }

    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

[Table("CacheData")]
public class CacheDataModel
{
    [PrimaryKey, AutoIncrement]
    public long ID { get; set; }

    public string UserId { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

[Table("UserPreferences")]
public class UserPreferenceModel
{
    [PrimaryKey, AutoIncrement]
    public long ID { get; set; }

    public string UserId { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

[Table("OfflineRequests")]
public class OfflineRequestModel
{
    [PrimaryKey, AutoIncrement]
    public long ID { get; set; }

    public string UserId { get; set; } = string.Empty;
    public string RequestType { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsSynced { get; set; }
    public DateTime? SyncedAt { get; set; }
    public string? ErrorMessage { get; set; }
}

[Table("LeaveRequests")]
public class LeaveRequestCacheModel
{
    [PrimaryKey, AutoIncrement]
    public long ID { get; set; }

    public string UserId { get; set; } = string.Empty;
    public long? RequestId { get; set; }
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsOffline { get; set; }
}

[Table("TimeEntries")]
public class TimeEntryCacheModel
{
    [PrimaryKey, AutoIncrement]
    public long ID { get; set; }

    public string UserId { get; set; } = string.Empty;
    public long? EntryId { get; set; }
    public DateTime Date { get; set; }
    public DateTime? TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }
    public string? Location { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsOffline { get; set; }
}

[Table("AttendanceRecords")]
public class AttendanceRecordCacheModel
{
    [PrimaryKey, AutoIncrement]
    public long ID { get; set; }

    public string UserId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime? TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }
    public decimal? TotalHours { get; set; }
    public string? Status { get; set; }
    public string? Location { get; set; }
    public string? Remarks { get; set; }
    public DateTime CachedAt { get; set; }
}

[Table("Approvals")]
public class ApprovalCacheModel
{
    [PrimaryKey, AutoIncrement]
    public long ID { get; set; }

    public string UserId { get; set; } = string.Empty;
    public long RequestId { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public DateTime CachedAt { get; set; }
}

// Cache data types constants
public static class CacheDataTypes
{
    public const string Dashboard = "Dashboard";
    public const string LeaveRequests = "LeaveRequests";
    public const string OvertimeRequests = "OvertimeRequests";
    public const string TimeEntries = "TimeEntries";
    public const string AttendanceRecords = "AttendanceRecords";
    public const string Approvals = "Approvals";
    public const string UserProfile = "UserProfile";
    public const string MenuItems = "MenuItems";
}

// Offline request types
public static class OfflineRequestTypes
{
    public const string LeaveRequest = "LeaveRequest";
    public const string OvertimeRequest = "OvertimeRequest";
    public const string TimeEntry = "TimeEntry";
    public const string ClockIn = "ClockIn";
    public const string ClockOut = "ClockOut";
    public const string Approval = "Approval";
    public const string ProfileUpdate = "ProfileUpdate";
}
