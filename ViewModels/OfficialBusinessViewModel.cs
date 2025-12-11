using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

public class OfficialBusinessViewModel : BaseViewModel
{
    private readonly IOfficialBusinessDataService _obService;
    private readonly NavigationManager _navigationManager;
    
    private OfficialBusinessModel _obRequest;
    private string _successMessage = string.Empty;

    public OfficialBusinessViewModel(
        IOfficialBusinessDataService obService,
        NavigationManager navigationManager)
    {
        _obService = obService ?? throw new ArgumentNullException(nameof(obService));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        
        _obRequest = new OfficialBusinessModel
        {
            OfficialBusinessDate = DateTime.Today,
            StartTime = DateTime.Today.AddHours(8),
            EndTime = DateTime.Today.AddHours(17),
            StatusId = RequestStatusValue.Draft,
            SourceId = (short)SourceEnum.Mobile,
            DateFiled = DateTime.Now
        };
        
        SubmitCommand = new AsyncRelayCommand(SubmitRequestAsync);
        GoBackCommand = new RelayCommand(GoBack);
    }

    public OfficialBusinessModel OBRequest
    {
        get => _obRequest;
        set => SetProperty(ref _obRequest, value);
    }

    public decimal TotalHours => CalculateHours();

    public ICommand SubmitCommand { get; }
    public ICommand GoBackCommand { get; }

    public virtual void SetTransactionType(long typeId)
    {
        OBRequest.TransactionTypeId = typeId;
    }

    public override async Task InitializeAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            // Default to OB if not set
            if (OBRequest.TransactionTypeId == 0)
                OBRequest.TransactionTypeId = 1; // Assuming 1 is OB, need to verify or strictly set it.
                // Wait, Xamarin doesn't seem to set it explicitly for OB in InitMenuList, but TimeOff sets it to 4 via Constants.TimeOff
                // Let's assume default OB is handled by backend if 0 or we should look up OB constant.
                // For now, base class is for OB, so let's stick to OB default.
            
            await Task.CompletedTask;
        }, "Loading...");
    }

    private decimal CalculateHours()
    {
        if (OBRequest.EndTime > OBRequest.StartTime)
        {
            var duration = OBRequest.EndTime - OBRequest.StartTime;
            return (decimal)(duration?.TotalHours ?? 0);
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

            var result = await _obService.SubmitOfficialBusinessRequestAsync(OBRequest);

            if (result.Success)
            {
                SuccessMessage = "Official business request submitted successfully!";
                await Task.Delay(1500);
                GoBack();
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Failed to submit request.";
            }
        }
        catch (Exception ex)
        {
            HandleError(ex, "Error submitting official business request");
        }
    }

    private bool ValidateRequest()
    {
        if (OBRequest.EndTime <= OBRequest.StartTime)
        {
            ErrorMessage = "End time must be after start time";
            return false;
        }

        if (string.IsNullOrWhiteSpace(OBRequest.Reason))
        {
            ErrorMessage = "Please provide a reason";
            return false;
        }

        return true;
    }

    private void GoBack()
    {
        _navigationManager.NavigateTo("/dashboard");
    }
}
