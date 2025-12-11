using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Models.Leave;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.Navigation;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

/// <summary>
/// ViewModel for Leave Request page
/// </summary>
public class LeaveViewModel : BaseViewModel
{
    private readonly ILeaveDataService _leaveService;
    private readonly INavigationService _navigationService;
    private readonly IDashboardDataService _dashboardService; // Added
    
    private LeaveRequestModel _leaveRequest;
    private ObservableCollection<ComboBoxObject> _leaveTypes;
    private ObservableCollection<ComboBoxObject> _applyToOptions;
    private ObservableCollection<LeaveRequestModel> _recentLeaves;
    private long _selectedLeaveTypeId;
    private List<string> _attachedDocuments = new();
    private string _leaveBalance = "Loading..."; // Added

    public LeaveViewModel(
        ILeaveDataService leaveService,
        INavigationService navigationService,
        IDashboardDataService dashboardService) // Added
    {
        _leaveService = leaveService ?? throw new ArgumentNullException(nameof(leaveService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
        
        _leaveRequest = new LeaveRequestModel();
        _leaveTypes = new ObservableCollection<ComboBoxObject>();
        _applyToOptions = new ObservableCollection<ComboBoxObject>();
        _recentLeaves = new ObservableCollection<LeaveRequestModel>();
        
        SubmitCommand = new AsyncRelayCommand(SubmitRequestAsync);
        GoBackCommand = new AsyncRelayCommand(GoBackAsync);
    }

    #region Properties

    public string LeaveBalance // Added
    {
        get => _leaveBalance;
        private set => SetProperty(ref _leaveBalance, value);
    }

    public LeaveRequestModel LeaveRequest
    {
        get => _leaveRequest;
        private set => SetProperty(ref _leaveRequest, value);
    }

    public ObservableCollection<ComboBoxObject> LeaveTypes
    {
        get => _leaveTypes;
        private set => SetProperty(ref _leaveTypes, value);
    }

    public ObservableCollection<ComboBoxObject> ApplyToOptions
    {
        get => _applyToOptions;
        private set => SetProperty(ref _applyToOptions, value);
    }

    public ObservableCollection<LeaveRequestModel> RecentLeaves
    {
        get => _recentLeaves;
        private set => SetProperty(ref _recentLeaves, value);
    }

    public long SelectedLeaveTypeId
    {
        get => _selectedLeaveTypeId;
        set
        {
            if (SetProperty(ref _selectedLeaveTypeId, value))
            {
                LeaveRequest.LeaveTypeId = value;
                OnLeaveTypeChanged(value);
            }
        }
    }

    public List<string> AttachedDocuments
    {
        get => _attachedDocuments;
        set => SetProperty(ref _attachedDocuments, value);
    }

    #endregion

    #region Commands

    public ICommand SubmitCommand { get; }
    public ICommand GoBackCommand { get; }
    public ICommand NavigateToHistoryCommand { get; } 

    #endregion

    #region Methods

    public override async Task InitializeAsync()
    {
        await LoadLeaveTypesAsync();
        LoadApplyToOptions();
        await LoadRecentHistoryAsync();
        await LoadLeaveBalanceAsync(); // Added
    }

    private async Task LoadLeaveBalanceAsync()
    {
        try
        {
            var dashboard = await _dashboardService.GetDashboardAsync();
            if (dashboard != null)
            {
                 var total = (dashboard.VacationLeaveBalance?.InfoboxValue ?? 0) +
                             (dashboard.SickLeaveBalance?.InfoboxValue ?? 0);
                 LeaveBalance = $"{total:0.##} Days";
            }
            else
            {
                LeaveBalance = "0 Days";
            }
        }
        catch
        {
            LeaveBalance = "N/A";
        }
    }

    private async Task LoadLeaveTypesAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var types = await _leaveService.GetLeaveTypesAsync();
            LeaveTypes = new ObservableCollection<ComboBoxObject>(
                types.Select(t => new ComboBoxObject { Id = t.Id, Value = t.DisplayText })
            );
        }, "Loading leave types...");
    }

    private void LoadApplyToOptions()
    {
        ApplyToOptions = new ObservableCollection<ComboBoxObject>
        {
            new ComboBoxObject { Id = 1, Value = "Full Day" },
            new ComboBoxObject { Id = 2, Value = "Half Day - Morning" },
            new ComboBoxObject { Id = 3, Value = "Half Day - Afternoon" }
        };
    }
    
    private async Task LoadRecentHistoryAsync()
    {
        try 
        {
            // Fetch last 60 days of history
            var history = await _leaveService.GetLeaveHistoryAsync(DateTime.Now.AddDays(-60), DateTime.Now.AddDays(180));
            
            // Sort by date filed descending or start date
            var sorted = history.OrderByDescending(x => x.InclusiveStartDate).ToList();
            
            RecentLeaves = new ObservableCollection<LeaveRequestModel>(sorted);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading recent history: {ex.Message}");
        }
    }

    private async void OnLeaveTypeChanged(long leaveTypeId)
    {
        if (leaveTypeId > 0)
        {
            await LoadLeaveSummaryAsync(leaveTypeId);
        }
    }

    private async Task LoadLeaveSummaryAsync(long leaveTypeId)
    {
        // Load leave summary/balance for selected type
        // Implementation depends on your service
        await Task.CompletedTask;
    }

    private async Task SubmitRequestAsync()
    {
        if (!ValidateRequest()) return;

        try
        {
            IsBusy = true;
            ClearError();
            SuccessMessage = string.Empty;

            var result = await _leaveService.SubmitLeaveRequestAsync(LeaveRequest);
            
            if (result.Success)
            {
                SuccessMessage = "Leave request submitted successfully!";
                await Task.Delay(1500); // Give user time to see success
                await GoBackAsync();
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Failed to submit leave request.";
            }
        }
        catch (Exception ex)
        {
            HandleError(ex, "Error submitting leave request");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool ValidateRequest()
    {
        if (LeaveRequest.LeaveTypeId <= 0)
        {
            ErrorMessage = "Please select a leave type";
            return false;
        }

        if (!LeaveRequest.InclusiveStartDate.HasValue)
        {
            ErrorMessage = "Please select a start date";
            return false;
        }

        if (!LeaveRequest.InclusiveEndDate.HasValue)
        {
            ErrorMessage = "Please select an end date";
            return false;
        }

        return true;
    }

    private async Task GoBackAsync()
    {
        await _navigationService.NavigateBackAsync();
    }

    #endregion
}
