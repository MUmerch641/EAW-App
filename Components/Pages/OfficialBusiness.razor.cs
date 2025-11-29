using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MauiHybridApp.Models; // <--- YE ZAROORI HAI
using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.State;
using MauiHybridApp.Utils;

namespace MauiHybridApp.Components.Pages;

public partial class OfficialBusiness : ComponentBase
{
    private OfficialBusinessModel officialBusinessRequest = new();
    
    // 🔥 CHANGE 1: 'object' hata kar ye class use karo
    private OBSummary? travelSummary;
    
    private bool isLoading = true;
    private bool isSubmitting = false;
    private string startTimeString = "08:00";
    private string endTimeString = "17:00";
    private string errorMessage = string.Empty;
    private string successMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadOfficialBusinessForm();
    }

    private Task LoadOfficialBusinessForm()
    {
        try
        {
            isLoading = true;
            StateHasChanged();

            // Initialize defaults
            var now = DateTime.Now;
            officialBusinessRequest.StartDate = now.Date;
            officialBusinessRequest.EndDate = now.Date;
            officialBusinessRequest.StartTime = now.Date.AddHours(8); 
            officialBusinessRequest.EndTime = now.Date.AddHours(17);
            officialBusinessRequest.StatusId = RequestStatusValue.Draft;
            officialBusinessRequest.SourceId = (short)SourceEnum.Mobile;
            officialBusinessRequest.DateFiled = now;

            startTimeString = officialBusinessRequest.StartTime?.ToString("HH:mm") ?? "08:00";
            endTimeString = officialBusinessRequest.EndTime?.ToString("HH:mm") ?? "17:00";

            CalculateTravelSummary();

            errorMessage = string.Empty;
            successMessage = string.Empty;
        }
        catch (Exception ex)
        {
            errorMessage = $"Error loading form: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }

        return Task.CompletedTask;
    }

    // --- Event Handlers ---
    private void OnDateChanged()
    {
        CalculateTravelSummary();
        StateHasChanged();
    }

    private void OnStartDateChanged(ChangeEventArgs e)
    {
        if (DateTime.TryParse(e.Value?.ToString(), out var newDate))
        {
            officialBusinessRequest.StartDate = newDate;
            OnDateChanged();
        }
    }

    private void OnEndDateChanged(ChangeEventArgs e)
    {
        if (DateTime.TryParse(e.Value?.ToString(), out var newDate))
        {
            officialBusinessRequest.EndDate = newDate;
            OnDateChanged();
        }
    }

    private void OnStartTimeChanged(ChangeEventArgs e)
    {
        if (TimeSpan.TryParse(e.Value?.ToString(), out var startTime))
        {
            startTimeString = e.Value?.ToString() ?? "08:00";
            // Date + Time combine karo
            var baseDate = officialBusinessRequest.StartDate ?? DateTime.Now.Date;
            officialBusinessRequest.StartTime = baseDate.Add(startTime);
            CalculateTravelSummary();
            StateHasChanged();
        }
    }

    private void OnEndTimeChanged(ChangeEventArgs e)
    {
        if (TimeSpan.TryParse(e.Value?.ToString(), out var endTime))
        {
            endTimeString = e.Value?.ToString() ?? "17:00";
            // Date + Time combine karo
            var baseDate = officialBusinessRequest.EndDate ?? DateTime.Now.Date;
            officialBusinessRequest.EndTime = baseDate.Add(endTime);
            CalculateTravelSummary();
            StateHasChanged();
        }
    }

    private void CalculateTravelSummary()
    {
        if (officialBusinessRequest.StartDate.HasValue && officialBusinessRequest.EndDate.HasValue)
        {
            try
            {
                var totalDays = (officialBusinessRequest.EndDate.Value - officialBusinessRequest.StartDate.Value).Days + 1;
                decimal totalHours = 0;

                // ... Time calculation logic same ...
                if (officialBusinessRequest.StartTime.HasValue && officialBusinessRequest.EndTime.HasValue)
                {
                    if (totalDays == 1)
                        totalHours = (decimal)(officialBusinessRequest.EndTime.Value - officialBusinessRequest.StartTime.Value).TotalHours;
                    else
                        totalHours = totalDays * 8;
                }

                if (totalDays > 0)
                {
                    // 🔥 CHANGE 2: Nayi Class ka object banao
                    travelSummary = new OBSummary 
                    { 
                        TotalDays = totalDays, 
                        TotalHours = totalHours > 0 ? totalHours : totalDays * 8 
                    };
                }
                else
                {
                    travelSummary = null;
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error calculating travel summary: {ex.Message}";
            }
        }
    }

    private async Task SubmitRequest()
    {
        if (isSubmitting) return;

        if (!ValidateForm()) return;

        try
        {
            isSubmitting = true;
            errorMessage = string.Empty;
            
            var result = await OfficialBusinessService.SubmitOfficialBusinessRequestAsync(officialBusinessRequest);

            if (result.Success)
            {
                successMessage = "Request submitted successfully.";
                await Task.Delay(2000);
                await GoBack();
            }
            else
            {
                errorMessage = result.ErrorMessage ?? "Failed to submit request.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Error submitting request: {ex.Message}";
        }
        finally
        {
            isSubmitting = false;
            StateHasChanged();
        }
    }
    
    // Dummy Save Draft
    private async Task SaveDraft() => await SubmitRequest();

    private bool ValidateForm()
    {
        var errors = new List<string>();

        if (!officialBusinessRequest.StartDate.HasValue) errors.Add("Select Start Date.");
        if (!officialBusinessRequest.EndDate.HasValue) errors.Add("Select End Date.");
        
        if (string.IsNullOrWhiteSpace(officialBusinessRequest.Destination)) 
            errors.Add("Destination is required.");
            
        if (string.IsNullOrWhiteSpace(officialBusinessRequest.Purpose)) 
            errors.Add("Purpose is required.");

        if (errors.Any())
        {
            errorMessage = string.Join("\n", errors);
            return false;
        }
        return true;
    }

    private async Task GoBack() => await JSRuntime.InvokeVoidAsync("history.back");
}

// 🔥 CHANGE 3: Ye Class file ke end mein add kardo
public class OBSummary
{
    public int TotalDays { get; set; }
    public decimal TotalHours { get; set; }
}