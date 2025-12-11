using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.Navigation;
using Microsoft.AspNetCore.Components;
using MauiHybridApp.Models.Questionnaire;
using System.Collections.ObjectModel;
using MauiHybridApp.Models.Leave;
using MauiHybridApp.Models.Schedule;
using Microsoft.Maui.Storage;

namespace MauiHybridApp.ViewModels;

/// <summary>
/// ViewModel for Dashboard page following proper MVVM pattern
/// Handles all business logic, data fetching, and navigation
/// Keeps the View (Dashboard.razor) clean and focused on presentation
/// </summary>
public class DashboardViewModel : BaseViewModel
{
    private readonly IDashboardDataService _dashboardService;
    private readonly ISurveyDataService _surveyService;
    private readonly ILeaveDataService _leaveService;
    private readonly IOvertimeDataService _overtimeService; // Added missing service
    private readonly IAuthenticationDataService _authService; // Added missing service
    private readonly INavigationService _navigationService; // Added missing service
    
    // Keeping NavigationManager for legacy support if needed, but trying to move to NavigationService
    private readonly NavigationManager _navigationManager;
    
    private DashboardResponse _dashboardData;
    private ObservableCollection<PulseSurveyList> _surveys;
    private string _userGreeting = string.Empty;
    private string _currentDate = string.Empty;

    public DashboardViewModel(
        INavigationService navigationService,
        IAuthenticationDataService authService, 
        IDashboardDataService dashboardService,
        ISurveyDataService surveyService, // Added back
        ILeaveDataService leaveService,
        IOvertimeDataService overtimeService,
        NavigationManager navigationManager) // Added back
        : base() // BaseViewModel doesn't take args based on previous read
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
        _surveyService = surveyService ?? throw new ArgumentNullException(nameof(surveyService)); // Init survey service
        _leaveService = leaveService ?? throw new ArgumentNullException(nameof(leaveService));
        _overtimeService = overtimeService ?? throw new ArgumentNullException(nameof(overtimeService));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        
        _dashboardData = new DashboardResponse();
        _surveys = new ObservableCollection<PulseSurveyList>();
        
