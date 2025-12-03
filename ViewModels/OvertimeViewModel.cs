using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models.Schedule;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

public class OvertimeViewModel : BaseViewModel
{
    private readonly IOvertimeDataService _overtimeService;
    private readonly NavigationManager _navigationManager;
    
    private OvertimeModel _overtimeRequest;
    private string _successMessage = string.Empty;

    public OvertimeViewModel(
        IOvertimeDataService overtimeService,
        NavigationManager navigationManager)
    {
        _overtimeService = overtimeService ?? throw new ArgumentNullException(nameof(overtimeService));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        
        _overtimeRequest = new OvertimeModel
        {
            OvertimeDate = DateTime.Today,
            StartTime = DateTime.Today.AddHours(8),
            EndTime = DateTime.Today.AddHours(17),
            StatusId = RequestStatusValue.Draft,
            SourceId = (short)SourceEnum.Mobile,
            DateFiled = DateTime.Now
        };
        
        SubmitCommand = new AsyncRelayCommand(SubmitRequestAsync);
        GoBackCommand = new RelayCommand(GoBack);
    }

    public OvertimeModel OvertimeRequest
    {
        get => _overtimeRequest;
        set => SetProperty(ref _overtimeRequest, value);
    }

    public string SuccessMessage
    {
        get => _successMessage;
        private set => SetProperty(ref _successMessage, value);
    }

    public decimal TotalHours => CalculateHours();
    public string FormattedHours => $"{TotalHours:F2} hours";

    public ICommand SubmitCommand { get; }
    public ICommand GoBackCommand { get; }

    public async Task InitializeAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            await Task.CompletedTask;
        }, "Loading...");
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

            var result = await _overtimeService.SubmitOvertimeRequestAsync(OvertimeRequest);

            if (result.Success)
            {
                SuccessMessage = "Overtime request submitted successfully!";
                await Task.Delay(1500);
                GoBack();
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

    private void GoBack()
    {
        _navigationManager.NavigateTo("/dashboard");
    }
}
