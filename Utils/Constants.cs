namespace MauiHybridApp.Utils;

public static class Constants
{
    public const string DatabaseName = "EAWMobile.db";

    public static DateTime NullDate = Convert.ToDateTime("01/01/1900 12:00 AM");
    public static DateTime NullDate2 = Convert.ToDateTime("01/02/1900 12:00 AM");
    public static DateTime MinimumNullDate = Convert.ToDateTime("01/01/0001 12:00 AM");

    public static int OfficialBusiness = 1;
    public static int TimeOff = 2;
    public static string OptionOthers = "Others";
    public static string DateFormatMMDDYYYY = "MM/dd/yyyy";
    public static string DateFormatMMDDYYYYHHMMTT = "MM/dd/yyyy hh:mm tt";
    public static string TimeFormatHHMMTT = "hh:mm tt";
    public static string DateFormatDDMMYYYY = "dd/MM/yyyy";

    public static string HourIcon = "fas-hourglass-half";
    public static string AmountIcon = "fas-money-bill-alt";
    public static string FileIcon = "fas-file-alt";
    public static string DateIcon = "md-date-range";
    public static string TimeIcon = "md-query-builder";
    public static string UtensilIcon = "fas-utensils";
    public static string AutomobileIcon = "fas-car-alt";
    public static string PencilIcon = "fas-car-alt";
    public static string InstitutionIcon = "fas-university";
    public static string NewspaperIcon = "fas-newspaper";
    public static string EllipsisHIcon = "fas-ellipsis-h";
    public static string CheckCircle = "fas-check-circle";
    public static string Circle = "far-circle";

    public static string NextLine = "\r\n";
    public static string SourceOnlineTimeEntry = "EAWAPP";
    public static string SourceTimeEntry = "EAWAPP TE";
    public static string PesoSign = "₱";
    public static string TRANSPARENT = "TRANSPARENT";
    public static string DEFAULTPAGE = "DEFAULTPAGE";
    public static string SUMMARYPAGE = "SUMMARYPAGE";
    public static string ENDINGPAGE = "ENDINGPAGE";
    public static string VersionNumber = "1.3";

    public static string OrgGoalsConstant = "Organization Goals";
    public static string NoOrgGoalsConstant = "No Organization Goal";
    public static short SelfEvaluation = 1;
    public static string ListDefaultDateFormat = "ddd, MMM. dd, yyyy";
    public static string PEStatusSubmitLookup = "17,25";
    public static string PEStatusSaveLookup = "19";
    public static string EditableStatusLookup = "1,13";
}

public static class ActionType
{
    public const string Disapprove = "Disapprove";
    public const string Approve = "Approve";
    public const string ForApproval = "For Approval";
    public const string Cancel = "Cancel";
    public const string Draft = "Draft";
    public const string PartiallyApprove = "Partially Approve";
    public const string Submitted = "Submitted";
}

public static class ActionTypeId
{
    public const long Approve = 1;
    public const long Cancel = 2;
    public const long Disapprove = 3;
    public const long ForRelease = 4;
    public const long Issue = 5;
    public const long Return = 6;
    public const long Release = 7;
    public const long Resume = 8;
    public const long Suspend = 9;
    public const long Start = 10;
    public const long Complete = 11;
    public const long Post = 12;
    public const long Publish = 13;
    public const long MarkAsActive = 14;
    public const long Revise = 15;
    public const long MarkReviewed = 16;
}

