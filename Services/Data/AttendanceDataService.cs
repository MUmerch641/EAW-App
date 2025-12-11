using MauiHybridApp.Models;
using MauiHybridApp.Models.Attendance;
using MauiHybridApp.Utils;
using MauiHybridApp.Services.Data;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
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
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);

                var request = new MyApprovalRequest
                {
                    ProfileId = pid,
                    Page = 1,
                    Rows = 100,
                    SortOrder = 1, // Descending
                    StartDate = startDate.ToString("yyyy-MM-dd"),
                    EndDate = endDate.ToString("yyyy-MM-dd"),
                    Status = "",
                    Keyword = "",
                    TransactionTypes = ""
                };

                // Use the correct endpoint for Attendance History
                // Assuming it's similar to TimeEntry or a specific report endpoint
                // For now, let's try to use the TimeEntry endpoint but mapped to AttendanceRecordModel
                // OR if there is a specific endpoint for "My Attendance"
                
                // Let's use the TimeEntry endpoint as it contains the raw logs
                var url = $"{ApiEndpoints.GetTimeEntries}?ProfileId={pid}&StartDate={startDate:yyyy-MM-dd}&EndDate={endDate:yyyy-MM-dd}&Page=1&Rows=100&SortOrder=1";
                var response = await _repository.GetAsync<TimeEntryListResponse>(url);

                if (response?.ListData != null)
                {
                    return response.ListData.Select(x => new AttendanceRecordModel
                    {
                        Date = x.TimeEntry.Date,
                        TimeIn = x.Type == "Time-In" ? x.TimeEntry : (DateTime?)null,
                        TimeOut = x.Type == "Time-Out" ? x.TimeEntry : (DateTime?)null,
                        Status = x.Status,
                        Remarks = "" // Remark not available in TimeEntryLogItem
                    }).ToList();
                }

                return new List<AttendanceRecordModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Attendance Error: {ex.Message}");
                return new List<AttendanceRecordModel>();
            }
        }

        public async Task<AttendanceSummaryModel?> GetAttendanceSummaryAsync(DateTime startDate, DateTime endDate)
        {
            // Stub implementation - Calculate from records if needed
            var records = await GetAttendanceRecordsAsync(startDate, endDate);
            return new AttendanceSummaryModel
            {
                PresentDays = records.Count(r => r.Status == "Present"),
                AbsentDays = records.Count(r => r.Status == "Absent"),
                LateDays = 0,
                UndertimeDays = 0
            };
        }
    }
}
