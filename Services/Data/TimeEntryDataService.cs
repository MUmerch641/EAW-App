using MauiHybridApp.Models;
using MauiHybridApp.Models.Attendance;
using MauiHybridApp.Utils;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Services.Data;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class TimeEntryDataService : ITimeEntryDataService
    {
        private readonly IGenericRepository _repository;

        public TimeEntryDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        // 1. History List
        public async Task<List<TimeEntryLogItem>> GetTimeEntriesAsync(DateTime? startDate = null, DateTime? endDate = null, string? status = null)
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                if (string.IsNullOrEmpty(profileIdStr)) return new List<TimeEntryLogItem>();

                // Default to last 30 days if not specified
                var startStr = (startDate ?? DateTime.Now.AddDays(-30)).ToString("yyyy-MM-dd");
                var endStr = (endDate ?? DateTime.Now).ToString("yyyy-MM-dd");

                var url = $"{ApiEndpoints.GetTimeEntries}?ProfileId={profileIdStr}&StartDate={startStr}&EndDate={endStr}&Page=1&Rows=50&SortOrder=1";

                var response = await _repository.GetAsync<TimeEntryListResponse>(url);
                var list = response?.ListData ?? new List<TimeEntryLogItem>();

                // Client-side status filtering (if API doesn't support it directly in this endpoint)
                if (!string.IsNullOrEmpty(status))
                {
                    // Assuming 'Status' or similar property exists on TimeEntryLogItem. 
                    // If status param is comma separated "Pending,Approved", we filter by that.
                    var statuses = status.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().ToLower()).ToList();
                    
                    if (statuses.Any())
                    {
                        list = list.Where(x => !string.IsNullOrEmpty(x.Status) && statuses.Contains(x.Status.ToLower())).ToList();
                    }
                }

                return list;
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
                var empNo = await SecureStorage.GetAsync("employee_number");
                var accessId = await SecureStorage.GetAsync("access_id");

                var request = new TimeEntryLogModel
                {
                    ProfileId = pid,
                    TimeEntry = DateTime.Now, // Use Device Time to avoid timezone confusion for user
                    Type = type, // "Time-In" ya "Time-Out"
                    Latitude = lat.ToString(),
                    Longitude = lng.ToString(),
                    Source = "Mobile",
                    Remark = "Mobile Punch",
                    StatusId = 0
                };

                // Use the correct payload structure for api/onlinetimeentry
                var payload = new 
                {
                    Data = request,
                    AccessId = accessId,
                    EmployeeNo = empNo,
                    UserImage = "", // Image capture logic to be added later if needed
                    IsLoggedIn = true
                };

                // API Call
                var response = await _repository.PostAsync<object, OnlineTimeEntryHolder>(ApiEndpoints.SubmitOnlineTimeEntryRequest, payload);
                
                if (response != null)
                {
                    if (response.IsSuccess)
                    {
                        return new SaveResult { Success = true, ErrorMessage = response.ResponseMesage };
                    }
                    else
                    {
                        string error = response.ResponseMesage;
                        if (string.IsNullOrEmpty(error) && response.UserErrorList != null && response.UserErrorList.Any())
                            error = string.Join(", ", response.UserErrorList);
                            
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

        public async Task<OnlineTimeEntryHolder> InitFormAsync()
        {
            try
            {
                var url = ApiEndpoints.GetOnlineTimeEntryFormHelper;
                var response = await _repository.GetAsync<OnlineTimeEntryInitFormResponse>(url);

                var retValue = new OnlineTimeEntryHolder();

                if (response != null)
                {
                    retValue.HasSetup = response.HasSetup;
                    retValue.UserError = response.UserError;
                    
                    if (response.HasSetup)
                    {
                        retValue.TimeClock = response.ServerTime;
                        retValue.ClockworkConfiguration = response.ClockworkConfigurationModel;
                        
                        // Map colors and settings
                        if (retValue.ClockworkConfiguration != null)
                        {
                            retValue.TimeInColor = retValue.ClockworkConfiguration.TimeInColor;
                            retValue.TimeOutColor = retValue.ClockworkConfiguration.TimeOutColor;
                            retValue.BreakInColor = retValue.ClockworkConfiguration.BreakInColor;
                            retValue.BreakOutColor = retValue.ClockworkConfiguration.BreakOutColor;
                            retValue.AllowImageCapture = retValue.ClockworkConfiguration.AllowImageCapture;
                            retValue.AllowLocationCapture = retValue.ClockworkConfiguration.AllowLocationCapture;
                        }
                    }
                }
                
                return retValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InitFormAsync Error: {ex.Message}");
                return new OnlineTimeEntryHolder { UserError = ex.Message };
            }
        }
    }
}
