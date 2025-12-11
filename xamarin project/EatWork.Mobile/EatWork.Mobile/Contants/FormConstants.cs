using EatWork.Mobile.Utils;
using System;

namespace EatWork.Mobile.Contants
{
    public static class Constants
    {
        public const string DatabaseName = "EAWMobile.db";

        public static DateTime NullDate = Convert.ToDateTime("01/01/1900 12:00 AM");
        public static DateTime NullDate2 = Convert.ToDateTime("01/02/1900 12:00 AM");
        public static DateTime MinimumNullDate = Convert.ToDateTime("01/01/0001 12:00 AM");

        public static int OfficialBusiness = 1;
        public static int TimeOff = 2;
        public static string OptionOthers = "Others";
        public static string DateFormatMMDDYYYY = PreferenceHelper.DateFormatSetup();
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

        //public static string HourIcon = Application.Current.Resources["HourIcon"].ToString();
        //public static string AmountIcon = Application.Current.Resources["AmountIcon"].ToString();
        //public static string FileIcon = Application.Current.Resources["ReasonIcon"].ToString();
        //public static string DateIcon = Application.Current.Resources["DateIcon"].ToString();
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
        /*public static string PEStatusLookup = "19,2,1,17,25";*/
        public static string PEStatusSubmitLookup = $"17,25";
        public static string PEStatusSaveLookup = $"19";
        public static string EditableStatusLookup = $"{RequestStatusValue.Draft},{RequestStatusValue.ForApproval}";
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

    public static class Color
    {
        public const string Disapproved = "#d15b47";

        //public const string Approved = "#82af6f";
        public const string Approved = "#27ae60";

        public const string ForApproval = "#428bca";
        public const string Submitted = "#428bca";
        public const string Cancelled = "#f89406";

        //public const string Draft = "#D3D3D3";
        public const string Draft = "#4a515e";

        public const string Transparent = "Transparent";

        public const string Warning = "#ffb752";
        public const string Info = "#6fb3e0";
        public const string NavigationPrimary = "#2f72e4";

        public const string DateDefaultColor = "#ff6624";
        public const string DateEventColor = "#e61610";
    }

    public static class ModuleForms
    {
        public const long TimeEntryLogsList = 102;
        public const long UndertimeRequest = 198;
        public const long OvertimeRequest = 199;
        public const long TimeOffRequest = 200;
        public const long OfficialBusiness = 201;
        public const long ChangeWorkScheduleRequest = 202;
        public const long LeaveRequest = 205;
        public const long LoanRequest = 211;
        public const long ChangeRestDaySchedule_Schedule = 232;
        public const long DocumentRequest = 188;
        public const long TravelRequest = 10575;
        public const long CashAdvanceRequest = 216;
    }

    public static class TransactionType
    {
        public const long Leave = 1;
        public const long Undertime = 2;
        public const long Overtime = 3;
        public const long TimeOff = 4;
        public const long OfficialBusiness = 5;
        public const long ChangeWorkSchedule = 6;
        public const long ExtendedTimeandOffset = 7;
        public const long TimeLog = 8;
        public const long Manpower = 9;
        public const long Training = 10;
        public const long Document = 11;
        public const long ProfileUpdate = 12;
        public const long AttendanceReview = 13;
        public const long PayrollDeduction = 14;
        public const long Loan = 15;
        public const long PayrollReview = 16;
        public const long CashAdvance = 17;
        public const long ExpenseReport = 18;
        public const long Item = 19;
        public const long FlexibleBenefit = 20;
        public const long ProjectTimeEntry = 21;
        public const long ChangeRestDay = 22;
        public const long ProvidentFundEnrollee = 23;
        public const long Onboarding = 24;
        public const long Offboarding = 25;
        public const long LegalCases = 26;
        public const long Violation = 27;
        public const long EmployeeAssigment = 28;
        public const long PerformanceAppraisalReview = 29;
        public const long IndividualDevelopmentPlan = 30;
        public const long MedicalConditionandClaimsReport = 31;
        public const long WorkScheduleRequest = 32;
        public const long JobOffer = 33;
        public const long AllowanceOtherEarnings = 34;
        public const long OtherDeductions = 35;
        public const long NetPayBreakdown = 36;
        public const long EmployeeOnhold = 37;
        public const long BankFile = 38;
        public const long Travel = 39;
        public const long SalaryBatchUpdate = 40;
        public const long BenefitIssuance = 41;
        public const long PerformanceObjective = 42;
    }

