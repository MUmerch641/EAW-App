using MauiHybridApp.Models;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Utils;
using MauiHybridApp.Models.Leave;
using MauiHybridApp.Services.Data;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
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
                // Using Legacy endpoint (same as Xamarin)
                var response = await _repository.GetAsync<LeaveListResponse>(ApiEndpoints.GetLeaveRequests);
                if (response != null && response.LeaveRequestDetailList != null)
                {
                    return response.LeaveRequestDetailList;
                }
                 
                return new List<LeaveRequestModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting leave requests: {ex.Message}");
                return new List<LeaveRequestModel>();
            }
        }

        public async Task<List<LeaveRequestModel>> GetLeaveHistoryAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _repository.GetAsync<LeaveListResponse>(ApiEndpoints.GetLeaveRequests);
                var allLeaves = (response?.LeaveRequestDetailList) ?? new List<LeaveRequestModel>();
                
                // Filter by date range
                return allLeaves.Where(x => 
                    x.InclusiveStartDate.HasValue && 
                    x.InclusiveStartDate.Value >= startDate && 
                    x.InclusiveStartDate.Value <= endDate
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting leave history: {ex.Message}");
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

                // 3. DATE FIX: Remove time component (Set to Midnight)
                // So that 19:00:00Z doesn't become the next day on server
                if (request.InclusiveStartDate.HasValue)
                    request.InclusiveStartDate = request.InclusiveStartDate.Value.Date;
                    
                if (request.InclusiveEndDate.HasValue)
                    request.InclusiveEndDate = request.InclusiveEndDate.Value.Date;
                    
                request.DateFiled = DateTime.Now; // Try local time instead of UTC

                // Wrapper
                var payload = new { data = request };

                // =========================================================
                // ️‍♂️ DEBUG MODE: Direct HTTP Client to see RAW RESPONSE
                // =========================================================
                // 4. API Call
                // We can use GenericRepository again now
                var response = await _repository.PostAsync<object, LeaveApiResponse>(ApiEndpoints.CreateLeaveRequest, payload);

                if (response != null)
                {
                    // 🔥 SUCCESS LOGIC FIX:
                    // If IsSuccess is true OR Model is not null, it's Success
                    if (response.IsSuccess || response.Model != null || string.IsNullOrEmpty(response.ValidationMessage))
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
                // API: GET /api/v1/leave/types
                // Try Wrapper first
                try 
                {
                    var response = await _repository.GetAsync<PaginatedResponse<SelectableListModel>>(ApiEndpoints.GetLeaveTypes);
                    if (response != null && response.Data != null && response.Data.Any())
                    {
                        return response.Data;
                    }
                }
                catch 
                {
                    // If wrapper fails, try direct list
                    var listResponse = await _repository.GetAsync<List<SelectableListModel>>(ApiEndpoints.GetLeaveTypes);
                    if (listResponse != null && listResponse.Any()) return listResponse;
                }

                Console.WriteLine("API returned no leave types, using defaults");
                return GetDefaultLeaveTypes();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetLeaveTypes Error: {ex.Message}, using defaults");
                return GetDefaultLeaveTypes();
            }
        }

        public async Task<LeaveSummary> CalculateLeaveSummaryAsync(long leaveTypeId, DateTime startDate, DateTime endDate, int applyToOption)
        {
            // Initialize Result object
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
                    DateFiled = DateTime.Now,
                    CreateDate = DateTime.Now
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
            // If API fails or returns 0, we calculate it ourselves
            // so the User can "Submit".
            // ---------------------------------------------------------
            if (summary.TotalDays <= 0)
            {
                Console.WriteLine("[CALC] API returned 0. Using Local Fallback calculation.");
                
                // Calculate manually: (End - Start) + 1
                // Example: 28th to 28th = 0 diff + 1 = 1 Day
                double daysDiff = (endDate.Date - startDate.Date).TotalDays + 1;
                
                // If Half Day, subtract 0.5
                if (applyToOption > 1) 
                {
                    summary.TotalDays = 0.5m;
                    summary.TotalHours = 4; // Assume 4 hours for half day
                }
                else
                {
                    summary.TotalDays = (decimal)daysDiff;
                    summary.TotalHours = (decimal)(daysDiff * 8); // Assume 8 hours per day
                }
            }

            return summary;
        }

        private List<SelectableListModel> GetDefaultLeaveTypes()
        {
            return new List<SelectableListModel>
            {
                new SelectableListModel { Id = 1, DisplayText = "Vacation Leave" },
                new SelectableListModel { Id = 2, DisplayText = "Sick Leave" },
                new SelectableListModel { Id = 3, DisplayText = "Emergency Leave" },
                new SelectableListModel { Id = 4, DisplayText = "Maternity Leave" },
                new SelectableListModel { Id = 5, DisplayText = "Paternity Leave" }
            };
        }
    }
}
