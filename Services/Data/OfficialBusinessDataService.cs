using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
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
                request.DateFiled = DateTime.Now;
                
                // UI Dates (Safe handling)
                var sDate = request.StartDate ?? DateTime.Now.Date;
                var eDate = request.EndDate ?? DateTime.Now.Date;
                
                request.OfficialBusinessDate = sDate;

                // 3. Time Sync (Fixing the Error CS1061)
                // If StartTime is null, default to 9 AM
                TimeSpan startTimePart = request.StartTime.HasValue 
                    ? request.StartTime.Value.TimeOfDay 
                    : new TimeSpan(9, 0, 0);

                // If EndTime is null, default to 5 PM
                TimeSpan endTimePart = request.EndTime.HasValue 
                    ? request.EndTime.Value.TimeOfDay 
                    : new TimeSpan(17, 0, 0);

                // Combine Date and Time
                request.StartTime = sDate.Date.Add(startTimePart);
                request.EndTime = eDate.Date.Add(endTimePart);
                
                // 4. Duration Calculation (Safe Math)
                // Since we assigned a value above, we can safely use .Value
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
                    if (response.IsSuccess || response.Model != null || string.IsNullOrEmpty(response.ValidationMessage))
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
}
