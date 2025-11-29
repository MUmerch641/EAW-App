using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MauiHybridApp.Models.Attendance;
using MauiHybridApp.Services.Data;
using AttendanceRecordModel = MauiHybridApp.Services.Data.AttendanceRecordModel;
using AttendanceSummaryModel = MauiHybridApp.Services.Data.AttendanceSummaryModel;

namespace MauiHybridApp.Components.Pages;

public partial class Attendance : ComponentBase
{
    private List<AttendanceRecordModel>? attendanceRecords;
    private AttendanceSummaryModel? attendanceSummary;
    
    private bool isLoading = true;
    private string errorMessage = string.Empty;
    private string selectedPeriod = "month";
    
    private DateTime startDate = DateTime.Now.AddDays(-30);
    private DateTime endDate = DateTime.Now;

    protected override async Task OnInitializedAsync()
    {
        await SetQuickFilter("month");
        await LoadAttendanceData();
    }

    private async Task LoadAttendanceData()
    {
        try
        {
            isLoading = true;
            StateHasChanged();

            // Load attendance records for the selected period
            attendanceRecords = await AttendanceService.GetAttendanceRecordsAsync(startDate, endDate);
            
            // Calculate summary
            CalculateAttendanceSummary();

            errorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            errorMessage = $"Error loading attendance data: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private void CalculateAttendanceSummary()
    {
        if (attendanceRecords == null || !attendanceRecords.Any())
        {
            attendanceSummary = new AttendanceSummaryModel();
            return;
        }

        var totalDays = (endDate - startDate).Days + 1;
        var workingDays = GetWorkingDaysCount(startDate, endDate);
        
        var presentDays = attendanceRecords.Count(r => r.Status?.ToLower() == "present");
        var absentDays = workingDays - attendanceRecords.Count;
        var lateDays = attendanceRecords.Count(r => r.Status?.ToLower() == "late");
        
        var totalHours = attendanceRecords.Where(r => r.TotalHours.HasValue).Sum(r => r.TotalHours!.Value);
        var averageHours = attendanceRecords.Any() ? totalHours / attendanceRecords.Count : 0;

        attendanceSummary = new AttendanceSummaryModel
        {
            TotalDays = workingDays,
            PresentDays = presentDays,
            AbsentDays = absentDays,
            LateDays = lateDays,
            TotalHours = totalHours,
            AverageHoursPerDay = averageHours
        };
    }

    private static int GetWorkingDaysCount(DateTime start, DateTime end)
    {
        var count = 0;
        var current = start;
        
        while (current <= end)
        {
            if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
            {
                count++;
            }
            current = current.AddDays(1);
        }
        
        return count;
    }

    private async Task SetQuickFilter(string period)
    {
        selectedPeriod = period;
        
        var now = DateTime.Now;
        switch (period)
        {
            case "today":
                startDate = now.Date;
                endDate = now.Date;
                break;
            case "week":
                var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
                startDate = startOfWeek.Date;
                endDate = now.Date;
                break;
            case "month":
                startDate = new DateTime(now.Year, now.Month, 1);
                endDate = now.Date;
                break;
        }
        
        await LoadAttendanceData();
    }

    private async Task OnDateFilterChanged()
    {
        selectedPeriod = "custom";
        await LoadAttendanceData();
    }

    private async Task RefreshAttendance()
    {
        await LoadAttendanceData();
    }

    private async Task GoBack()
    {
        await JSRuntime.InvokeVoidAsync("history.back");
    }

    private async Task OnStartDateChanged(ChangeEventArgs e)
    {
        if (DateTime.TryParse(e.Value?.ToString(), out var newDate))
        {
            startDate = newDate;
            await OnDateFilterChanged();
        }
    }

    private async Task OnEndDateChanged(ChangeEventArgs e)
    {
        if (DateTime.TryParse(e.Value?.ToString(), out var newDate))
        {
            endDate = newDate;
            await OnDateFilterChanged();
        }
    }
}