    public static class TimeEntryTypeValue
    {
        public const string TimeIn = "In";
        public const string TimeOut = "Out";
        public const string BreakIn = "Break-In";
        public const string BreakOut = "Break-Out";
        public const string LunchIn = "Lunch-In";
        public const string LunchOut = "Lunch-Out";
    }

    public static class TimeEntryTypeDisplay
    {
        public const string TimeIn = "Time-In";
        public const string TimeOut = "Time-Out";
        public const string BreakIn = "Break-In";
        public const string BreakOut = "Break-Out";
        public const string Transaction = "Transaction";
    }

    public static class EnumValues
    {
        public const string UndertimeType = "UndertimeType";
        public const string OvertimeType = "OvertimeType";
        public const string OfficialBusinessType = "OfficialBusinessType";
        public const string OfficialBusinessApplyAgainst = "OfficialBusinessApplyAgainst";
        public const string TimeOffType = "TimeOffType";
        public const string TimeOffApplyAgainst = "TimeOffApplyAgainst";
        public const string ChangeWorkScheduleReason = "ChangeWorkScheduleReason";
        public const string LeaveRequestApplyTo = "LeaveRequestApplyTo";
        public const string WorkScheduleRequestType = "WorkScheduleRequestType";
        public const string CivilStatus = "CivilStatus";
        public const string EmploymentStatus = "EmploymentStatus";
        public const string TaxExemptionStatus = "TaxExemptionStatus";
        public const string ApplicableTax = "ApplicableTax";
        public const string BloodType = "BloodType";
        public const string HairColor = "HairColor";
        public const string EyeColor = "EyeColor";
        public const string EmployeeProfileTitle = "EmployeeProfileTitle";
        public const string TypeOfBusinessTrip = "TypeOfBusinessTrip";
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

    public static class HistoryType
    {
        public const long UNROUTED_SUBMITTED_TRANSACTION = 1;
        public const long UNROUTED_APPROVED_TRANSACTION = 2;
        public const long REQUESTOR_TO_PARTIAL_APPROVER = 3;
        public const long PARTIAL_TO_NEXT_APPROVER = 4;
        public const long FINAL_APPROVER_TAKE_ACTION = 5;
        public const long REQUESTOR_CANCEL_REQUEST = 6;
        public const long IMPORTED_TRANSACTION = 7;
        public const long REROUTED_TRANSACTION = 10;
        public const long UNROUTED_TRANSACTION = 11;
        public const long ADDED_APPROVER = 12;
        public const long REMOVED_APPROVER = 13;

        public const long CUSTOM_FOR_DISPLAY = -999;
    }

    public static class ClockworkColor
    {
        public const string Success = "#27ae60";
        public const string Pink = "#EE3F60";
    }

    public static class MobileConfig
    {
        public static string EmpLimitCompany = "EmpLimitCompany";
        public static string EmpLimitBranch = "EmpLimitBranch";
        public static string EmpLimitDepartment = "EmpLimitDepartment";
        public static string EmpLimitTeam = "EmpLimitTeam";
    }

    public static class FileType
    {
        public const string Image = "image";
        public const string File = "file";
    }

    public static class FormHelper
    {
        public static string DateFormat = PreferenceHelper.DateFormatSetup();

        /*
        public static void SetDateFormat(string format)
        {
            DateFormat = format;
        }
        */
    }

    public enum SourceEnum
    {
        HRPortal = 1,
        EmployeePortal,
        Import,
        Mobile
    }

    public static class DisplayStarsBy
    {
        public static int RATE_VALUE = 1;
        public static int CUSTOM_ICON = 2;
        public static int MAX_RATE_VALUE = 3;
        public static int NONE = 4;
    }

    public static class EMOJI
    {
        public static int HAPPY = 1;
        public static int PROUD = 2;
        public static int DISAPPOINTED = 3;
        public static int FRUSTRATED = 4;
    }
}