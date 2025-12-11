using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class SpecialWorkScheduleDataService : ISpecialWorkScheduleDataService
    {
        private readonly IGenericRepository _repository;

        public SpecialWorkScheduleDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SpecialWorkScheduleListModel>> GetSpecialWorkScheduleRequestsAsync()
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);

                var url = $"{ApiEndpoints.BaseApiUrl}/api/specialworkschedule/list?ProfileId={pid}&Page=1&Rows=100&SortOrder=0";
                var response = await _repository.GetAsync<SpecialWorkScheduleListResponseWrapper>(url);
                return response?.ListData ?? new List<SpecialWorkScheduleListModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetSpecialWorkScheduleRequestsAsync Error: {ex.Message}");
                return new List<SpecialWorkScheduleListModel>();
            }
        }

        public async Task<List<ShiftModel>> GetShiftsAsync()
        {
            try
            {
                var url = $"{ApiEndpoints.BaseApiUrl}/api/shift/list";
                var response = await _repository.GetAsync<ShiftListResponseWrapper>(url);
                var list = response?.ListData ?? new List<ShiftModel>();
                
                // Add "Others" option manually as seen in Xamarin code
                list.Add(new ShiftModel { ShiftId = -1, Code = "Others", WorkSchedule = "Custom Schedule" });
                
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetShiftsAsync Error: {ex.Message}");
                return new List<ShiftModel>();
            }
        }

        public async Task<ShiftModel> GetShiftByIdAsync(long id)
        {
            try
            {
                var url = $"{ApiEndpoints.BaseApiUrl}/api/shift/{id}";
                var response = await _repository.GetAsync<ShiftDetailResponseWrapper>(url);
                return response?.Shift;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetShiftByIdAsync Error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SubmitSpecialWorkScheduleRequestAsync(SpecialWorkScheduleRequestModel request)
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);
                request.ProfileId = pid;
                request.SourceId = 2; // Mobile

                var payload = new { data = request };
                var response = await _repository.PostAsync<object, LeaveApiResponse>($"{ApiEndpoints.BaseApiUrl}/api/specialworkschedule", payload);

                return response != null && (response.IsSuccess || response.Model != null || string.IsNullOrEmpty(response.ValidationMessage));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SubmitSpecialWorkScheduleRequestAsync Error: {ex.Message}");
                return false;
            }
        }
    }

    public class SpecialWorkScheduleListResponseWrapper
    {
        public List<SpecialWorkScheduleListModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class ShiftListResponseWrapper
    {
        public List<ShiftModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class ShiftDetailResponseWrapper
    {
        public ShiftModel Shift { get; set; }
        public bool IsSuccess { get; set; }
    }
}
