using System.ComponentModel;

namespace MauiHybridApp.Models.Workflow;

public class MyApprovalListModel : INotifyPropertyChanged
{
    public MyApprovalListModel()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        EmployeeName = string.Empty;
        EmployeeNo = string.Empty;
        Department = string.Empty;
        Position = string.Empty;
        TransactionType = string.Empty;
        Details = string.Empty;
        Status = string.Empty;
        RequestedDate = string.Empty;
        RequestedTime = string.Empty;
        RequestedHours = string.Empty;
        ImageType = string.Empty;
        ItemName = string.Empty;
        RequestedHoursSuffixDisplay = string.Empty;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmployeeName { get; set; }
    public string EmployeeNo { get; set; }
    public string Department { get; set; }
    public string Position { get; set; }
    public string TransactionType { get; set; }
    public long TransactionTypeId { get; set; }
    public long TransactionId { get; set; }
    public DateTime DateFiled { get; set; }
    public DateTime DateFiledDisplay { get; set; }
    public string Details { get; set; }
    public string Status { get; set; }
    public string RequestedDate { get; set; }
    public string RequestedTime { get; set; }
    public string RequestedHours { get; set; }
    public bool IsVisible { get; set; }

    public string ImageType { get; set; }
    public string ItemName { get; set; }

    // Custom fields
    public bool DisplayItemName { get; set; }
    public bool IsDocumentRequest { get; set; }
    public bool IsChangeRestday { get; set; }
    public bool IsLoanRequest { get; set; }
    public bool IsScheduleRequest { get; set; }
    public bool IsTimeEntryLogRequest { get; set; }
    public bool IsLeaveRequest { get; set; }
    public bool IsTravelRequest { get; set; }
    public decimal RequestedHoursNumber { get; set; }
    public long ProfileId { get; set; }
    public string RequestedHoursSuffixDisplay { get; set; }

    // Additional properties for approval workflow
    public long RequestId { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? ApproverComments { get; set; }

    private string? _imageSource;
    public string? ImageSource
    {
        get { return _imageSource; }
        set { _imageSource = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageSource))); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