        // Initialize commands
        NavigateToLeaveCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync("leave"));
        NavigateToTimeEntryCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync("time-entry"));
        NavigateToOvertimeCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync("overtime"));
        NavigateToAttendanceCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync("attendance"));
        NavigateToPayslipsCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync("payslips"));
        NavigateToApprovalsCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync("approvals"));
        NavigateToOfficialBusinessCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync("official-business"));
        RefreshCommand = new AsyncRelayCommand(LoadDashboardDataAsync);
        EditSurveyCommand = new AsyncRelayCommand<PulseSurveyList>(AnswerSurveyAsync);
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

    public ObservableCollection<PulseSurveyList> Surveys
    {
        get => _surveys;
        private set => SetProperty(ref _surveys, value);
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
            var total = (DashboardData.VacationLeaveBalance?.InfoboxValue ?? 0) +
                       (DashboardData.SickLeaveBalance?.InfoboxValue ?? 0);
            return total.ToString("0.##");
        }
    }

    /// <summary>
    /// Absences this month
    /// </summary>
    public string AbsencesThisMonth => (DashboardData.AbsencesMTD?.InfoboxValue ?? 0).ToString("0.##");

    /// <summary>
    /// Tardiness (late arrivals) this month
    /// </summary>
    public string TardinessThisMonth => (DashboardData.TardinessMTD?.InfoboxValue ?? 0).ToString("0.##");

    /// <summary>
    /// Total overtime this month
    /// </summary>
    public string OvertimeThisMonth => (DashboardData.TotalOvertimeMTD?.InfoboxValue ?? 0).ToString("0.##");

    // Mini-Chart Data
    public List<double> VacationLeaveTrend { get; private set; } = new();
    public List<double> SickLeaveTrend { get; private set; } = new();
    public List<double> OvertimeTrend { get; private set; } = new();

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

    public ICommand NavigateToPayslipsCommand { get; }
    public ICommand NavigateToApprovalsCommand { get; }
    public ICommand NavigateToOfficialBusinessCommand { get; }

    /// <summary>
    /// Refresh dashboard data
    /// </summary>
    public ICommand RefreshCommand { get; }

    public ICommand EditSurveyCommand { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Initialize the ViewModel - called when page loads
    /// </summary>
    public override async Task InitializeAsync()
    {
        IsBusy = true;
        BusyText = "Loading dashboard...";
        ErrorMessage = string.Empty;

        try
        {
            // Run parallel tasks for performance
            var dashboardTask = _dashboardService.GetDashboardAsync();
            var surveysTask = _surveyService.RetrieveSurveysAsync();
            var leaveHistoryTask = _leaveService.GetLeaveHistoryAsync(DateTime.Now.AddMonths(-6), DateTime.Now);
            var overtimeHistoryTask = _overtimeService.GetOvertimeHistoryAsync(DateTime.Now.AddMonths(-6), DateTime.Now);

            await Task.WhenAll(dashboardTask, surveysTask, leaveHistoryTask, overtimeHistoryTask);

            var dashboardData = await dashboardTask;
            
            // DEBUG: Check Profile ID loaded 
            var pid = await SecureStorage.GetAsync("profile_id");
            if (string.IsNullOrEmpty(pid) || pid == "0")
            {
               await Application.Current.MainPage.DisplayAlert("Debug", "Profile ID is MISSING or 0. Data will be empty.", "OK");
            }
            else
            {
               // await Application.Current.MainPage.DisplayAlert("Debug", $"Loaded Profile ID: {pid}", "OK");
            }

            var surveys = await surveysTask;
            var leaveHistory = await leaveHistoryTask;
            var overtimeHistory = await overtimeHistoryTask;

            // 1. Process Dashboard Stats
            if (dashboardData != null)
            {
                DashboardData = dashboardData;
                OnPropertyChanged(nameof(TotalLeaveBalance));
                OnPropertyChanged(nameof(AbsencesThisMonth));
                OnPropertyChanged(nameof(TardinessThisMonth));
                OnPropertyChanged(nameof(OvertimeThisMonth));
            }

            // 2. Process Surveys
            if (surveys != null)
            {
                Surveys = new ObservableCollection<PulseSurveyList>(surveys);
            }

            // 3. Process Trends
            CalculateVacationWithSickLeaveTrend(leaveHistory);
            CalculateSickLeaveTrend(leaveHistory);
            CalculateOvertimeTrend(overtimeHistory);

            SetGreeting();
            SetCurrentDate();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load dashboard data. Please pull to refresh.";
            Console.WriteLine($"Dashboard Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Loads dashboard data from the API
    /// </summary>
    private async Task LoadDashboardDataAsync()
    {
        await InitializeAsync();
    }

    private async Task AnswerSurveyAsync(PulseSurveyList survey)
    {
        if (survey == null) return;
        await _navigationService.NavigateToAsync($"/survey/{survey.FormHeaderId}/{survey.AnswerId}");
    }

    private void CalculateVacationWithSickLeaveTrend(List<LeaveRequestModel> history)
    {
        var grouped = history
            .Where(x => x.InclusiveStartDate.HasValue)
            .GroupBy(x => new { x.InclusiveStartDate.Value.Year, x.InclusiveStartDate.Value.Month })
            .Select(g => new { Date = new DateTime(g.Key.Year, g.Key.Month, 1), Count = g.Sum(x => (double)(x.NoOfDays ?? 0)) })
            .OrderBy(x => x.Date)
            .ToList();

         VacationLeaveTrend = FillTrendBuckets(grouped.Select(x => x.Count).ToList());
         OnPropertyChanged(nameof(VacationLeaveTrend));
    }

    private void CalculateSickLeaveTrend(List<LeaveRequestModel> history)
    {
         var grouped = history
            .Where(x => x.InclusiveStartDate.HasValue && x.LeaveTypeId == 2) // ID 2 = Sick Leave
            .GroupBy(x => new { x.InclusiveStartDate.Value.Year, x.InclusiveStartDate.Value.Month })
            .Select(g => new { Date = new DateTime(g.Key.Year, g.Key.Month, 1), Count = g.Sum(x => (double)(x.NoOfDays ?? 0)) })
            .OrderBy(x => x.Date)
            .ToList();

         SickLeaveTrend = FillTrendBuckets(grouped.Select(x => x.Count).ToList());
         OnPropertyChanged(nameof(SickLeaveTrend));
    }

    private void CalculateOvertimeTrend(List<OvertimeModel> history)
    {
        var grouped = history
            .GroupBy(x => new { x.OvertimeDate.Year, x.OvertimeDate.Month })
            .Select(g => new { Date = new DateTime(g.Key.Year, g.Key.Month, 1), Hours = g.Sum(x => (double)(x.OROTHrs + x.NSOTHrs)) })
            .OrderBy(x => x.Date)
            .ToList();

         OvertimeTrend = FillTrendBuckets(grouped.Select(x => x.Hours).ToList());
         OnPropertyChanged(nameof(OvertimeTrend));
    }

    private List<double> FillTrendBuckets(List<double> data)
    {
        // Ensure at least 6 points for sparkline visual appeal
        var result = new List<double>(data);
        while (result.Count < 6)
        {
            result.Insert(0, 0);
        }
        return result;
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
