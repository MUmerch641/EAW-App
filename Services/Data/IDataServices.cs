// This file contains all data service interfaces
// Implementations will be created in separate files

using MauiHybridApp.Models.Leave;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Models.Schedule;
using MauiHybridApp.Models.Attendance;
using MauiHybridApp.Models.Workflow;
using MauiHybridApp.Models;

namespace MauiHybridApp.Services.Data;

// Authentication
public interface IAuthenticationDataService
{
    Task<AuthenticationResult> AuthenticateAsync(LoginRequest request);
    Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<RegistrationResult> RegistrationAsync(RegistrationRequest request);
    Task<bool> ValidateTokenAsync(string token);
    Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken);
}

// Main Page / Menu
public interface IMainPageDataService
{
    Task<List<object>> GetMenuItemsAsync();
    Task SaveDeviceInfoAsync();
}

// Dashboard
public interface IDashboardDataService
{
    Task<DashboardResponse> GetDashboardAsync();
}

// Leave
public interface ILeaveDataService
{
    Task<List<LeaveRequestModel>> GetLeaveRequestsAsync();
    Task<LeaveRequestModel?> GetLeaveRequestByIdAsync(long id);
    Task<SaveResult> SaveLeaveRequestAsync(LeaveRequestModel request);
    Task<SaveResult> SubmitLeaveRequestAsync(LeaveRequestModel request);
    Task<bool> DeleteLeaveRequestAsync(long id);
    Task<List<SelectableListModel>> GetLeaveTypesAsync();
    Task<LeaveSummary> CalculateLeaveSummaryAsync(long leaveTypeId, DateTime startDate, DateTime endDate, int applyToOption);
}

public class SaveResult
{
    public bool Success { get; set; }
    public long Id { get; set; }
    public string? ErrorMessage { get; set; }
}

public class LeaveSummary
{
    public decimal TotalDays { get; set; }
    public decimal TotalHours { get; set; }
    public decimal RemainingBalance { get; set; }
}

public class OvertimeSummary
{
    public decimal TotalHours { get; set; }
    public decimal RegularOTHours { get; set; }
    public decimal NightShiftOTHours { get; set; }
}

public class AttendanceRecordModel
{
    public DateTime Date { get; set; }
    public DateTime? TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }
    public decimal? TotalHours { get; set; }
    public string? Status { get; set; }
    public string? Location { get; set; }
    public string? Remarks { get; set; }
}

public class AttendanceSummaryModel
{
    public int TotalDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateDays { get; set; }
    public decimal TotalHours { get; set; }
    public decimal AverageHoursPerDay { get; set; }
}
// ... Upar Interfaces same rahenge ...

// ✅ UPDATED USER PROFILE MODEL (UI Model)
public class UserProfileModel
{
    // 🔥 Added ProfileId to fix CS0117
    public long ProfileId { get; set; } 

    public string EmployeeId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    
    // Note: FirstName aur LastName alag se nahi hain, hum FullName use karenge
    
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string? ManagerName { get; set; }
    public DateTime? HireDate { get; set; }
    public string? EmploymentStatus { get; set; }
    public string? WorkLocation { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactRelationship { get; set; }
    public string? EmergencyContactPhone { get; set; }
}

// ... Baki models same rahenge (LoginRequest, etc) ...
// Authentication Models
public class LoginRequest
{
    public string username { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public int portal { get; set; } = 0;
}

public class AuthenticationResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public object? User { get; set; }
    public DateTime? TokenExpiry { get; set; }
}

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}

public class ForgotPasswordResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? ErrorMessage { get; set; }
}

public class RegistrationRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string? EmployeeNo { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public class RegistrationResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? ErrorMessage { get; set; }
}

public class RefreshTokenResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? TokenExpiry { get; set; }
    public string? ErrorMessage { get; set; }
}

// Overtime
public interface IOvertimeDataService
{
    Task<List<OvertimeModel>> GetOvertimeRequestsAsync();
    Task<OvertimeModel?> GetOvertimeRequestByIdAsync(long id);
    Task<SaveResult> SaveOvertimeRequestAsync(OvertimeModel request);
    Task<SaveResult> SubmitOvertimeRequestAsync(OvertimeModel request);
    Task<bool> DeleteOvertimeRequestAsync(long id);
    Task<OvertimeSummary> CalculateOvertimeSummaryAsync(DateTime overtimeDate, DateTime startTime, DateTime endTime);
}

// Official Business
public interface IOfficialBusinessDataService
{
    Task<List<OfficialBusinessModel>> GetOfficialBusinessRequestsAsync();
    Task<OfficialBusinessModel?> GetOfficialBusinessRequestByIdAsync(long id);
    Task<SaveResult> SaveOfficialBusinessRequestAsync(OfficialBusinessModel request);
    Task<SaveResult> SubmitOfficialBusinessRequestAsync(OfficialBusinessModel request);
    Task<bool> DeleteOfficialBusinessRequestAsync(long id);
}

// Time Entry
public interface ITimeEntryDataService
{
    Task<List<TimeEntryLogItem>> GetTimeEntriesAsync();
    Task<SaveResult> CreateTimeEntryAsync(string type, double lat, double lng);
}

// Expense
public interface IExpenseDataService
{
    Task<List<ExpenseModel>> GetExpensesAsync();
    Task<SaveResult> SubmitExpenseAsync(ExpenseModel request);
    
    // Dropdown ke liye types (e.g. Travel, Food)
    Task<List<SelectableListModel>> GetExpenseTypesAsync();
}

// Approvals
public interface IApprovalDataService
{
    Task<List<MyApprovalListModel>> GetMyApprovalsAsync(); // Ye zaroori hai UI ke liye
    Task<SaveResult> ApproveRequestAsync(long requestId, string comments);
    Task<SaveResult> DisapproveRequestAsync(long requestId, string comments);
    
    // Purane stubs
    Task<List<MyApprovalListModel>> GetPendingApprovalsAsync();
    Task<MyApprovalListModel?> GetApprovalByIdAsync(long id);
}

// Attendance
public interface IAttendanceDataService
{
    Task<List<AttendanceRecordModel>> GetAttendanceRecordsAsync(DateTime startDate, DateTime endDate);
    Task<AttendanceSummaryModel?> GetAttendanceSummaryAsync(DateTime startDate, DateTime endDate);
}

// User Profile
public interface IUserDataService
{
    Task<UserProfileModel?> GetUserProfileAsync();
    Task<SaveResult> UpdateUserProfileAsync(UserProfileModel profile);
}

// Payroll
public interface IPayrollDataService
{
    Task<List<object>> GetPayslipsAsync();
    Task<object> GetPayslipDetailAsync(long id);
}

// Performance
public interface IPerformanceDataService
{
    Task<List<object>> GetPerformanceEvaluationsAsync();
    Task<object> GetIndividualObjectivesAsync();
}

// Employee Relations
public interface IEmployeeRelationsDataService
{
    Task<List<object>> GetSuggestionsAsync();
    Task<object> SubmitSuggestionAsync(object suggestion);
}

// Profile
public interface IProfileDataService
{
    Task<object> GetProfileAsync();
    Task<object> UpdateProfileAsync(object profile);
}

// Notifications
public interface INotificationDataService
{
    Task<List<object>> GetNotificationsAsync();
    Task MarkAsReadAsync(long notificationId);
}

// Common
public interface ICommonDataService
{
    Task<object> GetAppSettingsAsync();
}

// SignalR
public interface ISignalRDataService
{
    Task StartConnectionAsync();
    Task StopConnectionAsync();
    event Action<string>? OnNotificationReceived;
}

