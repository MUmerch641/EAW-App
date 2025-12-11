namespace EatWork.Mobile.Contants
{
    public static class Messages
    {
        public const string LoanRequestAgree = "Please agree to terms and conditions";
        public const string Save = "Do you want to save this record?";
        public const string Submit = "Do you want to submit this request?";
        public const string Delete = "Are you sure you want to delete?";
        public const string Logout = "Are you sure you want to sign-out?";

        public const string Approve = "Are you sure you want to approve this record?";
        public const string Disapprove = "Are you sure you want to disapprove this record?";
        public const string Cancel = "Are you sure you want to cancel this record?";

        public const string NOINTERNETCONNECTION = "The device was disconnected from the Internet. Please check your Internet connection and try again.";
        public const string UNREACHABLEHOST = "Unreachable host. Please contact your System Administrator.";

        public const string RecordSaved = "Record successfully saved.";
        public const string RecordCancelled = "Record successfully cancelled.";
        public const string ForgotPasswordResponse = "An email has been sent to your email address. Follow the directions in the email to reset your password.";
        public const string RegistrationSuccess = "Your request for user credentials in everything at work has been submitted for approval. Once approved, you will receive an email with your log in details.";

        public const string ExitApplication = "Are you sure you want to exit?";
        public const string ExitApplicationTitle = "Exit Application?";

        public const string LOCATIONDENIED = "Location Denied! Please allow the app to access your location.";
        public const string ErrorRegistration = "Unable to register, please check the details and try again.";

        //public const string TIMEINMESSAGE = "Thank you for clocking in {0}! You have successfully timed in at {1}";
        //public const string TIMEOUTMESSAGE = "Thank you for clocking out {0}! You have successfully timed out at {1}";

        public const string TIMEINMESSAGE = "Thank you for clocking in! You have successfully timed in at {1}";
        public const string TIMEOUTMESSAGE = "Thank you for clocking out! You have successfully timed out at {1}";
        public const string BREAKINMESSAGE = "Enjoy your break!";
        public const string BREAKOUTMESSAGE = "I hope you are recharged. Time to go back to work";

        public const string NoWorkSchedule = "You have no work schedule set on the selected day.";
        public const string NoTimeEntryLogs = "You have no time entry logs on the selected date.";
        public const string NoRestdaySchedule = "You have no restday on the selected date.";
        public const string ValidationHeaderMessage = "ONE OR MORE VALIDATION ERRORS OCCURED.";

        public const string GPSERROR = "Unable to retrieve your location, please check if GPS is enabled and try again.";
        public const string LEAVEPAGE = "Leave Page? Changes you made may not be saved.";
        public const string DeleteExpenses = "Are you sure you want to delete the selected expense/s?";

        public static string SurveyMessageText = $"I hope you're doing well. Your opinion and feedback is important to us!{Constants.NextLine}{Constants.NextLine} Please take some time to answer the survey!";
        public const string SurveyEndingMessage = "We really appreciate your effort for answering our survey.";

        public const string ApprovalFormSuccessMessage = "Record has been successfully processed!";

        public const string MobileAccessDenied = "Your Everything at Work Mobile Application account has not yet been activated. Please contact your system administrator. ";

        public const string NoObjectivesFound = "Please add objectives!";

        public const string CashAdvanceDateNeededValidation = "Date needed must be greater than requested date.";

        public const string RegisterDeviceInfoMessage = "Do you want to register this device to this account?";
    }
}