public static class RequestStatus
{
    public const string Cancelled = "Cancelled";
    public const string Deleted = "Deleted";
    public const string Draft = "Draft";
    public const string Approved = "Approved";
    public const string Issued = "Issued";
    public const string Resume = "Resume";
    public const string Suspended = "Suspended";
    public const string Request = "Request";
    public const string New = "New";
    public const string Waitlisted = "Waitlisted";
    public const string Disapproved = "Disapproved";
    public const string Posted = "Posted";
    public const string Unposted = "Unposted";
    public const string Submitted = "Submitted";
    public const string ForApproval = "For Approval";
    public const string Adjustment = "Adjustment";
    public const string Released = "Released";
    public const string Expired = "Expired";
    public const string NotStarted = "Not Started";
    public const string Started = "Started";
    public const string Completed = "Completed";
    public const string Revised = "Revised";
    public const string Paid = "Paid";
    public const string Attended = "Attended";
    public const string Assessed = "Assessed";
    public const string Reviewed = "Reviewed";
    public const string InProgress = "In Progress";
    public const string NotPaid = "Not Paid";
    public const string Processed = "Processed";
    public const string Denied = "Denied";
    public const string Available = "Available";
    public const string Overdue = "Overdue";
    public const string Returned = "Returned";
    public const string Incomplete = "Incomplete";
    public const string Scheduled = "Scheduled";
    public const string ForRelease = "For Release";
    public const string NotTaken = "Not Taken";
    public const string Normal = "Normal";
    public const string NotAttended = "Not Attended";
    public const string Archived = "Archived";
    public const string Open = "Open";
    public const string Closed = "Closed";
    public const string Rejected = "Rejected";
    public const string UnderInvestigation = "Under Investigation";
    public const string Resolved = "Resolved";
    public const string ClosedwithoutResolution = "Closed without Resolution";
    public const string Confirmed = "Confirmed";
    public const string Declined = "Declined";
    public const string Enrolled = "Enrolled";
    public const string Postponed = "Postponed";
    public const string Hired = "Hired";
    public const string ForReschedule = "For Reschedule";
    public const string Rescheduled = "Rescheduled";
    public const string Accepted = "Accepted";
    public const string Blacklisted = "Blacklisted";
    public const string Inactive = "Inactive";
    public const string ForProcessing = "For Processing";
    public const string OnHold = "On Hold";
    public const string Lifted = "Lifted";
    public const string Active = "Active";
    public const string Acknowledged = "Acknowledged";
    public const string ForReview = "For Review";
    public const string ForAssessment = "For Assessment";
    public const string ForFurtherInvestigation = "For Further Investigation";
    public const string ForNTEIssuance = "For NTE Issuance";
    public const string ForDecision = "For Decision";
    public const string Queued = "Queued";
    public const string ReversalInprogress = "Reversal Inprogress";
    public const string ApprovalInprogress = "Approval Inprogress";
    public const string Dismissed = "Dismissed";
    public const string Failed = "Failed";
    public const string Cleansed = "Cleansed";
    public const string ForDisposal = "For Disposal";
    public const string Defective = "Defective";
    public const string ForRepair = "For Repair";
    public const string Disposed = "Disposed";
    public const string ForAdminHearing = "For Admin Hearing";
    public const string Pubslished = "Pubslished";
    public const string Stopped = "Stopped";
    public const string PartiallyApproved = "PartiallyApproved";
}

public static class RequestStatusValue
{
    public const long Cancelled = -2;
    public const long Deleted = -1;
    public const long Draft = 1;
    public const long Approved = 2;
    public const long Issued = 3;
    public const long Resume = 4;
    public const long Suspended = 5;
    public const long Request = 6;
    public const long New = 7;
    public const long Waitlisted = 8;
    public const long Disapproved = 9;
    public const long Posted = 10;
    public const long Unposted = 11;
    public const long Submitted = 12;
    public const long ForApproval = 13;
    public const long Adjustment = 14;
    public const long Released = 15;
    public const long Expired = 16;
    public const long NotStarted = 17;
    public const long Started = 18;
    public const long Completed = 19;
    public const long Revised = 20;
    public const long Paid = 21;
    public const long Attended = 22;
    public const long Assessed = 23;
    public const long Reviewed = 24;
    public const long InProgress = 25;
    public const long NotPaid = 26;
    public const long Processed = 27;
    public const long Denied = 28;
    public const long Available = 29;
    public const long Overdue = 30;
    public const long Returned = 31;
    public const long Incomplete = 32;
    public const long Scheduled = 33;
    public const long ForRelease = 34;
    public const long NotTaken = 35;
    public const long Normal = 98;
    public const long NotAttended = 36;
    public const long Archived = 37;
    public const long Open = 38;
    public const long Closed = 39;
    public const long Rejected = 40;
    public const long UnderInvestigation = 41;
    public const long Resolved = 42;
    public const long ClosedwithoutResolution = 43;
    public const long Confirmed = 44;
    public const long Declined = 45;
    public const long Enrolled = 46;
    public const long Postponed = 47;
    public const long Hired = 48;
    public const long ForReschedule = 49;
    public const long Rescheduled = 50;
    public const long Accepted = 51;
    public const long Blacklisted = 52;
    public const long Inactive = 57;
    public const long ForProcessing = 53;
    public const long OnHold = 54;
    public const long Lifted = 55;
    public const long Active = 56;
    public const long Acknowledged = 60;
    public const long ForReview = 58;
    public const long ForAssessment = 59;
    public const long ForFurtherInvestigation = 61;
    public const long ForNTEIssuance = 62;
    public const long ForDecision = 63;
    public const long Queued = 64;
    public const long ReversalInprogress = 65;
    public const long ApprovalInprogress = 66;
    public const long Dismissed = 67;
    public const long Failed = 68;
    public const long Cleansed = 69;
    public const long ForDisposal = 70;
    public const long Defective = 71;
    public const long ForRepair = 72;
    public const long Disposed = 73;
    public const long ForAdminHearing = 74;
    public const long Pubslished = 75;
    public const long Stopped = 76;
}

