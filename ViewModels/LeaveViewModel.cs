using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Models.Leave;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

/// <summary>
/// ViewModel for Leave Request page
/// </summary>
public class LeaveViewModel : BaseViewModel
{
    private readonly ILeaveDataService _leaveService;
    private readonly NavigationManager _navigationManager;
    
    private LeaveRequestModel _leaveRequest;
    private ObservableCollection<ComboBoxObject> _leaveTypes;
    private ObservableCollection<ComboBoxObject> _applyToOptions;
    private long _selectedLeaveTypeId;
    private string _successMessage = string.Empty;

    public LeaveViewModel(
        ILeaveDataService leaveService,
        NavigationManager navigationManager)
    {
        _leaveService = leaveService ?? throw new ArgumentNullException(nameof(leaveService));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        
        _leaveRequest = new LeaveRequestModel();
        _leaveTypes = new ObservableCollection<ComboBoxObject>();
        _applyToOptions = new ObservableCollection<ComboBoxObject>();
        
        SubmitCommand = new AsyncRelayCommand(SubmitRequestAsync);
        GoBackCommand = new RelayCommand(GoBack);
    }

    #region Properties

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

    public string SuccessMessage
    {
        get => _successMessage;
        private set => SetProperty(ref _successMessage, value);
    }

    public bool HasSuccessMessage => !string.IsNullOrEmpty(SuccessMessage);

    #endregion

    #region Commands

    public ICommand SubmitCommand { get; }
    public ICommand GoBackCommand { get; }

    #endregion

    #region Methods

    public override async Task InitializeAsync()
    {
        await LoadLeaveTypesAsync();
        LoadApplyToOptions();
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
                await Task.Delay(1500);
                GoBack();
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

    private void GoBack()
    {
        _navigationManager.NavigateTo("/dashboard");
    }

    #endregion
}
