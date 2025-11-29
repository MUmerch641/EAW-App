using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MauiHybridApp.Models.Leave;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.State;
using MauiHybridApp.Utils;
using ApiEndpoints = MauiHybridApp.Utils.ApiEndpoints;
using Microsoft.Maui.Storage;

namespace MauiHybridApp.Components.Pages;

public partial class Leave : ComponentBase
{
private LeaveRequestModel leaveRequest = new();
private List<SelectableListModel> leaveTypes = new();
private List<ComboBoxObject> applyToOptions = new();
private LeaveSummary? leaveSummary;

private bool isLoading = true;
private bool isSubmitting = false;
private bool showPartialOptions = false;
private bool isPlanned = false;
private string selectedLeaveTypeId = string.Empty;
private int selectedApplyToOption = 1;
private string errorMessage = string.Empty;
private string successMessage = string.Empty;

protected override async Task OnInitializedAsync()
{
    await LoadLeaveForm();
}

private async Task LoadLeaveForm()
{
    try
    {
        isLoading = true;
        StateHasChanged();

        // ============================================================
        // 🔥 FIX: Profile ID Load karo warna API ko pata nahi chalega
        // ============================================================
        var profileIdStr = await SecureStorage.GetAsync("profile_id");
        if (long.TryParse(profileIdStr, out long pid))
        {
            leaveRequest.ProfileId = pid;
            // EmployeeNo bhi zaroori ho sakta hai, filhal ProfileId se try karte hain
        }
        else 
        {
            errorMessage = "Error: User Profile ID not found. Please logout and login again.";
            return;
        }

        // Load leave types
        leaveTypes = await LeaveService.GetLeaveTypesAsync();
        Console.WriteLine($"Loaded {leaveTypes?.Count ?? 0} leave types");
        if (leaveTypes != null && leaveTypes.Any())
        {
            Console.WriteLine($"First leave type: {leaveTypes[0].DisplayText} (ID: {leaveTypes[0].Id})");
        }
        
        // Load apply to options
        applyToOptions = new List<ComboBoxObject>
        {
            new() { Id = 1, Value = "Full Day" },
            new() { Id = 2, Value = "Half Day - Morning" },
            new() { Id = 3, Value = "Half Day - Afternoon" }
        };

        // Initialize form with default values
        leaveRequest.InclusiveStartDate = DateTime.UtcNow.Date;  // ✅ Use UTC
        leaveRequest.InclusiveEndDate = DateTime.UtcNow.Date;    // ✅ Use UTC
        leaveRequest.StatusId = RequestStatusValue.Draft;
        leaveRequest.SourceId = (short)SourceEnum.Mobile;
        leaveRequest.DateFiled = DateTime.UtcNow;  // ✅ Use UTC

        errorMessage = string.Empty;
        successMessage = string.Empty;
    }
    catch (Exception ex)
    {
        errorMessage = $"Error loading leave form: {ex.Message}";
    }
    finally
    {
        isLoading = false;
        StateHasChanged();
    }
}

private async Task OnLeaveTypeChanged(ChangeEventArgs e)
{
    if (long.TryParse(e.Value?.ToString(), out var leaveTypeId))
    {
        leaveRequest.LeaveTypeId = leaveTypeId;
        selectedLeaveTypeId = leaveTypeId.ToString();

        // Check if this leave type supports partial days
        var selectedLeaveType = leaveTypes.FirstOrDefault(lt => lt.Id == leaveTypeId);
        showPartialOptions = selectedLeaveType?.DisplayData?.Contains("partial") == true;

        await CalculateLeaveSummary();
        StateHasChanged();
    }
}



private async Task OnDateChanged()
{
    if (leaveRequest.InclusiveStartDate.HasValue && leaveRequest.InclusiveEndDate.HasValue)
    {
        // Ensure end date is not before start date
        if (leaveRequest.InclusiveEndDate < leaveRequest.InclusiveStartDate)
        {
            leaveRequest.InclusiveEndDate = leaveRequest.InclusiveStartDate;
        }

        await CalculateLeaveSummary();
        StateHasChanged();
    }
}

private async Task OnApplyToChanged(int option)
{
    selectedApplyToOption = option;
    
    // Agar Option 1 (Full Day) hai to PartialDayLeave 0 hoga
    // Agar Option 2/3 hai to PartialDayLeave 1 hoga
    leaveRequest.PartialDayLeave = (short)(option > 1 ? 1 : 0);
    
    // PartialDayApplyTo ko Option ki value do (1, 2, or 3), ya API requirement ke hisab se 0 agar Full Day hai
    // Swagger example mein 0 tha, lekin usually logic ye hoti hai:
    // 0 = None, 1 = AM, 2 = PM (Check karlo API documentation agar hai)
    // Filhal hum safe side ke liye ye karte hain:
    
    if (option == 1) // Full Day
    {
        leaveRequest.PartialDayApplyTo = 0;
    }
    else 
    {
        leaveRequest.PartialDayApplyTo = (short)option;
    }

    await CalculateLeaveSummary();
    StateHasChanged();
}

private async Task CalculateLeaveSummary()
{
    // Sirf tab calculate karo jab dates aur type selected hon
    if (leaveRequest.LeaveTypeId.HasValue && 
        leaveRequest.InclusiveStartDate.HasValue && 
        leaveRequest.InclusiveEndDate.HasValue)
    {
        try
        {
            // 1. API Call
            leaveSummary = await LeaveService.CalculateLeaveSummaryAsync(
                leaveRequest.LeaveTypeId.Value,
                leaveRequest.InclusiveStartDate.Value,
                leaveRequest.InclusiveEndDate.Value,
                selectedApplyToOption);

            // =========================================================
            // 🔥 CRITICAL FIX: Result ko Model mein Copy karo
            // =========================================================
            if (leaveSummary != null)
            {
                // Ye line zaroori hai taake submit karte waqt 0 na jaye
                leaveRequest.NoOfDays = (short)leaveSummary.TotalDays;
                leaveRequest.TotalNoOfHours = leaveSummary.TotalHours;
                leaveRequest.NoOfHours = leaveSummary.TotalHours;
                
                // Debugging ke liye (Optional)
                Console.WriteLine($"[CALC] Days: {leaveRequest.NoOfDays}, Hours: {leaveRequest.TotalNoOfHours}");
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Error calculating leave summary: {ex.Message}";
        }
    }
}

private async Task SaveDraft()
{
    if (isSubmitting) return;

    try
    {
        isSubmitting = true;
        errorMessage = string.Empty;
        successMessage = string.Empty;
        StateHasChanged();

        leaveRequest.StatusId = RequestStatusValue.Draft;
        var result = await LeaveService.SaveLeaveRequestAsync(leaveRequest);

        if (result.Success)
        {
            successMessage = "Leave request saved as draft successfully.";
            leaveRequest.LeaveRequestId = result.Id;
        }
        else
        {
            errorMessage = result.ErrorMessage ?? "Failed to save leave request.";
        }
    }
    catch (Exception ex)
    {
        errorMessage = $"Error saving draft: {ex.Message}";
    }
    finally
    {
        isSubmitting = false;
        StateHasChanged();
    }
}

private async Task SubmitRequest()
{
    if (isSubmitting) return;

    try
    {
        // 1. Validation (Dates, Reason etc.)
        if (!ValidateForm()) return;

        // 2. 🔥 NEW CHECK: Agar Days 0 hain to dobara calculate karo
        if (leaveRequest.NoOfDays <= 0)
        {
            await CalculateLeaveSummary();
            
            // Agar abhi bhi 0 hai, to user ko roko
            if (leaveRequest.NoOfDays <= 0)
            {
                errorMessage = "Cannot submit a leave request for 0 days. Please check dates.";
                return;
            }
        }

        isSubmitting = true;
        errorMessage = string.Empty;
        successMessage = string.Empty;
        StateHasChanged();

        leaveRequest.StatusId = RequestStatusValue.Submitted;
        
        // 3. API Call
        var result = await LeaveService.SubmitLeaveRequestAsync(leaveRequest);

        if (result.Success)
        {
            successMessage = "Leave request submitted successfully.";
            await Task.Delay(2000);
            await GoBack();
        }
        else
        {
            errorMessage = result.ErrorMessage ?? "Failed to submit leave request.";
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

private bool ValidateForm()
{
    var errors = new List<string>();

    if (!leaveRequest.LeaveTypeId.HasValue || leaveRequest.LeaveTypeId.Value == 0)
    {
        errors.Add("Please select a leave type.");
    }

    if (!leaveRequest.InclusiveStartDate.HasValue)
    {
        errors.Add("Please select a start date.");
    }

    if (!leaveRequest.InclusiveEndDate.HasValue)
    {
        errors.Add("Please select an end date.");
    }

    if (string.IsNullOrWhiteSpace(leaveRequest.Reason))
    {
        errors.Add("Please enter a reason for the leave request.");
    }

    if (leaveRequest.InclusiveStartDate.HasValue && leaveRequest.InclusiveEndDate.HasValue)
    {
        if (leaveRequest.InclusiveEndDate < leaveRequest.InclusiveStartDate)
        {
            errors.Add("End date cannot be before start date.");
        }
    }

    if (errors.Any())
    {
        errorMessage = string.Join(" ", errors);
        return false;
    }

    return true;
}

private void ViewDocuments()
{
    // Navigate to documents page
    NavManager.NavigateTo("/leave/documents");
}

private async Task GoBack()
{
    await JSRuntime.InvokeVoidAsync("history.back");
}


}
