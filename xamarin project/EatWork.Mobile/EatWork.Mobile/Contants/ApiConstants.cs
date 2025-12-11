namespace EatWork.Mobile.Contants
{
    public class ApiConstants
    {
#if AGC
        public const string BaseApiUrl = "http://192.168.66.1:901/"; /*DEVELOPMENT MODE SA OFFICE IOS*/
        //public const string BaseApiUrl = "http://192.168.10.193:901/"; /*DEVELOPMENT MODE SA OFFICE IOS*/

        public const string SignalRServiceUrl = "http://192.168.10.193:1001/chathub";
#elif PMC
        public const string BaseApiUrl = "http://13.67.42.87:2027/"; /*POWERMAC*/
#elif MCWILSON
        public const string BaseApiUrl = "https://api-test.mcw-hris.com/"; /*MCWILSON*/
#else
        /*public const string BaseApiUrl = "http://13.76.209.158:1037/";*/ /*MOBILE API MAIN CONNECTION*/
        public const string BaseApiUrl = "https://mobile-api.everythingatworksupport.com:443/";
#endif

        public const string SetupClientApi = "api/v1/authentication/client-setup";

        public const string Authenticate = "api/authentication/login";

        public const string MyApproval = "api/workflow/my-approvals";

        public const string MyRequest = "api/workflow/my-requests";

        public const string WorkFlowCancelRequest = "api/workflow/cancel-workflow-transaction";

        public const string MySchedule = "api/workschedule/my-schedule";
        public const string CurrentSchedule = "api/workschedule/current-schedule";

        public const string GetChangeRestdayScheduleList = "api/changerestday/restday-schedule-list";

        /*
        public const string MyExpense = "api/expense-report/list";
        public const string ExpenseSetupList = "api/expense-setup/expense-setup-list";
        public const string VendorList = "api/vendor/vendor-list";
        */

        public const string ProcessWFTransaction = "api/workflow/process-workflow-transaction";

        public const string GetWorkflowDetails = "api/workflow/get-workflow-details";

        public const string GetLeaveRequestDetail = "api/leave/detail/";

        public const string GetLeaveRequestRecord = "api/leave/record/";
        public const string LeaveApi = "api/leave";

        public const string GetLeaveUsage = "api/leave/{0}/{1}/leave-usage-list";

        //public const string LeaveUsageConstant = "leave-usage-list";

        public const string GetOvertimeRequestDetail = "api/overtime/";
        public const string OvertimeApi = "api/overtime";

        public const string GetUndertimeRequestDetail = "api/undertime/";
        public const string UndertimeApi = "api/undertime";

        public const string GetOfficialBusinessRequestDetail = "api/officialbusiness/";
        public const string OfficialBusinessApi = "api/officialbusiness";

        public const string GetChangeWorkSchedRequestDetail = "api/changeworkschedule/";
        public const string ChangeWorkScheduleApi = "api/changeworkschedule";

        public const string GetTimeEntryRequestDetail = "api/timeentrylogs/";
        public const string TimeEntryLogApi = "api/timeentrylogs";

        public const string GetChangeRestdayRequestDetail = "api/changerestday/";
        public const string ChangeRestdayApi = "api/changerestday";

        public const string GetSpecialWorkScheduleRequest = "api/workschedulerequest/";
        public const string SpecialWorkSchedApi = "api/workschedulerequest";

        public const string GetDocumentRequestDetail = "api/documentrequest/";
        public const string DocumentRequest = "api/documentrequest";

        public const string GetFileAttachments = "api/fileattachment/";
        public const string SaveFileAttachment = "api/fileattachment";
        public const string FileAttachmentApi = "api/fileattachment";

        public const string GetEnums = "api/enum/";

        public const string GetShiftList = "api/shift/list";
        public const string GetShiftById = "api/shift/";

        public const string GetExpenseReportRecord = "api/expense-report/";
        public const string ExpeseReportApi = "api/expense-report";

        public const string GetPreOTValidations = "api/overtime/";

        //api/workschedule/{profileId}/employee-schedule
        public const string GetEmployeScheduleByWorkDate = "api/workschedule/{0}/employee-schedule";

        public const string GetWorkScheduleRequestByWorkDate = "api/workschedulerequest/date-schedule";

        //public const string GetEmployeScheduleByWorkDate = "api/workschedule/";

        public const string GetLeaveTypes = "api/leavetype/list";
        public const string GetLeaveTypeById = "api/leavetype/";

        public const string GetLeaveRequestHelper = "api/leave/{0}/{1}/init-form";
        public const string GetValidateLeaveGeneration = "api/leave/validate-leave-generation";

        //api/leavebalance/{profileId}/{leaveTypeId}
        //public const string GetLeaveBalanceByLeaveType = "api/leavebalance/";
        public const string GetLeaveRequestBalance = "api/leave/balance/";

        public const string GetLoanTypes = "api/loantype/list";
        public const string GetLoanType = "api/loantype/";
        public const string GetLoanRequestDetail = "api/loanrequest/";
        public const string GetLoanRequestFile = "api/loanrequest/file/";
        public const string LoanRequestApi = "api/loanrequest";

        public const string EmployeeRegistration = "api/authentication/employee-registration";
        public const string AuthenticationApi = "api/authentication";

        //requests
        public const string SubmitTimeEntryRequest = "api/timeentrylogs";

        public const string SubmitUndertimeRequest = "api/undertime";
        public const string SubmitOvertimeRequest = "api/overtime";
        public const string SubmitOfficialBusinessRequest = "api/officialbusiness";
        public const string SubmitChangeRestdayRequest = "api/changerestday";
        public const string SubmitChangeWorkScheduleRequest = "api/changeworkschedule";
        public const string SubmitSpecialWorkScheduleRequest = "api/workschedulerequest";
        public const string SubmitLeaveRequestRequest = "api/leave";
        public const string SubmitLoanRequest = "api/loanrequest";
        public const string SubmitDocumentRequest = "api/documentrequest";
        public const string SubmitOnlineTimeEntryRequest = "api/onlinetimeentry";

        public const string SubmitVendorDetail = "api/vendor";
        public const string SubmitExpenseReport = "api/expense-report";

        public const string ValidateOBRequest = "api/officialbusiness/validate";

        public const string SubmitLeaveRequestRequestEngine = "api/leave/submit-record";

        public const string GetDocumentTypes = "api/documenttype/list";
        public const string GetReasonPurposeList = "api/reasonpurpose/list";

        //employee
        public const string GetEmployeeById = "api/employee/";

        public const string GetEmployeeList = "api/employee/list";
        public const string GetEmployeePersonalDetails = "api/employee/{0}/personal-details";
        public const string GetEmployeeBySecurityId = "api/employee/{0}/employee-details-by-userid";

        public const string GetAccessMobile = "api/authentication/{0}/get-mobile-access";

        public const string GetTranasactionHistory = "api/workflow/{0}/{1}/transaction-history";

        public const string GetWalkthrough = "api/home/get-walkthrough";
        public const string GetMimeType = "api/home/get-mimetypes";
        public const string GetThemeSetup = "api/home/{0}/get-themesetup";
        public const string GetMaxFileSize = "api/home/file-upload/max-size";

        public const string GetDashboardDefault = "api/dashboard/{0}/default";

        public const string GetOnlineTimeEntryFormHelper = "api/onlinetimeentry/initialize-ote";

        public const string MyTimeLogsList = "api/timeentrylogs/my-timelogs";

        public const string GetMobileConfig = "api/mobileconfig";
        public const string MyAttendanceList = "api/myattendance/list";
        public const string GetImageSetup = "api/home/{0}/{1}/image-setup";
        public const string IndividualAttendance = "api/myattendance/individual-attendance";
        public const string ValidateOvertimeRequest = "api/overtime/validate";

        public const string MyPayslip = "api/payslip/list";
        public const string MyPayslipDetail = "api/payslip/{0}/{1}/detail";
        public const string PrintSetup = "api/payslip/{0}/print-setup";
        public const string PrintPayslip = "/AISGenerateReport/GenerateReport?ListOfSpParams=@PaySheetHeaderId={0}|@ProfileId={1}&ReportCode={2}&ExportFormat=5&ID={3}&DoNotUseScope=1";
        public const string PaslipYTDTemplate = "api/payslip/{0}/{1}/ytd-breakdown-template";

        public const string MyExpenses = "api/expense";
        public const string Vendor = "api/vendor";
        public const string ExpenseReport = "api/expense-report";
        public const string Device = "api/device";
        public const string ProfileImage = "api/profile-image";
        public const string CashAdvance = "api/cash-advance";
        public const string Questionnaire = "api/questionnaire";
        public const string FormAnswer = "api/form-answer";
        public const string TravelRequest = "api/travelrequest";
        public const string IndividualObjectives = "api/individual-objective";
        public const string PerformanceEvaluation = "api/performanceevaluation";
        public const string Suggestion = "api/suggestion";
        public const string SuggestionCategory = "api/suggestion-category";

        public const string Home = "api/home";
        public const string CostCenter = "api/costcenter";

        public const string DetailedAttendanceList = "api/myattendance/detailed-attendance-list";
        public const string DetailedAttendanceForm = "api/myattendance/detailed-attendance-form";
    }
}