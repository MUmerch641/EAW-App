using Autofac;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Services.TestServices;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Utils.DataAccess;
using EatWork.Mobile.ViewModels;
using EatWork.Mobile.ViewModels.AttendanceViewTemplate2;
using System;
using CA = EatWork.Mobile.ViewModels.CashAdvance;
using Expense = EatWork.Mobile.ViewModels.Expenses;
using HELPER = Mobile.Utils.ControlGenerator;
using IO = EatWork.Mobile.ViewModels.IndividualObjectives;
using Payslip = EatWork.Mobile.ViewModels.Payslip;
using PE = EatWork.Mobile.ViewModels.PerformanceEvaluation;
using SC = EatWork.Mobile.ViewModels.SuggestionCorner;
using SV = EatWork.Mobile.ViewModels.Survey;
using Travel = EatWork.Mobile.ViewModels.TravelRequest;

namespace EatWork.Mobile.Bootstrap
{
    public class AppContainer
    {
        private static IContainer _container;

        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            //view models
            builder.RegisterType<LoginViewModel>();
            builder.RegisterType<ForgotPasswordViewModel>();
            builder.RegisterType<SignupViewModel>();
            builder.RegisterType<MainPageViewModel>();
            builder.RegisterType<MyRequestViewModel>();
            builder.RegisterType<LeaveRequestViewModel>();
            builder.RegisterType<OfficialBusinessViewModel>();
            builder.RegisterType<RequestMenuViewModel>();
            builder.RegisterType<OvertimeViewModel>();
            builder.RegisterType<UndertimeRequestViewModel>();
            builder.RegisterType<TimeEntryViewModel>();
            builder.RegisterType<ChangeWorkScheduleViewModel>();
            builder.RegisterType<LoanRequestViewModel>();
            builder.RegisterType<DocumentRequestViewModel>();
            builder.RegisterType<MyApprovalViewModel>();
            builder.RegisterType<ChangeRestDayViewModel>();
            builder.RegisterType<SpecialWorkScheduleViewModel>();

            builder.RegisterType<FileAttachmentViewModel>();

            builder.RegisterType<LeaveApprovalViewModel>();
            builder.RegisterType<ChangeWorkScheduleApprovalViewModel>();
            builder.RegisterType<OvertimeApprovalViewModel>();
            builder.RegisterType<OfficialBusinessApprovalViewModel>();
            builder.RegisterType<UndertimeApprovalViewModel>();
            builder.RegisterType<TimeEntryApprovalViewModel>();
            builder.RegisterType<DocumentRequestApprovalViewModel>();
            builder.RegisterType<LoanApprovalViewModel>();
            builder.RegisterType<ChangeRestdayScheduleApprovalViewModel>();
            builder.RegisterType<SpecialWorkScheduleApprovalViewModel>();

            builder.RegisterType<MyProfileViewModel>();
            builder.RegisterType<EmployeeProfileViewModel>();
            builder.RegisterType<PersonalDetailsViewModel>();
            builder.RegisterType<EmploymentInformationViewModel>();

            builder.RegisterType<DashboardViewModel>();
            builder.RegisterType<ConnectionViewModel>();
            builder.RegisterType<WalkthroughViewModel>();
            builder.RegisterType<WorkflowViewModel>();
            builder.RegisterType<OnlineTimeEntryViewModel>();
            builder.RegisterType<MyScheduleViewModel>();
            builder.RegisterType<MyAttendanceViewModel>();
            builder.RegisterType<MyTimeLogsViewModel>();
            builder.RegisterType<SettingsViewModel>();
            builder.RegisterType<IndividualAttendanceViewModel>();
            builder.RegisterType<Payslip.MyPayslipViewModel>();
            builder.RegisterType<Payslip.MyPayslipDetailViewModel>();

            builder.RegisterType<Payslip.YTDOTPaymentBreakdownListViewModel>();
            builder.RegisterType<Payslip.YTDOTPaymentBreakdownDetailViewModel>();

