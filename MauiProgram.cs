using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.WebView.Maui;
using CommunityToolkit.Maui;
using MauiHybridApp.Services;
using MauiHybridApp.Services.Authentication;
using MauiHybridApp.Services.Common;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.Navigation;
using MauiHybridApp.Services.SignalR;
using MauiHybridApp.Services.Platform;
using MauiHybridApp.Services.State;

namespace MauiHybridApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMediaElement()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
                fonts.AddFont("Roboto-Bold.ttf", "RobotoBold");
            });

        // Add Blazor Hybrid support
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddTransient<MauiHybridApp.ViewModels.PayslipDetailViewModel>();
        builder.Services.AddTransient<MauiHybridApp.ViewModels.TaskNotificationViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        // Register platform-specific services
        RegisterPlatformServices(builder.Services);

        // Register core services
        RegisterCoreServices(builder.Services);

        // Register data services
        RegisterDataServices(builder.Services);

        // Register ViewModels/State services
        RegisterStateServices(builder.Services);

        return builder.Build();
    }

    private static void RegisterPlatformServices(IServiceCollection services)
    {
        // Platform-specific implementations will be registered here
        services.AddSingleton<ISQLite, SQLiteImplementation>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IDeviceService, DeviceService>();
    }

    private static void RegisterCoreServices(IServiceCollection services)
    {
        // Navigation
        services.AddSingleton<INavigationService, NavigationService>();

        // HTTP Client with Polly
        services.AddHttpClient<IGenericRepository, GenericRepository>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(100);
        })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            });

        // Authentication
        services.AddSingleton<IAuthenticationDataService, AuthenticationDataService>();
        
        // SignalR
        services.AddSingleton<MauiHybridApp.Services.SignalR.ISignalRDataService, MauiHybridApp.Services.SignalR.SignalRDataService>();

        // Common services
        services.AddSingleton<ICommonDataService, CommonDataService>();
        services.AddSingleton<IPreferenceHelper, PreferenceHelper>();
        services.AddSingleton<IAppCenterService, AppCenterService>();

        // State service
        services.AddSingleton<IStateService, StateService>();
    }

    private static void RegisterDataServices(IServiceCollection services)
    {
        // Data access services - will be populated with all data services
        services.AddScoped<IMainPageDataService, MainPageDataService>();
        services.AddScoped<IDashboardDataService, DashboardDataService>();
        services.AddSingleton<ILeaveDataService, LeaveDataService>();
        services.AddSingleton<IEmployeeDataService, EmployeeListDataService>();
        services.AddSingleton<IOfficialBusinessDataService, OfficialBusinessDataService>();
        services.AddSingleton<IOvertimeDataService, OvertimeDataService>();
        services.AddSingleton<IChangeRestdayScheduleDataService, ChangeRestdayScheduleDataService>();
        services.AddSingleton<IChangeWorkScheduleDataService, ChangeWorkScheduleDataService>();
        services.AddSingleton<IMyRequestDataService, MyRequestDataService>();
        services.AddSingleton<IWorkflowDataService, WorkflowDataService>();
        services.AddScoped<ITimeEntryDataService, TimeEntryDataService>();
        services.AddScoped<IApprovalDataService, ApprovalDataService>();
        services.AddScoped<IAttendanceDataService, AttendanceDataService>();
        services.AddScoped<IExpenseDataService, ExpenseDataService>();
        services.AddScoped<IPayrollDataService, PayrollDataService>();
        services.AddScoped<IPerformanceDataService, PerformanceDataService>();
        services.AddScoped<IPerformanceEvaluationDataService, PerformanceEvaluationDataService>();
        services.AddScoped<IIndividualObjectivesDataService, IndividualObjectivesDataService>();
        services.AddScoped<IIndividualObjectiveItemDataService, IndividualObjectiveItemDataService>();
        services.AddScoped<ISurveyDataService, SurveyDataService>();
        services.AddScoped<IEmployeeRelationsDataService, EmployeeRelationsDataService>();
        services.AddScoped<IUserDataService, UserDataService>();
        services.AddScoped<INotificationDataService, NotificationDataService>();
        services.AddScoped<ISettingsDataService, SettingsDataService>();
        services.AddScoped<IScheduleDataService, ScheduleDataService>();
        services.AddScoped<IFinancialDataService, FinancialDataService>();
        services.AddScoped<ITravelDataService, TravelDataService>();
        services.AddScoped<IDocumentDataService, DocumentDataService>();
        services.AddScoped<IUndertimeDataService, UndertimeDataService>();
        services.AddScoped<ISuggestionDataService, SuggestionDataService>();
        services.AddScoped<ISpecialWorkScheduleDataService, SpecialWorkScheduleDataService>();
        services.AddScoped<IWorkflowDataService, WorkflowDataService>();

        // Additional services
        services.AddSingleton<ISQLiteDataService, SQLiteDataService>();
        services.AddSingleton<IFileUploadService, FileUploadService>();

        // Common services - Logging
        services.AddSingleton<MauiHybridApp.Services.Common.ILoggingService, MauiHybridApp.Services.Common.LoggingService>();

        // Performance services
        services.AddSingleton<MauiHybridApp.Services.Performance.ICacheService, MauiHybridApp.Services.Performance.CacheService>();
        services.AddSingleton<MauiHybridApp.Services.Performance.IPerformanceService, MauiHybridApp.Services.Performance.PerformanceService>();


    }

    private static void RegisterStateServices(IServiceCollection services)
    {
        // ViewModels - MVVM Pattern Implementation
        services.AddTransient<MauiHybridApp.ViewModels.DashboardViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.ProfileViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.LeaveViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.TimeEntryViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.OvertimeViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.OfficialBusinessViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.PayslipsViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.PayslipDetailViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.ExpensesViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.AttendanceViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.ApprovalsViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.ConnectionViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.LeaveHistoryViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.PerformanceEvaluationViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.PerformanceEvaluationFormViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.IndividualObjectivesViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.IndividualObjectiveFormViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.SurveyViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.SurveyFormViewModel>();
        
        // Profile ViewModels
        services.AddTransient<MauiHybridApp.ViewModels.PersonalDetailsViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.ContactInformationViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.FamilyBackgroundViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.EmergencyContactViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.CurrentJobInformationViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.AreaOfAssignmentViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.RelevantEmploymentDatesViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.TaxGovernmentRelatedInfoViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.ObjectiveDetailViewModel>();
        
        // Schedule ViewModels
        services.AddTransient<MauiHybridApp.ViewModels.MyScheduleViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.ScheduleRequestViewModel>();
        
        // Financial ViewModels
        services.AddTransient<MauiHybridApp.ViewModels.FinancialViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.CashAdvanceRequestViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.LoanRequestViewModel>();
        
        // Request ViewModels
        services.AddTransient<MauiHybridApp.ViewModels.TimeOffViewModel>();
        
        // Travel ViewModels
        services.AddTransient<MauiHybridApp.ViewModels.TravelRequestViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.TravelRequestFormViewModel>();
        
        // Document ViewModels
        services.AddTransient<MauiHybridApp.ViewModels.DocumentRequestViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.DocumentRequestFormViewModel>();
        
        // Undertime ViewModels
        services.AddTransient<MauiHybridApp.ViewModels.UndertimeRequestViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.UndertimeRequestFormViewModel>();
        
        // Suggestion ViewModels
        services.AddTransient<MauiHybridApp.ViewModels.SuggestionViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.SuggestionFormViewModel>();
        
        // Special Work Schedule ViewModels
        services.AddTransient<MauiHybridApp.ViewModels.SpecialWorkScheduleViewModel>();
        services.AddTransient<MauiHybridApp.ViewModels.SpecialWorkScheduleFormViewModel>();
        
        // State management services for Blazor components
        services.AddScoped<AuthenticationStateService>();
        services.AddScoped<DashboardStateService>();
        services.AddScoped<LeaveStateService>();
        services.AddScoped<OvertimeStateService>();
        services.AddScoped<OfficialBusinessStateService>();
        services.AddScoped<TimeEntryStateService>();
        services.AddScoped<ApprovalStateService>();
        services.AddScoped<AttendanceStateService>();
        services.AddScoped<PayrollStateService>();
        services.AddScoped<PerformanceStateService>();
        services.AddScoped<EmployeeRelationsStateService>();
        services.AddScoped<ProfileStateService>();
        services.AddScoped<ProfileStateService>();
        services.AddScoped<NotificationStateService>();
        
        // Notification ViewModels

        
        // Settings ViewModels
        services.AddTransient<MauiHybridApp.ViewModels.SettingsViewModel>();
        
        // Employee List ViewModel (was missing - caused navigation freeze)
        services.AddTransient<MauiHybridApp.ViewModels.EmployeeListViewModel>();
    }
}

