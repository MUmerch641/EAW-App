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

#if DEBUG
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
            client.Timeout = TimeSpan.FromSeconds(30);
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
        services.AddScoped<ILeaveDataService, LeaveDataService>();
        services.AddScoped<IOvertimeDataService, OvertimeDataService>();
        services.AddScoped<IOfficialBusinessDataService, OfficialBusinessDataService>();
        services.AddScoped<ITimeEntryDataService, TimeEntryDataService>();
        services.AddScoped<IApprovalDataService, ApprovalDataService>();
        services.AddScoped<IAttendanceDataService, AttendanceDataService>();
        services.AddScoped<IPayrollDataService, PayrollDataService>();
        services.AddScoped<IPerformanceDataService, PerformanceDataService>();
        services.AddScoped<IEmployeeRelationsDataService, EmployeeRelationsDataService>();
        services.AddScoped<IUserDataService, UserDataService>();
        services.AddScoped<INotificationDataService, NotificationDataService>();

        // Additional services
        services.AddSingleton<ISQLiteDataService, SQLiteDataService>();
        services.AddSingleton<IFileUploadService, FileUploadService>();

        // Performance services
        services.AddSingleton<MauiHybridApp.Services.Performance.ICacheService, MauiHybridApp.Services.Performance.CacheService>();
        services.AddSingleton<MauiHybridApp.Services.Performance.IPerformanceService, MauiHybridApp.Services.Performance.PerformanceService>();

        // SignalR and State services
        services.AddSingleton<MauiHybridApp.Services.SignalR.ISignalRDataService, MauiHybridApp.Services.SignalR.SignalRDataService>();
        services.AddSingleton<MauiHybridApp.Services.State.IStateService, MauiHybridApp.Services.State.StateService>();
    }

    private static void RegisterStateServices(IServiceCollection services)
    {
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
        services.AddScoped<NotificationStateService>();
    }
}

