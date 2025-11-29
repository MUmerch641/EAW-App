// Stub implementations for all data services
// These will be fully implemented in Phase 2

using Microsoft.AspNetCore.SignalR.Client;
using MauiHybridApp.Models.Leave;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Models.Schedule;
using MauiHybridApp.Models.Attendance;
using MauiHybridApp.Models.Workflow;
using MauiHybridApp.Models;
using MauiHybridApp.Utils;
using MauiHybridApp.Services.Authentication;
using Microsoft.Maui.Storage;
using MauiHybridApp.Models.Employee; // For ProfileModel


namespace MauiHybridApp.Services.Data;

public class MainPageDataService : IMainPageDataService
{
    private readonly IGenericRepository _repository;

    public MainPageDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<object>> GetMenuItemsAsync()
    {
        await Task.Delay(100);
        return new List<object>
        {
            new MenuItemModel { MenuName = "Dashboard" },
            new MenuItemModel { MenuName = "Leave" },
            new MenuItemModel { MenuName = "Overtime" },
            new MenuItemModel { MenuName = "Official Business" },
            new MenuItemModel { MenuName = "Time Entry" },
            new MenuItemModel { MenuName = "Approvals" },
            new MenuItemModel { MenuName = "Attendance" },
            new MenuItemModel { MenuName = "Profile" }
        };
    }

    public Task SaveDeviceInfoAsync()
    {
        // TODO: Implement device info saving
        return Task.CompletedTask;
    }
}

public class DashboardDataService : IDashboardDataService
{
    private readonly IGenericRepository _repository;

    public DashboardDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    public async Task<DashboardResponse> GetDashboardAsync()
    {
        var profileId = await SecureStorage.GetAsync("profile_id");
        
        // Agar profileId na mile to return null
        if (string.IsNullOrEmpty(profileId)) return new DashboardResponse();

        // API Call
        var result = await _repository.GetAsync<DashboardResponse>($"api/v1/dashboard/{profileId}/default");
        
        return result ?? new DashboardResponse();
    }
}

public class LeaveDataService : ILeaveDataService
{
    private readonly IGenericRepository _repository;

    public LeaveDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<LeaveRequestModel>> GetLeaveRequestsAsync()
    {
        try
        {
            var response = await _repository.GetAsync<List<LeaveRequestModel>>(ApiEndpoints.GetLeaveRequests);
            return response ?? new List<LeaveRequestModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting leave requests: {ex.Message}");
            return new List<LeaveRequestModel>();
        }
    }

