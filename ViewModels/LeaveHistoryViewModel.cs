using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiHybridApp.Models.Leave;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

public class LeaveHistoryViewModel : BaseViewModel
{
    private readonly ILeaveDataService _leaveService;
    private readonly NavigationManager _navigationManager;
    
    private ObservableCollection<LeaveRequestDisplayModel> _leaveRequests;
    private ObservableCollection<LeaveRequestDisplayModel> _filteredRequests;
    private string _selectedStatus = "All";
    private List<SelectableListModel> _leaveTypes = new();

    public LeaveHistoryViewModel(
        ILeaveDataService leaveService,
        NavigationManager navigationManager)
    {
        _leaveService = leaveService ?? throw new ArgumentNullException(nameof(leaveService));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        
        _leaveRequests = new ObservableCollection<LeaveRequestDisplayModel>();
        _filteredRequests = new ObservableCollection<LeaveRequestDisplayModel>();
        
        LoadRequestsCommand = new Command(async () => await LoadRequestsAsync());
        ViewDetailCommand = new Command<LeaveRequestDisplayModel>(ViewDetail);
        FilterCommand = new Command<string>(FilterRequests);
    }

    public ObservableCollection<LeaveRequestDisplayModel> FilteredRequests
    {
        get => _filteredRequests;
        private set => SetProperty(ref _filteredRequests, value);
    }

    public string SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            if (SetProperty(ref _selectedStatus, value))
            {
                FilterRequests(value);
            }
        }
    }

    public ICommand LoadRequestsCommand { get; }
    public ICommand ViewDetailCommand { get; }
    public ICommand FilterCommand { get; }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await LoadRequestsAsync();
    }

    private async Task LoadRequestsAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            // 1. Fetch Leave Types for mapping
            _leaveTypes = await _leaveService.GetLeaveTypesAsync();

            // 2. Fetch Requests
            var requests = await _leaveService.GetLeaveRequestsAsync();
            
            // 3. Map to Display Model
            var displayModels = requests.Select(r => MapToDisplayModel(r)).OrderByDescending(r => r.DateFiled).ToList();

            _leaveRequests = new ObservableCollection<LeaveRequestDisplayModel>(displayModels);
            FilterRequests(SelectedStatus);
        }, "Loading leave history...");
    }

    private LeaveRequestDisplayModel MapToDisplayModel(LeaveRequestModel request)
    {
        var displayModel = new LeaveRequestDisplayModel
        {
            // Copy properties
            LeaveRequestId = request.LeaveRequestId,
            ProfileId = request.ProfileId,
            LeaveTypeId = request.LeaveTypeId,
            CompanyId = request.CompanyId,
            RemainingHours = request.RemainingHours,
            InclusiveStartDate = request.InclusiveStartDate,
            InclusiveEndDate = request.InclusiveEndDate,
            NoOfHours = request.NoOfHours,
            PartialDayLeave = request.PartialDayLeave,
            PartialDayApplyTo = request.PartialDayApplyTo,
            Reason = request.Reason,
            StatusId = request.StatusId,
            DateFiled = request.DateFiled,
            TotalNoOfHours = request.TotalNoOfHours,
            NoOfDays = request.NoOfDays,
            Planned = request.Planned,
            LeaveRequestHeaderId = request.LeaveRequestHeaderId,
            SourceId = request.SourceId
        };

        // Map Leave Type Name
        var type = _leaveTypes.FirstOrDefault(t => t.Id == request.LeaveTypeId);
        displayModel.LeaveType = type?.DisplayText ?? "Unknown Type";

        // Map Status Name
        displayModel.Status = GetStatusName(request.StatusId);

        // Calculate Duration
        if (request.NoOfDays.HasValue && request.NoOfDays > 0)
        {
            displayModel.Duration = request.NoOfDays.Value;
        }
        else if (request.InclusiveStartDate.HasValue && request.InclusiveEndDate.HasValue)
        {
             displayModel.Duration = (request.InclusiveEndDate.Value - request.InclusiveStartDate.Value).Days + 1;
        }

        return displayModel;
    }

    private string GetStatusName(long statusId)
    {
        // Simple mapping based on RequestStatusValue
        // Ideally use reflection or a dictionary map if complete accuracy is needed
        return statusId switch
        {
            1 => "Draft",
            2 => "Approved",
            6 => "Request",
            7 => "New",
            9 => "Disapproved",
            12 => "Submitted",
            13 => "Pending", // ForApproval
            40 => "Rejected",
            -2 => "Cancelled",
            _ => "Pending" // Default to Pending for unknown statuses
        };
    }

    private void FilterRequests(string status)
    {
        if (string.IsNullOrEmpty(status) || status == "All")
        {
            FilteredRequests = new ObservableCollection<LeaveRequestDisplayModel>(_leaveRequests);
        }
        else
        {
            // Map UI status to backend status logic
            // "Pending" covers Submitted, ForApproval, New, Request
            // "Approved" covers Approved, Posted
            // "Rejected" covers Disapproved, Rejected, Cancelled

            var filtered = _leaveRequests.Where(r => 
            {
                if (status == "Pending")
                    return r.Status == "Pending" || r.Status == "Submitted" || r.Status == "New" || r.Status == "Request";
                if (status == "Approved")
                    return r.Status == "Approved";
                if (status == "Rejected")
                    return r.Status == "Rejected" || r.Status == "Disapproved" || r.Status == "Cancelled";
                
                return r.Status == status;
            }).ToList();
            
            FilteredRequests = new ObservableCollection<LeaveRequestDisplayModel>(filtered);
        }
    }

    private void ViewDetail(LeaveRequestDisplayModel request)
    {
        Console.WriteLine($"View detail for request {request.LeaveRequestId}");
    }
}

public class LeaveRequestDisplayModel : LeaveRequestModel
{
    public string LeaveType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public double Duration { get; set; }
}
