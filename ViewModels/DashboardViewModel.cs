using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

/// <summary>
/// ViewModel for Dashboard page following proper MVVM pattern
/// Handles all business logic, data fetching, and navigation
/// Keeps the View (Dashboard.razor) clean and focused on presentation
/// </summary>
public class DashboardViewModel : BaseViewModel
{
    private readonly IDashboardDataService _dashboardService;
    private readonly NavigationManager _navigationManager;
    
    private DashboardResponse _dashboardData;
    private string _userGreeting = string.Empty;
    private string _currentDate = string.Empty;

    public DashboardViewModel(
        IDashboardDataService dashboardService,
        NavigationManager navigationManager)
    {
        _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        
        _dashboardData = new DashboardResponse();
        
        // Initialize commands
        NavigateToLeaveCommand = new RelayCommand(NavigateToLeave);
        NavigateToTimeEntryCommand = new RelayCommand(NavigateToTimeEntry);
        NavigateToOvertimeCommand = new RelayCommand(NavigateToOvertime);
        NavigateToAttendanceCommand = new RelayCommand(NavigateToAttendance);
        RefreshCommand = new AsyncRelayCommand(LoadDashboardDataAsync);
    }

    #region Properties

    /// <summary>
    /// Dashboard data from API
    /// </summary>
    public DashboardResponse DashboardData
    {
        get => _dashboardData;
        private set => SetProperty(ref _dashboardData, value);
    }

    /// <summary>
    /// Personalized greeting message
    /// </summary>
    public string UserGreeting
    {
        get => _userGreeting;
        private set => SetProperty(ref _userGreeting, value);
    }

    /// <summary>
    /// Formatted current date
    /// </summary>
    public string CurrentDate
    {
        get => _currentDate;
        private set => SetProperty(ref _currentDate, value);
    }

    /// <summary>
    /// Total leave balance (Vacation + Sick)
    /// </summary>
    public string TotalLeaveBalance
    {
        get
        {
            var total = DashboardData.VacationLeaveBalance.InfoboxValue +
                       DashboardData.SickLeaveBalance.InfoboxValue;
            return total.ToString("0.##");
        }
    }

    /// <summary>
    /// Absences this month
    /// </summary>
    public string AbsencesThisMonth => DashboardData.AbsencesMTD.InfoboxValue.ToString("0.##");

    /// <summary>
    /// Tardiness (late arrivals) this month
    /// </summary>
    public string TardinessThisMonth => DashboardData.TardinessMTD.InfoboxValue.ToString("0.##");

    /// <summary>
    /// Total overtime this month
    /// </summary>
    public string OvertimeThisMonth => DashboardData.TotalOvertimeMTD.InfoboxValue.ToString("0.##");

    #endregion

    #region Commands

    /// <summary>
    /// Navigate to Leave Request page
    /// </summary>
    public ICommand NavigateToLeaveCommand { get; }

    /// <summary>
    /// Navigate to Time Entry page
    /// </summary>
    public ICommand NavigateToTimeEntryCommand { get; }

    /// <summary>
    /// Navigate to Overtime page
    /// </summary>
    public ICommand NavigateToOvertimeCommand { get; }

    /// <summary>
    /// Navigate to Attendance page
    /// </summary>
    public ICommand NavigateToAttendanceCommand { get; }

    /// <summary>
    /// Refresh dashboard data
    /// </summary>
    public ICommand RefreshCommand { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Initialize the ViewModel - called when page loads
    /// </summary>
    public override async Task InitializeAsync()
    {
        await LoadDashboardDataAsync();
        SetGreeting();
        SetCurrentDate();
    }

    /// <summary>
    /// Loads dashboard data from the API
    /// </summary>
    private async Task LoadDashboardDataAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var result = await _dashboardService.GetDashboardAsync();
            if (result != null)
            {
                DashboardData = result;
                
                // Notify dependent properties
                OnPropertyChanged(nameof(TotalLeaveBalance));
                OnPropertyChanged(nameof(AbsencesThisMonth));
                OnPropertyChanged(nameof(TardinessThisMonth));
                OnPropertyChanged(nameof(OvertimeThisMonth));
            }
            else
            {
                ErrorMessage = "Failed to load dashboard data";
            }
        }, "Loading dashboard...");
    }

    /// <summary>
    /// Gets the current user's profile ID
    /// </summary>
    private async Task<long> GetProfileIdAsync()
    {
        try
        {
            var profileIdStr = await SecureStorage.GetAsync("ProfileId");
            return long.TryParse(profileIdStr, out var profileId) ? profileId : 0;
        }
        catch (Exception ex)
        {
            HandleError(ex, "Failed to retrieve user profile");
            return 0;
        }
    }

    /// <summary>
    /// Sets personalized greeting based on time of day
    /// </summary>
    private void SetGreeting()
    {
        var hour = DateTime.Now.Hour;
        var greeting = hour < 12 ? "Good morning" : hour < 17 ? "Good afternoon" : "Good evening";
        UserGreeting = $"{greeting}!";
    }

    /// <summary>
    /// Sets formatted current date
    /// </summary>
    private void SetCurrentDate()
    {
        CurrentDate = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
    }

    /// <summary>
    /// Navigate to Leave Request page
    /// </summary>
    private void NavigateToLeave()
    {
        _navigationManager.NavigateTo("/leave");
    }

    /// <summary>
    /// Navigate to Time Entry page
    /// </summary>
    private void NavigateToTimeEntry()
    {
        _navigationManager.NavigateTo("/time-entry");
    }

    /// <summary>
    /// Navigate to Overtime page
    /// </summary>
    private void NavigateToOvertime()
    {
        _navigationManager.NavigateTo("/overtime");
    }

    /// <summary>
    /// Navigate to Attendance page
    /// </summary>
    private void NavigateToAttendance()
    {
        _navigationManager.NavigateTo("/attendance");
    }

    #endregion

    #region Dispose

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Cleanup any subscriptions or resources
        }
        base.Dispose(disposing);
    }

    #endregion
}