            builder.RegisterType<Expense.MyExpensesListViewModel>();
            builder.RegisterType<Expense.MyExpenseDetailViewModel>();
            builder.RegisterType<Expense.NewExpenseViewModel>();
            builder.RegisterType<Expense.MyExpenseReportDetailViewModel>();
            builder.RegisterType<Expense.MyExpenseReportsViewModel>();
            builder.RegisterType<Expense.NewVendorViewModel>();

            builder.RegisterType<CA.CashAdvanceRequestViewModel>();
            builder.RegisterType<CA.CashAdvancesViewModel>();

            builder.RegisterType<SV.SurveryDetailViewModel>();
            builder.RegisterType<SV.SurveyListViewModel>();
            builder.RegisterType<SV.SurveyChartViewModel>();

            builder.RegisterType<TravelRequestFormViewModel>();
            builder.RegisterType<SpecialNoteRequestViewModel>();

            builder.RegisterType<IO.IndividualObjectivesViewModel>();
            builder.RegisterType<IO.IndividualObjectiveItemViewModel>();
            builder.RegisterType<IO.ObjectiveDetailViewModel>();
            builder.RegisterType<IO.RateScaleViewModel>();
            builder.RegisterType<IO.GoalsListViewModel>();
            builder.RegisterType<IO.ObjectivesListViewModel>();

            builder.RegisterType<PE.PEListViewModel>();
            builder.RegisterType<PE.PEFormViewModel>();
            builder.RegisterType<PE.QuestionnaireViewModel>();
            builder.RegisterType<PE.NarrativeSectionViewModel>();
            builder.RegisterType<PE.InputRatingViewModel>();
            builder.RegisterType<FileOptionViewModel>();

            builder.RegisterType<SC.SuggestionsViewModel>();
            builder.RegisterType<SC.SuggestionFormViewModel>();
            builder.RegisterType<SC.CategoryModalViewModel>();
            builder.RegisterType<Travel.TravelRequestApprovalViewModel>();
            builder.RegisterType<AttendanceViewTemplate2ViewModel>();
            builder.RegisterType<AttendanceTemplate2DetailViewModel>();

            //services
            builder.RegisterType<AuthenticationDataService>().As<IAuthenticationDataService>();
            builder.RegisterType<MainPageDataService>().As<IMainPageDataService>();
            builder.RegisterType<CommonDataService>().As<ICommonDataService>();

            //builder.RegisterType<TimeOffRequestPageDataService>().As<ITimeOffRequestPageDataService>();
            builder.RegisterType<Services.LoanRequestDataService>().As<ILoanRequestDataService>();
            builder.RegisterType<Services.DocumentRequestDataService>().As<IDocumentRequestDataService>();
            builder.RegisterType<RequestMenuDataService>().As<IRequestMenuDataService>();
            builder.RegisterType<EmployeeProfileDataService>().As<IEmployeeProfileDataService>();

            builder.RegisterType<Services.GenericRepository>().As<IGenericRepository>();
            builder.RegisterType<Services.MyApprovalDataService>().As<IMyApprovalDataService>();
            builder.RegisterType<Services.WorkflowDataService>().As<IWorkflowDataService>();
            //builder.RegisterType<WorkflowDataService>().As<IWorkflowDataService>();
            builder.RegisterType<Services.MyRequestDataService>().As<IMyRequestDataService>();
            builder.RegisterType<Services.TimeEntryRequestDataService>().As<ITimeEntryRequestDataService>();
            builder.RegisterType<Services.UndertimeRequestDataService>().As<IUndertimeRequestDataService>();
            builder.RegisterType<Services.OvertimeRequestDataService>().As<IOvertimeRequestDataService>();
            builder.RegisterType<Services.OfficialBusinessRequestDataService>().As<IOfficialBusinessRequestDataService>();
            builder.RegisterType<Services.ChangeRestdayScheduleDataService>().As<IChangeRestdayScheduleDataService>();
            builder.RegisterType<Services.EmployeeListDataService>().As<IEmployeeDataService>();
            builder.RegisterType<Services.ChangeWorkScheduleDataService>().As<IChangeWorkScheduleDataService>();
            builder.RegisterType<Services.SpecialWorkScheduleDataService>().As<ISpecialWorkScheduleDataService>();
            builder.RegisterType<Services.LeaveRequestDataService>().As<ILeaveRequestDataService>();
            builder.RegisterType<Services.DashboardDataService>().As<IDashboardDataService>();

