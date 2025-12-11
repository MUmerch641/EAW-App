using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class ScheduleDataService : IScheduleDataService
    {
        private readonly IGenericRepository _repository;

        public ScheduleDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MyScheduleListModel>> RetrieveMyScheduleListAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);

                // Construct URL with query parameters
                var url = $"{ApiEndpoints.MySchedule}?ProfileId={pid}&StartDate={startDate:yyyy-MM-dd}&EndDate={endDate:yyyy-MM-dd}&Page=1&Rows=100&SortOrder=0";

                var response = await _repository.GetAsync<ScheduleListResponseWrapper>(url);
                return response?.ListData ?? new List<MyScheduleListModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RetrieveMyScheduleListAsync Error: {ex.Message}");
                return new List<MyScheduleListModel>();
            }
        }

        public async Task<bool> SubmitWorkScheduleChangeAsync(ChangeWorkScheduleRequest request)
        {
            try
            {
                 var profileIdStr = await SecureStorage.GetAsync("profile_id");
                 request.ProfileId = profileIdStr;

                 var payload = new { data = request };
                 var response = await _repository.PostAsync<object, LeaveApiResponse>(ApiEndpoints.SubmitChangeWorkScheduleRequest, payload);
                 
                 return response != null && (response.IsSuccess || response.Model != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SubmitWorkScheduleChangeAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SubmitRestDayChangeAsync(ChangeRestDayRequest request)
        {
            try
            {
                 var profileIdStr = await SecureStorage.GetAsync("profile_id");
                 request.ProfileId = profileIdStr;

                 var payload = new { data = request };
                 var response = await _repository.PostAsync<object, LeaveApiResponse>(ApiEndpoints.SubmitChangeRestdayRequest, payload);
                 
                 return response != null && (response.IsSuccess || response.Model != null || string.IsNullOrEmpty(response.ValidationMessage));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SubmitRestDayChangeAsync Error: {ex.Message}");
                return false;
            }
        }
    }

    public class ScheduleListResponseWrapper
    {
        public List<MyScheduleListModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
    }
}
