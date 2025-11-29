using MauiHybridApp.Services;
using MauiHybridApp.Services.Platform;
using MauiHybridApp.Services.SignalR;
using MauiHybridApp.Utils;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace MauiHybridApp;

public partial class App : Application
{
    private readonly ISignalRDataService _signalRService;
    private readonly IPreferenceHelper _preferenceHelper;
    private readonly IDeviceService _deviceService;
    private readonly MauiHybridApp.Services.Platform.IAppCenterService _appCenterService;

    public App(
        ISignalRDataService signalRService,
        IPreferenceHelper preferenceHelper,
        IDeviceService deviceService,
        MauiHybridApp.Services.Platform.IAppCenterService appCenterService)
    {
        InitializeComponent();

        _signalRService = signalRService;
        _preferenceHelper = preferenceHelper;
        _deviceService = deviceService;
        _appCenterService = appCenterService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new MainPage());
    }

    protected override async void OnStart()
    {
        base.OnStart();

        // Initialize AppCenter
        _appCenterService.Initialize();

        // Apply color scheme
        ApplyColorScheme();

        // Set initial login state
        FormSession.IsLoggedIn = false;

        // Store device ID
        var deviceId = _deviceService.GetDeviceId();
        _preferenceHelper.SetDeviceId(deviceId);

        // Start SignalR connection
        try
        {
            await _signalRService.StartConnectionAsync();
        }
        catch (Exception ex)
        {
            // Log error but don't crash the app
            System.Diagnostics.Debug.WriteLine($"SignalR connection failed: {ex.Message}");
        }
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        // Handle when your app sleeps
    }

    protected override void OnResume()
    {
        base.OnResume();
        // Handle when your app resumes
    }

    private void ApplyColorScheme()
    {
        // Apply the color scheme from the original app
        // This will be implemented based on the Extensions.ApplyColorSet() logic
        Application.Current!.UserAppTheme = AppTheme.Light;
    }
}