            builder.RegisterType<Services.OnlineTimeEntryDataService>().As<IOnlineTimeEntryDataService>();
            builder.RegisterType<Services.DialogService>().As<IDialogService>();
            builder.RegisterType<Services.MyScheduleDataService>().As<IMyScheduleDataService>();
            builder.RegisterType<Services.MyAttendanceDataService>().As<IMyAttendanceDataService>();
            builder.RegisterType<Services.MyTimeLogsDataService>().As<IMyTimeLogsDataService>();
            builder.RegisterType<Services.SettingsDataService>().As<ISettingsDataService>();
            builder.RegisterType<Services.PayslipDataService>().As<IPayslipDataService>();

            builder.RegisterType<Services.ExpenseDataService>().As<IExpenseDataService>();
            builder.RegisterType<Services.ExpenseReportDataService>().As<IExpenseReportDataService>();
            builder.RegisterType<Services.CashAdvanceRequestDataService>().As<ICashAdvanceRequestDataService>();

            builder.RegisterType<HELPER.GenerateControlService>().As<HELPER.IGenerateControlService>();
            builder.RegisterType<Services.Questionnaire.SurveyService>().As<ISurveyDataService>();

            builder.RegisterType<Services.TravelRequestService>().As<ITravelRequestDataService>();

            builder.RegisterType<Services.IndividualObjectivesDataService>().As<IIndividualObjectivesDataService>();
            builder.RegisterType<Services.IndividualObjectiveItemDataService>().As<IIndividualObjectiveItemDataService>();
            builder.RegisterType<Services.GoalDataService>().As<IGoalDataService>();

            builder.RegisterType<Services.PEListDataService>().As<IPEListDataService>();

            builder.RegisterType<Services.PEFormDataService>().As<IPEFormDataService>();

            builder.RegisterType<Services.SuggestionCorner.SuggestionFormDataService>().As<ISuggestionFormDataService>();
            builder.RegisterType<Services.SuggestionCorner.SuggestionListDataService>().As<ISuggestionListDataService>();
            builder.RegisterType<Services.DownloadDataService>().As<IDownloadDataService>();
            builder.RegisterType<Services.NavigationService>().As<INavigationService>();
            builder.RegisterType<Services.SignalRDataService>().As<ISignalRDataService>();

            //helpers
            builder.RegisterType<LoginDataAccess>();
            builder.RegisterType<MyExpenseDataAccess>();
            builder.RegisterType<RequestType>();
            builder.RegisterType<KeyboardHelper>();
            builder.RegisterType<StringHelper>();

            builder.RegisterType<ClientSetupDataAccess>();
            builder.RegisterType<ThemeDataAccess>();
            builder.RegisterType<EmployeeFilterSelectionDataAccess>();
            builder.RegisterType<UserDeviceInfoDataAccess>();
            builder.RegisterType<PermissionHelper>();

            _container = builder.Build();
        }

        public static object Resolve(Type typeName)
        {
            return _container.Resolve(typeName);
        }

        public static T Resolve<T>(TypedParameter typedParameter)
        {
            return _container.Resolve<T>();
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public static T Resolve<T>(NamedParameter param_)
        {
            return _container.Resolve<T>(param_);
        }
    }
}