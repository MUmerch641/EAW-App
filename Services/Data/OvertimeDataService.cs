using MauiHybridApp.Models;
using MauiHybridApp.Models.Schedule;
using MauiHybridApp.Utils;
using MauiHybridApp.Services.Data;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
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
                request.DateFiled = DateTime.Now;
                
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
                    if (response.IsSuccess || response.Model != null || string.IsNullOrEmpty(response.ValidationMessage))
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
        public async Task<List<OvertimeModel>> GetOvertimeRequestsAsync()
        {
             try
            {
                // API v1 returns a Paginated Wrapper, not a direct List
                var response = await _repository.GetAsync<PaginatedResponse<OvertimeModel>>(ApiEndpoints.GetOvertimeRequests);
                if (response != null && response.Data != null)
                {
                    return response.Data;
                }
                
                // Fallback: If deserialization matched but Data is null, return empty
                return new List<OvertimeModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting overtime requests: {ex.Message}");
                return new List<OvertimeModel>();
            }
        }

        public async Task<List<OvertimeModel>> GetOvertimeHistoryAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Reusing the LeaveRequests endpoint because it likely returns history
                // Ideally this would be a filtered endpoint like /api/leave/history?start=...&end=...
                // But for now we fetch all and filter client-side if needed, OR relies on the endpoint returning "My Leaves"
                
                // Note: If optimal API exists, swap this logic.
                var response = await _repository.GetAsync<PaginatedResponse<OvertimeModel>>(ApiEndpoints.GetOvertimeRequests);
                var allOvertime = (response?.Data) ?? new List<OvertimeModel>();
                
                // Filter by date range
                return allOvertime.Where(x => 
                    x.OvertimeDate >= startDate &&
                    x.OvertimeDate <= endDate
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting overtime history: {ex.Message}");
                return new List<OvertimeModel>();
            }
        }

        public Task<OvertimeModel?> GetOvertimeRequestByIdAsync(long id) => Task.FromResult<OvertimeModel?>(null);
        public Task<SaveResult> SaveOvertimeRequestAsync(OvertimeModel request) => SubmitOvertimeRequestAsync(request);
        public Task<bool> DeleteOvertimeRequestAsync(long id) => Task.FromResult(false);
        public Task<OvertimeSummary> CalculateOvertimeSummaryAsync(DateTime d, DateTime s, DateTime e) => Task.FromResult(new OvertimeSummary());
    }
}
