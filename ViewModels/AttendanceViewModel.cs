using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models.Attendance;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

/// <summary>
/// ViewModel for Attendance page
/// </summary>
public class AttendanceViewModel : BaseViewModel
{
    private readonly IAttendanceDataService _attendanceService;
    private readonly NavigationManager _navigationManager;
    
    private ObservableCollection<AttendanceRecordModel> _attendanceRecords;
    private AttendanceSummaryModel? _attendanceSummary;
    private string _selectedPeriod;
    private DateTime _startDate;
    private DateTime _endDate;

    public AttendanceViewModel(
        IAttendanceDataService attendanceService,
        NavigationManager navigationManager)
    {
        _attendanceService = attendanceService ?? throw new ArgumentNullException(nameof(attendanceService));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        
        _attendanceRecords = new ObservableCollection<AttendanceRecordModel>();
        _selectedPeriod = "month";
        _startDate = DateTime.Now.AddDays(-30);
        _endDate = DateTime.Now;
        
        SetQuickFilterCommand = new AsyncRelayCommand<string>(SetQuickFilterAsync);
        RefreshCommand = new AsyncRelayCommand(LoadAttendanceDataAsync);
        GoBackCommand = new RelayCommand(GoBack);
    }

    #region Properties

    public ObservableCollection<AttendanceRecordModel> AttendanceRecords
    {
        get => _attendanceRecords;
        private set => SetProperty(ref _attendanceRecords, value);
    }

    public AttendanceSummaryModel? AttendanceSummary
    {
        get => _attendanceSummary;
        private set => SetProperty(ref _attendanceSummary, value);
    }

    public string SelectedPeriod
    {
        get => _selectedPeriod;
        set => SetProperty(ref _selectedPeriod, value);
    }

    public DateTime StartDate
    {
        get => _startDate;
        set
        {
            if (SetProperty(ref _startDate, value))
            {
                SelectedPeriod = "custom";
                _ = LoadAttendanceDataAsync();
            }
        }
    }

    public DateTime EndDate
    {
        get => _endDate;
        set
        {
            if (SetProperty(ref _endDate, value))
            {
                SelectedPeriod = "custom";
                _ = LoadAttendanceDataAsync();
            }
        }
    }

    public bool HasRecords => AttendanceRecords.Any();
    public bool ShowEmptyState => !IsBusy && !HasRecords;

    #endregion

    #region Commands

    public ICommand SetQuickFilterCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand GoBackCommand { get; }

    #endregion

    #region Methods

    public override async Task InitializeAsync()
    {
        await SetQuickFilterAsync("month");
    }

    private async Task LoadAttendanceDataAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            try
            {
                var records = await _attendanceService.GetAttendanceRecordsAsync(StartDate, EndDate);
                AttendanceRecords = new ObservableCollection<AttendanceRecordModel>(records ?? new List<AttendanceRecordModel>());
                
                CalculateAttendanceSummary();
                ClearError();
            }
            catch (Exception ex)
            {
                HandleError(ex, "Error loading attendance data");
            }
        }, "Loading attendance...");
    }

    private void CalculateAttendanceSummary()
    {
        if (!AttendanceRecords.Any())
        {
            AttendanceSummary = new AttendanceSummaryModel();
            return;
        }

        var workingDays = GetWorkingDaysCount(StartDate, EndDate);
        var presentDays = AttendanceRecords.Count(r => r.Status?.ToLower() == "present");
        var absentDays = workingDays - AttendanceRecords.Count;
        var lateDays = AttendanceRecords.Count(r => r.Status?.ToLower() == "late");
        
        var totalHours = AttendanceRecords.Where(r => r.TotalHours.HasValue).Sum(r => r.TotalHours!.Value);
        var averageHours = AttendanceRecords.Any() ? totalHours / AttendanceRecords.Count : 0;

        AttendanceSummary = new AttendanceSummaryModel
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

    private async Task SetQuickFilterAsync(string? period)
    {
        if (string.IsNullOrEmpty(period)) return;

        SelectedPeriod = period;
        var now = DateTime.Now;
        
        switch (period)
        {
            case "today":
                StartDate = now.Date;
                EndDate = now.Date;
                break;
            case "week":
                var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
                StartDate = startOfWeek.Date;
                EndDate = now.Date;
                break;
            case "month":
                StartDate = new DateTime(now.Year, now.Month, 1);
                EndDate = now.Date;
                break;
        }
        
        await LoadAttendanceDataAsync();
    }

    private void GoBack()
    {
        _navigationManager.NavigateTo("/dashboard");
    }

    #endregion
}