    public async Task<LeaveRequestModel?> GetLeaveRequestByIdAsync(long id)
    {
        try
        {
            var endpoint = string.Format(ApiEndpoints.GetLeaveRequestById, id);
            return await _repository.GetAsync<LeaveRequestModel>(endpoint);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting leave request {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<SaveResult> SaveLeaveRequestAsync(LeaveRequestModel request)
    {
        try
        {
            if (request.LeaveRequestId == 0)
            {
                var response = await _repository.PostAsync<LeaveRequestModel, SaveResult>(
                    ApiEndpoints.CreateLeaveRequest, request);
                return response ?? new SaveResult { Success = false, ErrorMessage = "Failed to create leave request" };
            }
            else
            {
                var endpoint = string.Format(ApiEndpoints.UpdateLeaveRequest, request.LeaveRequestId);
                var response = await _repository.PutAsync<LeaveRequestModel, SaveResult>(endpoint, request);
                return response ?? new SaveResult { Success = false, ErrorMessage = "Failed to update leave request" };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving leave request: {ex.Message}");
            return new SaveResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<SaveResult> SubmitLeaveRequestAsync(LeaveRequestModel request)
    {
        try
        {
            // 1. Profile ID Check
            if (request.ProfileId == 0)
            {
                var pid = await SecureStorage.GetAsync("profile_id");
                if (long.TryParse(pid, out long id)) request.ProfileId = id;
            }

            // 2. Company ID Fix (Try 1 instead of 0, sometimes 0 is invalid)
            if (request.CompanyId == null || request.CompanyId == 0) 
                request.CompanyId = 1; 

            // 3. ⏰ DATE FIX: Time component hata do (Midnight set karo)
            // Taake 19:00:00Z server par ja kar agla din na ban jaye
            if (request.InclusiveStartDate.HasValue)
                request.InclusiveStartDate = request.InclusiveStartDate.Value.Date;
                
            if (request.InclusiveEndDate.HasValue)
                request.InclusiveEndDate = request.InclusiveEndDate.Value.Date;
                
            request.DateFiled = DateTime.Now; // UTC nahi, Local time bhej kar dekho

            // Wrapper
            var payload = new { data = request };

            // =========================================================
            // �️‍♂️ DEBUG MODE: Direct HTTP Client to see RAW RESPONSE
            // =========================================================
            // 4. API Call
            // Hum GenericRepository wapis use kar sakte hain ab
            var response = await _repository.PostAsync<object, LeaveApiResponse>(ApiEndpoints.CreateLeaveRequest, payload);

            if (response != null)
            {
                // 🔥 SUCCESS LOGIC FIX:
                // Agar IsSuccess true hai YA Model null nahi hai, to Success hai
                if (response.IsSuccess || response.Model != null)
                {
                    return new SaveResult { Success = true };
                }
                else
                {
                    string error = response.ValidationMessage ?? "";
                    if (response.ValidationMessages != null && response.ValidationMessages.Any())
                    {
                        error = string.Join(", ", response.ValidationMessages);
                    }
                    return new SaveResult { Success = false, ErrorMessage = error ?? "Submission failed." };
                }
            }
            
            return new SaveResult { Success = false, ErrorMessage = "Server returned null response" };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EXCEPTION] {ex.Message}");
            return new SaveResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<bool> DeleteLeaveRequestAsync(long id)
    {
        try
        {
            var endpoint = string.Format(ApiEndpoints.DeleteLeaveRequest, id);
            return await _repository.DeleteAsync(endpoint);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting leave request {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<List<SelectableListModel>> GetLeaveTypesAsync()
    {
        try
        {
            // API: GET /api/leave/types (using original endpoint from Constants)
            var response = await _repository.GetAsync<List<SelectableListModel>>(ApiEndpoints.GetLeaveTypes);
            Console.WriteLine($"GetLeaveTypes API Response: {response?.Count ?? 0} items");

            // If API returns data, use it; otherwise fall back to defaults
            if (response != null && response.Any())
            {
                return response;
            }
            else
            {
                Console.WriteLine("API returned no leave types, using defaults");
                return GetDefaultLeaveTypes();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetLeaveTypes Error: {ex.Message}, using defaults");
            return GetDefaultLeaveTypes();
        }
    }

    public async Task<LeaveSummary> CalculateLeaveSummaryAsync(long leaveTypeId, DateTime startDate, DateTime endDate, int applyToOption)
    {
        // Result object initialize karo
        var summary = new LeaveSummary();

        try
        {
            var profileIdStr = await SecureStorage.GetAsync("profile_id");
            long.TryParse(profileIdStr, out long pid);

            // ---------------------------------------------------------
            // 1. DATE FIX: Send as String "yyyy-MM-dd" to avoid Timezone issues
            // ---------------------------------------------------------
            var calcRequest = new
            {
                ProfileId = pid,
                LeaveTypeId = leaveTypeId,
                InclusiveStartDate = startDate.ToString("yyyy-MM-dd"), // String bhejo
                InclusiveEndDate = endDate.ToString("yyyy-MM-dd"),     // String bhejo
                Duration = 1,
                IsHalfDay = applyToOption > 1,
                DateFiled = DateTime.UtcNow,
                CreateDate = DateTime.UtcNow
            };

            Console.WriteLine($"[CALC] Sending Dates: {calcRequest.InclusiveStartDate} to {calcRequest.InclusiveEndDate}");

            // API Call
            var response = await _repository.PostAsync<object, LeaveSummary>(ApiEndpoints.CalculateLeaveSummary, calcRequest);

            if (response != null)
            {
                summary = response;
                Console.WriteLine($"[CALC] API Result: Days={summary.TotalDays}, Hours={summary.TotalHours}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CALC ERROR] {ex.Message}");
        }

        // ---------------------------------------------------------
        // 2. FALLBACK LOGIC (Safety Net)
        // Agar API fail ho jaye ya 0 return kare, to hum khud calculate karenge
        // taake User "Submit" kar sake.
        // ---------------------------------------------------------
        if (summary.TotalDays <= 0)
        {
            Console.WriteLine("[CALC] API returned 0. Using Local Fallback calculation.");
            
            // Khud calculate karo: (End - Start) + 1
            // Example: 28th to 28th = 0 diff + 1 = 1 Day
            double daysDiff = (endDate.Date - startDate.Date).TotalDays + 1;
            
            // Agar Half Day hai to 0.5 karo
            if (applyToOption > 1) 
            {
                summary.TotalDays = 0.5m;
                summary.TotalHours = 4; // Assume 4 hours for half day
            }
            else
            {
                summary.TotalDays = (decimal)daysDiff;
                summary.TotalHours = (decimal)daysDiff * 8; // Assume 8 hours per day
            }
        }

        return summary;
    }

    private static List<SelectableListModel> GetDefaultLeaveTypes()
    {
        return new List<SelectableListModel>
        {
            new() { Id = 1, DisplayText = "Vacation Leave", IsChecked = false, DisplayData = "VL" },
            new() { Id = 2, DisplayText = "Sick Leave", IsChecked = false, DisplayData = "SL" },
            new() { Id = 3, DisplayText = "Emergency Leave", IsChecked = false, DisplayData = "EL" },
            new() { Id = 4, DisplayText = "Maternity Leave", IsChecked = false, DisplayData = "ML" },
            new() { Id = 5, DisplayText = "Paternity Leave", IsChecked = false, DisplayData = "PL" }
        };
    }

    public class LeaveSummaryRequest
    {
        public long LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ApplyToOption { get; set; }
    }
}

public class OvertimeDataService : IOvertimeDataService
{
    private readonly IGenericRepository _repository;

    public OvertimeDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    public async Task<SaveResult> SubmitOvertimeRequestAsync(OvertimeModel request)
    {
        try
        {
            // 1. Profile ID Check
            if (request.ProfileId == 0)
            {
                var pid = await SecureStorage.GetAsync("profile_id");
                if (long.TryParse(pid, out long id)) request.ProfileId = id;
            }

            // 2. Date Fixes (UTC)
            request.DateFiled = DateTime.UtcNow;
            
            // 3. Construct Payload (Swagger Style)
            var payload = new 
            {
                data = request,
                skipExistingOT = true,
                skipMinimumOT = true,
                allowNoWorkOT = true,
                minimumOTHoursToggle = true,
                overrideMinimumOT = true
            };

            // 4. API Call
            var response = await _repository.PostAsync<object, LeaveApiResponse>(ApiEndpoints.CreateOvertimeRequest, payload);

            if (response != null)
            {
                if (response.IsSuccess || response.Model != null)
                {
                    return new SaveResult { Success = true };
                }
                else
                {
                    string error = response.ValidationMessage ?? "";
                    if (response.ValidationMessages != null && response.ValidationMessages.Any())
                        error = string.Join(", ", response.ValidationMessages);
                        
                    return new SaveResult { Success = false, ErrorMessage = error ?? "Submission failed." };
                }
            }

            return new SaveResult { Success = false, ErrorMessage = "Server returned no response" };
        }
        catch (Exception ex)
        {
            return new SaveResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    // Stubs
    public Task<List<OvertimeModel>> GetOvertimeRequestsAsync() => Task.FromResult(new List<OvertimeModel>());
    public Task<OvertimeModel?> GetOvertimeRequestByIdAsync(long id) => Task.FromResult<OvertimeModel?>(null);
    public Task<SaveResult> SaveOvertimeRequestAsync(OvertimeModel request) => SubmitOvertimeRequestAsync(request);
    public Task<bool> DeleteOvertimeRequestAsync(long id) => Task.FromResult(false);
    public Task<OvertimeSummary> CalculateOvertimeSummaryAsync(DateTime d, DateTime s, DateTime e) => Task.FromResult(new OvertimeSummary());
}




public class OfficialBusinessDataService : IOfficialBusinessDataService
{
    private readonly IGenericRepository _repository;

    public OfficialBusinessDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    public async Task<SaveResult> SubmitOfficialBusinessRequestAsync(OfficialBusinessModel request)
    {
        try
        {
            // 1. Profile ID Check
            if (request.ProfileId == 0)
            {
                var pid = await SecureStorage.GetAsync("profile_id");
                if (long.TryParse(pid, out long id)) request.ProfileId = id;
            }

            // 2. Date Setup
            request.DateFiled = DateTime.UtcNow;
            
            // UI Dates (Safe handling)
            var sDate = request.StartDate ?? DateTime.UtcNow.Date;
            var eDate = request.EndDate ?? DateTime.UtcNow.Date;
            
            request.OfficialBusinessDate = sDate;

            // 3. Time Sync (Fixing the Error CS1061)
            // Agar StartTime null hai to subah 9 baje ka default lelo
            TimeSpan startTimePart = request.StartTime.HasValue 
                ? request.StartTime.Value.TimeOfDay 
                : new TimeSpan(9, 0, 0);

            // Agar EndTime null hai to sham 5 baje ka default lelo
            TimeSpan endTimePart = request.EndTime.HasValue 
                ? request.EndTime.Value.TimeOfDay 
                : new TimeSpan(17, 0, 0);

            // Date aur Time ko combine karo
            request.StartTime = sDate.Date.Add(startTimePart);
            request.EndTime = eDate.Date.Add(endTimePart);
            
            // 4. Duration Calculation (Safe Math)
            // Ab kyunke humne upar value assign kardi hai, hum .Value use kar sakte hain
            if (request.StartTime.HasValue && request.EndTime.HasValue)
            {
                request.NoOfHours = (decimal)(request.EndTime.Value - request.StartTime.Value).TotalHours;
            }
            else
            {
                request.NoOfHours = 0;
            }

            // 5. Mapping (UI -> API)
            request.Reason = request.Purpose; 
            
            string transport = !string.IsNullOrEmpty(request.Transportation) ? $"Via: {request.Transportation}" : "";
            string cost = request.WithAllowance == true ? $"(Allowance: {request.EstimatedCost})" : "";
            request.Remarks = $"Destination: {request.Destination}. {transport} {cost}".Trim();

            // 6. Payload
            var payload = new 
            {
                data = request,
                startDate = request.StartTime,
                endDate = request.EndTime,
                proceed = true,
                isResubmit = false,
                isPostFiling = true,
                overrideNoWorkSchedule = true,
                includePreFilingDates = true,
                officialBusinessToSave = new List<OfficialBusinessModel> { request }
            };

            // 7. Call API
            var response = await _repository.PostAsync<object, LeaveApiResponse>(ApiEndpoints.CreateOfficialBusinessRequest, payload);

            if (response != null)
            {
                if (response.IsSuccess || response.Model != null)
                    return new SaveResult { Success = true };
                else
                    return new SaveResult { Success = false, ErrorMessage = response.ValidationMessage ?? "Submission failed." };
            }

            return new SaveResult { Success = false, ErrorMessage = "Server returned no response" };
        }
        catch (Exception ex)
        {
            return new SaveResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    // Stubs
    public Task<List<OfficialBusinessModel>> GetOfficialBusinessRequestsAsync() => Task.FromResult(new List<OfficialBusinessModel>());
    public Task<OfficialBusinessModel?> GetOfficialBusinessRequestByIdAsync(long id) => Task.FromResult<OfficialBusinessModel?>(null);
    public Task<SaveResult> SaveOfficialBusinessRequestAsync(OfficialBusinessModel request) => SubmitOfficialBusinessRequestAsync(request);
    public Task<bool> DeleteOfficialBusinessRequestAsync(long id) => Task.FromResult(false);
}


public class TimeEntryDataService : ITimeEntryDataService
{
    private readonly IGenericRepository _repository;

    public TimeEntryDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    // 1. History List
    public async Task<List<TimeEntryLogItem>> GetTimeEntriesAsync()
    {
        try
        {
            var profileIdStr = await SecureStorage.GetAsync("profile_id");
            if (string.IsNullOrEmpty(profileIdStr)) return new List<TimeEntryLogItem>();

            var startDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            var endDate = DateTime.Now.ToString("yyyy-MM-dd");

            var url = $"api/v1/timeentrylogs/my-timelogs?ProfileId={profileIdStr}&StartDate={startDate}&EndDate={endDate}&Page=1&Rows=50&SortOrder=desc";

            var response = await _repository.GetAsync<TimeEntryListResponse>(url);

            return response?.ListData ?? new List<TimeEntryLogItem>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetTimeEntries Error: {ex.Message}");
            return new List<TimeEntryLogItem>();
        }
    }

    // 2. Clock In / Out (Fixed)
    public async Task<SaveResult> CreateTimeEntryAsync(string type, double lat, double lng)
    {
        try
        {
            var profileIdStr = await SecureStorage.GetAsync("profile_id");
            long.TryParse(profileIdStr, out long pid);

            var request = new TimeEntryRequest
            {
                ProfileId = pid,
                TransactionDate = DateTime.UtcNow,
                Type = type, // "Time-In" ya "Time-Out"
                
                // Convert double to string
                Latitude = lat.ToString(),
                Longitude = lng.ToString(),
                
                Source = "Mobile",
                Remarks = "Mobile Punch"
            };

            // 🔥 FIX: Data ko Wrap karo (Yehi missing tha!)
            var payload = new { data = request };

            // API Call
            // Use LeaveApiResponse (Success/Model structure same hai)
            var response = await _repository.PostAsync<object, LeaveApiResponse>("api/v1/timeentrylogs", payload);
            
            if (response != null)
            {
                if (response.IsSuccess || response.Model != null)
                {
                    return new SaveResult { Success = true };
                }
                else
                {
                    // Error message extract karo
                    string error = response.ValidationMessage ?? "";
                    if (response.ValidationMessages != null && response.ValidationMessages.Any())
                        error = string.Join(", ", response.ValidationMessages);
                        
                    return new SaveResult { Success = false, ErrorMessage = error ?? "Punch failed." };
                }
            }

            return new SaveResult { Success = false, ErrorMessage = "Server returned null response" };
        }
        catch (Exception ex)
        {
            return new SaveResult { Success = false, ErrorMessage = ex.Message };
        }
    }
}

public class ApprovalDataService : IApprovalDataService
{
    private readonly IGenericRepository _repository;

    public ApprovalDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    // 1. LIST MANGWANA
    public async Task<List<MyApprovalListModel>> GetMyApprovalsAsync()
    {
        try
        {
            var profileIdStr = await SecureStorage.GetAsync("profile_id");
            long.TryParse(profileIdStr, out long pid);

            // Swagger Payload for List
            var payload = new
            {
                profileId = pid,
                status = "", // Empty means fetch all (Pending, Approved etc)
                page = 1,
                rows = 100,
                sortOrder = 0
            };

            // API Call
            var response = await _repository.PostAsync<object, ApprovalApiResponse>(ApiEndpoints.GetMyApprovals, payload);

            if (response != null && response.ListData != null)
            {
                // 🔥 MAPPING: API Data -> UI Model
                return response.ListData.Select(x => new MyApprovalListModel
                {
                    // IDs
                    RequestId = x.TransactionId, // UI uses RequestId
                    TransactionId = x.TransactionId,
                    TransactionTypeId = x.TransactionTypeId,
                    ProfileId = x.ProfileId,

                    // Text Info
                    RequestType = x.TransactionType, // UI uses RequestType
                    RequesterName = x.EmployeeName,  // UI uses RequesterName
                    Details = x.Details,
                    Status = x.Status,
                    
                    // Dates
                    DateFiled = x.DateFiled,
                    RequestDate = x.DateFiled,
                    
                    // Extra Parsing
                    StartDate = DateTime.TryParse(x.RequestedDate, out var d) ? d : null,
                    Reason = x.Details // Details ko hi Reason bana diya
                }).ToList();
            }

            return new List<MyApprovalListModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Approvals Error: {ex.Message}");
            return new List<MyApprovalListModel>();
        }
    }

    // 2. APPROVE
    public async Task<SaveResult> ApproveRequestAsync(long requestId, string comments)
    {
        // ActionTriggeredId = 1 (Usually Approve)
        return await ProcessTransaction(requestId, 1, comments);
    }

    // 3. DISAPPROVE
    public async Task<SaveResult> DisapproveRequestAsync(long requestId, string comments)
    {
        // ActionTriggeredId = 3 (Usually Disapprove/Reject)
        return await ProcessTransaction(requestId, 3, comments);
    }

    // --- Helper: Process Workflow ---
    private async Task<SaveResult> ProcessTransaction(long transactionId, int actionId, string remarks)
    {
        try
        {
            var profileIdStr = await SecureStorage.GetAsync("profile_id");
            long.TryParse(profileIdStr, out long approverId);

            // Swagger Payload for Process
            var payload = new
            {
                transactionId = transactionId,
                actionTriggeredId = actionId, // 1=Approve, 3=Disapprove
                approverId = approverId,
                remarks = remarks,
                
                // Defaults required by API
                transactionTypeId = 0, // Server usually finds this by ID
                stageId = 0,
                userAccessId = 0
            };

            // API Call
            var response = await _repository.PostAsync<object, LeaveApiResponse>(ApiEndpoints.ProcessWorkflow, payload);

            if (response != null)
            {
                if (response.IsSuccess)
                    return new SaveResult { Success = true };
                else
                    return new SaveResult { Success = false, ErrorMessage = response.ValidationMessage ?? "Action failed." };
            }

            return new SaveResult { Success = false, ErrorMessage = "Server returned no response" };
        }
        catch (Exception ex)
        {
            return new SaveResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    // --- Interface Stub (Not used but required by Interface) ---
    public Task<List<MyApprovalListModel>> GetPendingApprovalsAsync() => GetMyApprovalsAsync();
    public Task<MyApprovalListModel?> GetApprovalByIdAsync(long id) => Task.FromResult<MyApprovalListModel?>(null);
}

// --- Helper Class for JSON Response ---
public class ApprovalApiResponse
{
    public List<MyApprovalListModel>? ListData { get; set; }
    public bool IsSuccess { get; set; }
}
public class AttendanceDataService : IAttendanceDataService
{
    private readonly IGenericRepository _repository;

    public AttendanceDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AttendanceRecordModel>> GetAttendanceRecordsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var endpoint = $"{ApiEndpoints.GetAttendanceRecords}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            var response = await _repository.GetAsync<List<AttendanceRecordModel>>(endpoint);
            return response ?? GetSampleAttendanceRecords(startDate, endDate);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting attendance records: {ex.Message}");
            return GetSampleAttendanceRecords(startDate, endDate);
        }
    }

    public async Task<AttendanceSummaryModel?> GetAttendanceSummaryAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var endpoint = $"{ApiEndpoints.GetAttendanceSummary}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            return await _repository.GetAsync<AttendanceSummaryModel>(endpoint);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting attendance summary: {ex.Message}");
            return null;
        }
    }

    private static List<AttendanceRecordModel> GetSampleAttendanceRecords(DateTime startDate, DateTime endDate)
    {
        var records = new List<AttendanceRecordModel>();
        var current = startDate;
        var random = new Random();

        while (current <= endDate)
        {
            // Skip weekends
            if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
            {
                var timeIn = current.Date.AddHours(8).AddMinutes(random.Next(-15, 30));
                var timeOut = timeIn.AddHours(8).AddMinutes(random.Next(0, 60));
                var totalHours = (decimal)(timeOut - timeIn).TotalHours;

                var status = timeIn.TimeOfDay > new TimeSpan(8, 15, 0) ? "Late" : "Present";

                records.Add(new AttendanceRecordModel
                {
                    Date = current,
                    TimeIn = timeIn,
                    TimeOut = timeOut,
                    TotalHours = Math.Round(totalHours, 1),
                    Status = status,
                    Location = "Office",
                    Remarks = status == "Late" ? "Traffic delay" : null
                });
            }
            current = current.AddDays(1);
        }

        return records.OrderByDescending(r => r.Date).ToList();
    }
}

public class UserDataService : IUserDataService
{
    private readonly IGenericRepository _repository;

    public UserDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserProfileModel?> GetUserProfileAsync()
    {
        try
        {
            var profileIdStr = await SecureStorage.GetAsync("profile_id");
            if (string.IsNullOrEmpty(profileIdStr)) return null;

            // URL format karo
            var url = string.Format(ApiEndpoints.GetUserProfile, profileIdStr);

            // API se Bara Model (ProfileModel) mangwao
            var response = await _repository.GetAsync<EmployeeProfileResponse>(url);

            if (response != null && response.Model != null)
            {
                var apiData = response.Model;

                // 🔥 FIXED MAPPING (Ab koi error nahi ayega)
                return new UserProfileModel
                {
                    // 1. Profile Id (Added in Model)
                    ProfileId = apiData.ProfileId,
                    
                    // 2. Employee Info
                    EmployeeId = apiData.EmployeeNo ?? "N/A",
                    
                    // 3. Name (UI model mein First/Last nahi hai, sirf FullName hai)
                    FullName = $"{apiData.FirstName} {apiData.LastName}".Trim(),
                    
                    // 4. Contact
                    Email = apiData.EmailAddress ?? "",
                    PhoneNumber = apiData.MobileNumber ?? apiData.PhoneNumber,

                    // 5. Dates
                    DateOfBirth = apiData.Birthdate ?? DateTime.MinValue,
                    HireDate = apiData.HireDate ?? DateTime.MinValue,
                    
                    // 6. Job
                    JobTitle = apiData?.Position ?? "N/A", 
                    Department = apiData?.Department ?? "N/A",
                    WorkLocation = apiData?.Location ?? apiData?.Branch ?? "N/A",
                    EmploymentStatus = apiData?.EmploymentStatus ?? "N/A",
                    
                    // 7. Address
                    Address = $"{apiData?.CityAddress1} {apiData?.CityAddressCity}".Trim(),
                    
                    // 8. Emergency
                    EmergencyContactName = apiData?.EmergencyContactName,
                    EmergencyContactRelationship = apiData?.EmergencyContactRelationship,
                    EmergencyContactPhone = apiData?.EmergencyContactContactNumber
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Profile Error: {ex.Message}");
            return null;
        }
    }

    public async Task<SaveResult> UpdateUserProfileAsync(UserProfileModel profile)
    {
        // Fake update for now
        await Task.Delay(500);
        return new SaveResult { Success = true };
    }
}
public class PayrollDataService : IPayrollDataService
{
    private readonly IGenericRepository _repository;

    public PayrollDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<object>> GetPayslipsAsync()
    {
        await Task.Delay(100);
        return new List<object>();
    }

    public async Task<object> GetPayslipDetailAsync(long id)
    {
        await Task.Delay(100);
        return new { };
    }
}

public class PerformanceDataService : IPerformanceDataService
{
    private readonly IGenericRepository _repository;

    public PerformanceDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<object>> GetPerformanceEvaluationsAsync()
    {
        await Task.Delay(100);
        return new List<object>();
    }

    public async Task<object> GetIndividualObjectivesAsync()
    {
        await Task.Delay(100);
        return new { };
    }
}

public class EmployeeRelationsDataService : IEmployeeRelationsDataService
{
    private readonly IGenericRepository _repository;

    public EmployeeRelationsDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<object>> GetSuggestionsAsync()
    {
        await Task.Delay(100);
        return new List<object>();
    }

    public async Task<object> SubmitSuggestionAsync(object suggestion)
    {
        await Task.Delay(100);
        return new { };
    }
}

public class ProfileDataService : IProfileDataService
{
    private readonly IGenericRepository _repository;

    public ProfileDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> GetProfileAsync()
    {
        await Task.Delay(100);
        return new { };
    }

    public async Task<object> UpdateProfileAsync(object profile)
    {
        await Task.Delay(100);
        return new { };
    }
}

public class NotificationDataService : INotificationDataService
{
    private readonly IGenericRepository _repository;

    public NotificationDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<object>> GetNotificationsAsync()
    {
        await Task.Delay(100);
        return new List<object>();
    }

    public Task MarkAsReadAsync(long notificationId)
    {
        return Task.CompletedTask;
    }
}

public class CommonDataService : ICommonDataService
{
    private readonly IGenericRepository _repository;

    public CommonDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> GetAppSettingsAsync()
    {
        await Task.Delay(100);
        return new { };
    }
}

public class SignalRDataService : ISignalRDataService
{
    private HubConnection? _hubConnection;
    public event Action<string>? OnNotificationReceived;

    public async Task StartConnectionAsync()
    {
        try
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://api.everythingatwork.com/notificationHub")
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string>("ReceiveNotification", (message) =>
            {
                OnNotificationReceived?.Invoke(message);
            });

            await _hubConnection.StartAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SignalR connection error: {ex.Message}");
        }
    }

    public async Task StopConnectionAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
        }
    }
}

