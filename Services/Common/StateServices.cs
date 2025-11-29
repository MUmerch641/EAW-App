// State management services for Blazor components
// These services maintain state and notify components of changes

namespace MauiHybridApp.Services.Common;

public class DashboardStateService
{
    public event Action? OnStateChanged;
    
    private object? _dashboardData;
    public object? DashboardData
    {
        get => _dashboardData;
        set
        {
            _dashboardData = value;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}

public class LeaveStateService
{
    public event Action? OnStateChanged;
    
    private List<object>? _leaveRequests;
    public List<object>? LeaveRequests
    {
        get => _leaveRequests;
        set
        {
            _leaveRequests = value;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}

public class OvertimeStateService
{
    public event Action? OnStateChanged;
    
    private List<object>? _overtimeRequests;
    public List<object>? OvertimeRequests
    {
        get => _overtimeRequests;
        set
        {
            _overtimeRequests = value;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}

public class OfficialBusinessStateService
{
    public event Action? OnStateChanged;
    
    private List<object>? _officialBusinessRequests;
    public List<object>? OfficialBusinessRequests
    {
        get => _officialBusinessRequests;
        set
        {
            _officialBusinessRequests = value;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}

public class TimeEntryStateService
{
    public event Action? OnStateChanged;
    
    private List<object>? _timeEntries;
    public List<object>? TimeEntries
    {
        get => _timeEntries;
        set
        {
            _timeEntries = value;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}

public class ApprovalStateService
{
    public event Action? OnStateChanged;
    
    private List<object>? _pendingApprovals;
    public List<object>? PendingApprovals
    {
        get => _pendingApprovals;
        set
        {
            _pendingApprovals = value;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}

public class AttendanceStateService
{
    public event Action? OnStateChanged;
    
    private List<object>? _attendanceRecords;
    public List<object>? AttendanceRecords
    {
        get => _attendanceRecords;
        set
        {
            _attendanceRecords = value;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}

public class PayrollStateService
{
    public event Action? OnStateChanged;
    
    private List<object>? _payslips;
    public List<object>? Payslips
    {
        get => _payslips;
        set
        {
            _payslips = value;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}

public class PerformanceStateService
{
    public event Action? OnStateChanged;
    
    private List<object>? _evaluations;
    public List<object>? Evaluations
    {
        get => _evaluations;
        set
        {
            _evaluations = value;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}

public class EmployeeRelationsStateService
{
    public event Action? OnStateChanged;
    
    private List<object>? _suggestions;
    public List<object>? Suggestions
    {
        get => _suggestions;
        set
        {
            _suggestions = value;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}

public class ProfileStateService
{
    public event Action? OnStateChanged;
    
    private object? _profile;
    public object? Profile
    {
        get => _profile;
        set
        {
            _profile = value;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}

public class NotificationStateService
{
    public event Action? OnStateChanged;
    
    private List<object>? _notifications;
    public List<object>? Notifications
    {
        get => _notifications;
        set
        {
            _notifications = value;
            NotifyStateChanged();
        }
    }

    private int _unreadCount;
    public int UnreadCount
    {
        get => _unreadCount;
        set
        {
            _unreadCount = value;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}