public static class ApiEndpoints
{
    public const string BaseUrl = "https://mobile-api.everythingatworksupport.com:443/";
    public const string Dashboard = "api/dashboard";
    public const string MenuItems = "api/menu";
    public const string Leave = "api/leave";
    public const string Overtime = "api/overtime";
    public const string Attendance = "api/attendance";

    // Leave endpoints
    public const string GetLeaveRequests = "api/leave/requests";
    public const string GetLeaveRequestById = "api/leave/requests/{0}";
    public const string CreateLeaveRequest = "api/v1/leave";
    public const string UpdateLeaveRequest = "api/leave/requests/{0}";
    public const string SubmitLeaveRequest = "api/leave/requests/{0}/submit";
    public const string DeleteLeaveRequest = "api/leave/requests/{0}";
    public const string GetLeaveTypes = "api/leave/types";
    public const string CalculateLeaveSummary = "api/leave/calculate-summary";

    // Overtime endpoints
    public const string GetOvertimeRequests = "api/v1/overtime/request/list"; 
    public const string GetOvertimeRequestById = "api/overtime/requests/{0}";
    public const string CreateOvertimeRequest = "api/v1/overtime"; 
    public const string UpdateOvertimeRequest = "api/overtime/requests/{0}";
    public const string SubmitOvertimeRequest = "api/overtime/requests/{0}/submit";
    public const string DeleteOvertimeRequest = "api/overtime/requests/{0}";
    public const string CalculateOvertimeSummary = "api/overtime/calculate-summary";

    // Official Business endpoints
    public const string GetOfficialBusinessRequests = "api/v1/officialbusiness/request/list";
    public const string GetOfficialBusinessRequestById = "api/officialbusiness/requests/{0}";
    public const string CreateOfficialBusinessRequest = "api/v1/officialbusiness";
    public const string UpdateOfficialBusinessRequest = "api/officialbusiness/requests/{0}";
    public const string SubmitOfficialBusinessRequest = "api/officialbusiness/requests/{0}/submit";
    public const string DeleteOfficialBusinessRequest = "api/officialbusiness/requests/{0}";

    // Time Entry endpoints
    public const string GetTimeEntries = "api/timeentry/entries";
    public const string GetTimeEntryById = "api/timeentry/entries/{0}";
    public const string CreateTimeEntry = "api/timeentry/entries";
    public const string UpdateTimeEntry = "api/timeentry/entries/{0}";
    public const string DeleteTimeEntry = "api/timeentry/entries/{0}";
    public const string ClockIn = "api/timeentry/clockin";
    public const string ClockOut = "api/timeentry/clockout";

    // Approval endpoints
    public const string GetMyApprovals = "api/v1/workflow/my-approvals";
        public const string ProcessWorkflow = "api/v1/workflow/process-workflow-transaction";

    public const string ApproveRequest = "api/approvals/{0}/approve";
    public const string DisapproveRequest = "api/approvals/{0}/disapprove";

    // Attendance endpoints
    public const string GetAttendanceRecords = "api/attendance/records";
    public const string GetAttendanceSummary = "api/attendance/summary";

    // User Profile endpoints
public const string GetUserProfile = "api/v1/employee/{0}"; 
        public const string UpdateUserProfile = "api/v1/employee/update"; 

    // Authentication endpoints
    public const string Login = "api/authentication/login";
    public const string Register = "api/authentication/employee-registration";
    public const string ForgotPassword = "api/authentication/forgot-password";
    public const string RefreshToken = "api/authentication/refresh-token";
    public const string ValidateToken = "api/authentication/validate-token";
    public const string Logout = "api/authentication/logout";

    // File Upload endpoints
    public const string UploadProfilePhoto = "api/files/profile-photo";
    public const string UploadDocument = "api/files/document/{0}";
    public const string DeleteFile = "api/files/{0}";
    public const string DownloadFile = "api/files/{0}/download";
    public const string GetMaxFileSize = "api/files/max-size";
}

public enum SourceEnum
{
    HRPortal = 1,
    EmployeePortal,
    Import,
    Mobile
}


