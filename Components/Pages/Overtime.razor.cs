using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MauiHybridApp.Models.Schedule;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.State;
using MauiHybridApp.Utils;
using OvertimeSummary = MauiHybridApp.Services.Data.OvertimeSummary;

namespace MauiHybridApp.Components.Pages;

public partial class Overtime : ComponentBase
{
    private OvertimeModel overtimeRequest = new();
    private OvertimeSummary? overtimeSummary;
    
    private bool isLoading = true;
    private bool isSubmitting = false;
    private string startTimeString = "08:00";
    private string endTimeString = "17:00";
    private string errorMessage = string.Empty;
    private string successMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadOvertimeForm();
    }

    private Task LoadOvertimeForm()
    {
        try
        {
            isLoading = true;
            StateHasChanged();

            // Initialize dates
            var now = DateTime.Now;
            overtimeRequest.OvertimeDate = now.Date;
            overtimeRequest.StartTime = now.Date.AddHours(8);
            overtimeRequest.EndTime = now.Date.AddHours(17);
            overtimeRequest.StatusId = RequestStatusValue.Draft;
            overtimeRequest.SourceId = (short)SourceEnum.Mobile;
            overtimeRequest.DateFiled = DateTime.Now;

            // Update Strings
            startTimeString = overtimeRequest.StartTime.ToString("HH:mm");
            endTimeString = overtimeRequest.EndTime.ToString("HH:mm");

            CalculateOvertimeHours();

            errorMessage = string.Empty;
            successMessage = string.Empty;
        }
        catch (Exception ex)
        {
            errorMessage = $"Error loading overtime form: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }

        return Task.CompletedTask;
    }

    private void OnStartTimeChanged(ChangeEventArgs e)
    {
        if (TimeSpan.TryParse(e.Value?.ToString(), out var startTime))
        {
            startTimeString = e.Value?.ToString() ?? "08:00";
            // Combine Date + Time
            overtimeRequest.StartTime = overtimeRequest.OvertimeDate.Date.Add(startTime);
            CalculateOvertimeHours();
            StateHasChanged();
        }
    }

    private void OnEndTimeChanged(ChangeEventArgs e)
    {
        if (TimeSpan.TryParse(e.Value?.ToString(), out var endTime))
        {
            endTimeString = e.Value?.ToString() ?? "17:00";
            // Combine Date + Time
            overtimeRequest.EndTime = overtimeRequest.OvertimeDate.Date.Add(endTime);
            CalculateOvertimeHours();
            StateHasChanged();
        }
    }

    private void OnForOffsettingChanged(ChangeEventArgs e)
    {
        if (e.Value is bool val)
        {
            overtimeRequest.ForOffsetting = val;
            if (val)
            {
                overtimeRequest.OffsettingExpirationDate = overtimeRequest.OvertimeDate.AddDays(30);
            }
            else
            {
                overtimeRequest.OffsettingExpirationDate = null;
            }
            StateHasChanged();
        }
    }

    private void CalculateOvertimeHours()
    {
        try
        {
            // Simple Calculation (No API needed for local calc)
            var totalHours = (decimal)(overtimeRequest.EndTime - overtimeRequest.StartTime).TotalHours;
            
            if (totalHours > 0)
            {
                // Assign to model directly
                overtimeRequest.OROTHrs = totalHours; // Simplified (Regular OT)
                overtimeRequest.NSOTHrs = 0; // Simplified (Night Shift 0 for now)

                // Update Summary UI
                overtimeSummary = new OvertimeSummary
                {
                    TotalHours = totalHours,
                    RegularOTHours = totalHours,
                    NightShiftOTHours = 0
                };
            }
            else
            {
                overtimeRequest.OROTHrs = 0;
                overtimeRequest.NSOTHrs = 0;
                overtimeSummary = null;
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Error calculating hours: {ex.Message}";
        }
    }

    private async Task SubmitRequest()
    {
        if (isSubmitting) return;

        try
        {
            if (overtimeRequest.StartTime >= overtimeRequest.EndTime)
            {
                errorMessage = "End Time must be after Start Time.";
                return;
            }
            
            if (string.IsNullOrWhiteSpace(overtimeRequest.Reason))
            {
                errorMessage = "Reason is required.";
                return;
            }

            isSubmitting = true;
            errorMessage = string.Empty;
            
            // Recalculate just in case
            CalculateOvertimeHours();

            var result = await OvertimeService.SubmitOvertimeRequestAsync(overtimeRequest);

            if (result.Success)
            {
                successMessage = "Overtime Request Submitted Successfully!";
                await Task.Delay(2000);
                await GoBack();
            }
            else
            {
                errorMessage = result.ErrorMessage;
            }
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isSubmitting = false;
            StateHasChanged();
        }
    }

    // Dummy SaveDraft to satisfy UI
    private async Task SaveDraft() => await SubmitRequest(); 

    private bool ValidateForm() => true; // Validation logic moved to SubmitRequest

    private async Task GoBack()
    {
        await JSRuntime.InvokeVoidAsync("history.back");
    }
}