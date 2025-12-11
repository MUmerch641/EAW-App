using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models.Schedule;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.Navigation;
using MauiHybridApp.Utils;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace MauiHybridApp.ViewModels;

public class OvertimeViewModel : BaseViewModel
{
    private readonly IOvertimeDataService _overtimeService;
    private readonly INavigationService _navigationService;
    
    private OvertimeModel _overtimeRequest;
    private string _successMessage = string.Empty;
    private ObservableCollection<OvertimeModel> _recentRequests;

    public OvertimeViewModel(
        IOvertimeDataService overtimeService,
        INavigationService navigationService)
    {
        _overtimeService = overtimeService ?? throw new ArgumentNullException(nameof(overtimeService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        
        _overtimeRequest = new OvertimeModel
        {
            OvertimeDate = DateTime.Today,
            StartTime = DateTime.Today.AddHours(17), // Default to 5 PM
            EndTime = DateTime.Today.AddHours(19),   // Default to 7 PM
            StatusId = RequestStatusValue.Draft,
            SourceId = (short)SourceEnum.Mobile,
            DateFiled = DateTime.Now
        };
        
        _recentRequests = new ObservableCollection<OvertimeModel>();
        
        SubmitCommand = new AsyncRelayCommand(SubmitRequestAsync);
        GoBackCommand = new AsyncRelayCommand(GoBackAsync);
    }

    public OvertimeModel OvertimeRequest
    {
        get => _overtimeRequest;
        set => SetProperty(ref _overtimeRequest, value);
    }
    
    public ObservableCollection<OvertimeModel> RecentRequests
    {
        get => _recentRequests;
        set => SetProperty(ref _recentRequests, value);
    }

    public decimal TotalHours => CalculateHours();
    public string FormattedHours => $"{TotalHours:F2} hours";

    public ICommand SubmitCommand { get; }
    public ICommand GoBackCommand { get; }

    public override async Task InitializeAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            // Load history for the last 30 days to show recent requests
            var history = await _overtimeService.GetOvertimeHistoryAsync(DateTime.Now.AddDays(-30), DateTime.Now.AddDays(30));
            
            // Sort by date descending
            var sorted = history.OrderByDescending(x => x.OvertimeDate).ToList();
            
            RecentRequests = new ObservableCollection<OvertimeModel>(sorted);
            
        }, "Loading overtime data...");
    }

    private decimal CalculateHours()
    {
        if (OvertimeRequest.EndTime > OvertimeRequest.StartTime)
        {
            var duration = OvertimeRequest.EndTime - OvertimeRequest.StartTime;
            return (decimal)duration.TotalHours;
        }
        return 0;
    }

    private async Task SubmitRequestAsync()
    {
        if (!ValidateRequest()) return;

        try
        {
            ClearError();
            SuccessMessage = string.Empty;
            
            // Update calculated fields before submission
            OvertimeRequest.OROTHrs = CalculateHours(); 
            // Note: In a real scenario, we might split OROT/NSOT based on time of day (night shift diff),
            // but for this MVP we put total into OROTHrs (Regular Overtime).

            var result = await _overtimeService.SubmitOvertimeRequestAsync(OvertimeRequest);

            if (result.Success)
            {
                SuccessMessage = "Overtime request submitted successfully!";
                await Task.Delay(1500);
                await GoBackAsync();
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Failed to submit overtime request.";
            }
        }
        catch (Exception ex)
        {
            HandleError(ex, "Error submitting overtime request");
        }
    }

    private bool ValidateRequest()
    {
        if (OvertimeRequest.EndTime <= OvertimeRequest.StartTime)
        {
            ErrorMessage = "End time must be after start time";
            return false;
        }

        if (string.IsNullOrWhiteSpace(OvertimeRequest.Reason))
        {
            ErrorMessage = "Please provide a reason for overtime";
            return false;
        }

        return true;
    }

    private async Task GoBackAsync()
    {
        await _navigationService.NavigateBackAsync();
    }
}
