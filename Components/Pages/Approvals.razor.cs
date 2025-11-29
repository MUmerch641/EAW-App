using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MauiHybridApp.Models.Workflow;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;

namespace MauiHybridApp.Components.Pages;

public partial class Approvals : ComponentBase
{
    private List<MyApprovalListModel>? allApprovals;
    private List<MyApprovalListModel>? filteredApprovals;
    private MyApprovalListModel? selectedApproval;
    
    private bool isLoading = true;
    private bool isSubmitting = false;
    private bool showApprovalDialog = false;
    private bool isApproving = false;
    private string activeTab = "pending";
    private string approvalComments = string.Empty;
    private string errorMessage = string.Empty;
    private string successMessage = string.Empty;
    
    private int pendingCount = 0;
    private int totalCount = 0;

    protected override async Task OnInitializedAsync()
    {
        await LoadApprovals();
    }

    private async Task LoadApprovals()
    {
        try
        {
            isLoading = true;
            StateHasChanged();

            // Load all approvals
            allApprovals = await ApprovalService.GetMyApprovalsAsync();
            
            // Calculate counts
            pendingCount = allApprovals?.Count(a => a.Status?.ToLower() == "pending") ?? 0;
            totalCount = allApprovals?.Count ?? 0;
            
            // Apply current filter
            FilterApprovals();

            errorMessage = string.Empty;
            successMessage = string.Empty;
        }
        catch (Exception ex)
        {
            errorMessage = $"Error loading approvals: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private void SetActiveTab(string tab)
    {
        activeTab = tab;
        FilterApprovals();
        StateHasChanged();
    }

    private void FilterApprovals()
    {
        if (allApprovals == null)
        {
            filteredApprovals = new List<MyApprovalListModel>();
            return;
        }

        filteredApprovals = activeTab switch
        {
            "pending" => allApprovals.Where(a => a.Status?.ToLower() == "pending").ToList(),
            "all" => allApprovals.ToList(),
            _ => allApprovals.ToList()
        };

        // Sort by request date descending
        filteredApprovals = filteredApprovals.OrderByDescending(a => a.RequestDate).ToList();
    }

    private void ShowApprovalDialog(MyApprovalListModel approval, bool isApprove)
    {
        selectedApproval = approval;
        isApproving = isApprove;
        approvalComments = string.Empty;
        showApprovalDialog = true;
        StateHasChanged();
    }

    private void CloseApprovalDialog()
    {
        showApprovalDialog = false;
        selectedApproval = null;
        approvalComments = string.Empty;
        StateHasChanged();
    }

    private async Task ProcessApproval()
    {
        if (selectedApproval == null) return;

        // Validate comments for disapproval
        if (!isApproving && string.IsNullOrWhiteSpace(approvalComments))
        {
            errorMessage = "Comments are required when disapproving a request.";
            return;
        }

        try
        {
            isSubmitting = true;
            errorMessage = string.Empty;
            successMessage = string.Empty;
            StateHasChanged();

            SaveResult result;
            if (isApproving)
            {
                result = await ApprovalService.ApproveRequestAsync(selectedApproval.RequestId, approvalComments);
            }
            else
            {
                result = await ApprovalService.DisapproveRequestAsync(selectedApproval.RequestId, approvalComments);
            }

            if (result.Success)
            {
                successMessage = $"Request {(isApproving ? "approved" : "disapproved")} successfully.";
                CloseApprovalDialog();
                await LoadApprovals(); // Refresh the list
            }
            else
            {
                errorMessage = result.ErrorMessage ?? $"Failed to {(isApproving ? "approve" : "disapprove")} request.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Error processing approval: {ex.Message}";
        }
        finally
        {
            isSubmitting = false;
            StateHasChanged();
        }
    }

    private async Task RefreshApprovals()
    {
        await LoadApprovals();
    }

    private async Task GoBack()
    {
        await JSRuntime.InvokeVoidAsync("history.back");
    }
}
