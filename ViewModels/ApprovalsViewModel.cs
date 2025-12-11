using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models.Workflow;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.Navigation;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

public class ApprovalsViewModel : BaseViewModel
{
    #region Fields

    private readonly IApprovalDataService _approvalService;
    private readonly INavigationService _navigationService;

    private ObservableCollection<MyApprovalListModel> _approvals;
    private ObservableCollection<MyApprovalListModel> _filteredApprovals;
    private MyApprovalListModel? _selectedApproval;
    private string _activeFilter;
    private string _searchText;
    private string _approvalComments;
    private bool _showApprovalDialog;
    private bool _isApproving;
    private int _pendingCount;
    private int _totalCount;

    #endregion

    #region Properties

    public ObservableCollection<MyApprovalListModel> Approvals
    {
        get => _approvals;
        set => SetProperty(ref _approvals, value);
    }

    public ObservableCollection<MyApprovalListModel> FilteredApprovals
    {
        get => _filteredApprovals;
        set => SetProperty(ref _filteredApprovals, value);
    }

    public MyApprovalListModel? SelectedApproval
    {
        get => _selectedApproval;
        set => SetProperty(ref _selectedApproval, value);
    }

    public string ActiveFilter
    {
        get => _activeFilter;
        set
        {
            if (SetProperty(ref _activeFilter, value))
            {
                ApplyFilter();
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                ApplyFilter();
            }
        }
    }

    public string ApprovalComments
    {
        get => _approvalComments;
        set => SetProperty(ref _approvalComments, value);
    }

    public bool ShowApprovalDialog
    {
        get => _showApprovalDialog;
        set => SetProperty(ref _showApprovalDialog, value);
    }

    public bool IsApproving
    {
        get => _isApproving;
        set => SetProperty(ref _isApproving, value);
    }

    public int PendingCount
    {
        get => _pendingCount;
        set => SetProperty(ref _pendingCount, value);
    }

    public int TotalCount
    {
        get => _totalCount;
        set => SetProperty(ref _totalCount, value);
    }

    public bool HasApprovals => FilteredApprovals?.Any() == true;

    #endregion

    #region Commands

    public ICommand LoadApprovalsCommand { get; }
    public ICommand SetFilterCommand { get; }
    public ICommand OpenApprovalDialogCommand { get; }
    public ICommand CloseApprovalDialogCommand { get; }
    public ICommand ApproveCommand { get; }
    public ICommand RejectCommand { get; }
    public ICommand RefreshCommand { get; }

    #endregion

    #region Constructor

    public ApprovalsViewModel(IApprovalDataService approvalService, INavigationService navigationService)
    {
        _approvalService = approvalService ?? throw new ArgumentNullException(nameof(approvalService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        _approvals = new ObservableCollection<MyApprovalListModel>();
        _filteredApprovals = new ObservableCollection<MyApprovalListModel>();
        _activeFilter = "pending";
        _searchText = string.Empty;
        _approvalComments = string.Empty;

        LoadApprovalsCommand = new AsyncRelayCommand(LoadApprovalsAsync);
        SetFilterCommand = new RelayCommand<string>(SetFilter);
        OpenApprovalDialogCommand = new RelayCommand<MyApprovalListModel>(OpenApprovalDialog);
        CloseApprovalDialogCommand = new RelayCommand(CloseApprovalDialog);
        ApproveCommand = new AsyncRelayCommand(ApproveAsync);
        RejectCommand = new AsyncRelayCommand(RejectAsync);
        RefreshCommand = new AsyncRelayCommand(LoadApprovalsAsync);
    }

    #endregion

    #region Methods

    public override async Task InitializeAsync()
    {
        await LoadApprovalsAsync();
    }

    private async Task LoadApprovalsAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            try
            {
                var approvals = await _approvalService.GetMyApprovalsAsync();
                
                Approvals.Clear();
                if (approvals != null)
                {
                    foreach (var approval in approvals)
                    {
                        Approvals.Add(approval);
                    }
                }

                CalculateCounts();
                ApplyFilter();

                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load approvals: {ex.Message}";
            }
        });
    }

    private void SetFilter(string? filter)
    {
        if (!string.IsNullOrEmpty(filter))
        {
            ActiveFilter = filter;
        }
    }

    private void ApplyFilter()
    {
        var filtered = Approvals.AsEnumerable();

        // Apply status filter
        filtered = ActiveFilter.ToLower() switch
        {
            "pending" => filtered.Where(a => a.Status?.ToLower() == "pending"),
            "approved" => filtered.Where(a => a.Status?.ToLower() == "approved"),
            "rejected" => filtered.Where(a => a.Status?.ToLower() == "rejected"),
            _ => filtered
        };

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(a =>
                a.TransactionType?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                a.TransactionId.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                a.EmployeeName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true);
        }

        FilteredApprovals.Clear();
        foreach (var approval in filtered)
        {
            FilteredApprovals.Add(approval);
        }
    }

    private void CalculateCounts()
    {
        PendingCount = Approvals?.Count(a => a.Status?.ToLower() == "pending") ?? 0;
        TotalCount = Approvals?.Count ?? 0;
    }

    private void OpenApprovalDialog(MyApprovalListModel? approval)
    {
        if (approval != null)
        {
            SelectedApproval = approval;
            ApprovalComments = string.Empty;
            ShowApprovalDialog = true;
        }
    }

    private void CloseApprovalDialog()
    {
        ShowApprovalDialog = false;
        SelectedApproval = null;
        ApprovalComments = string.Empty;
        IsApproving = false;
    }

    private async Task ApproveAsync()
    {
        if (SelectedApproval == null) return;

        IsApproving = true;
        
        await ExecuteBusyAsync(async () =>
        {
            try
            {
                var result = await _approvalService.ApproveRequestAsync(
                    SelectedApproval.TransactionId,
                    ApprovalComments);

                if (result.Success)
                {
                    SuccessMessage = "Request approved successfully";
                    CloseApprovalDialog();
                    await LoadApprovalsAsync();
                }
                else
                {
                    ErrorMessage = result.ErrorMessage ?? "Failed to approve request";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error approving request: {ex.Message}";
            }
            finally
            {
                IsApproving = false;
            }
        });
    }

    private async Task RejectAsync()
    {
        if (SelectedApproval == null) return;

        if (string.IsNullOrWhiteSpace(ApprovalComments))
        {
            ErrorMessage = "Please provide comments for rejection";
            return;
        }

        IsApproving = true;
        
        await ExecuteBusyAsync(async () =>
        {
            try
            {
                var result = await _approvalService.DisapproveRequestAsync(
                    SelectedApproval.TransactionId,
                    ApprovalComments);

                if (result.Success)
                {
                    SuccessMessage = "Request rejected";
                    CloseApprovalDialog();
                    await LoadApprovalsAsync();
                }
                else
                {
                    ErrorMessage = result.ErrorMessage ?? "Failed to reject request";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error rejecting request: {ex.Message}";
            }
            finally
            {
                IsApproving = false;
            }
        });
    }

    #endregion
}
