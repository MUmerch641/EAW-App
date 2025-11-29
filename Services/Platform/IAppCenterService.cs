using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace MauiHybridApp.Services.Platform;

public interface IAppCenterService
{
    void Initialize();
    void TrackEvent(string eventName, Dictionary<string, string>? properties = null);
    void TrackError(Exception exception, Dictionary<string, string>? properties = null);
}

public class AppCenterService : IAppCenterService
{
    private bool _isInitialized = false;

    public void Initialize()
    {
        if (_isInitialized) return;

        var appCenterSecret = "ios=9d941729-db55-4aba-b130-93884db353de;" +
                             "android=d0b6a032-2dd0-4528-96b6-735f21d4e52f";

        AppCenter.Start(appCenterSecret, typeof(Analytics), typeof(Crashes));
        _isInitialized = true;
    }

    public void TrackEvent(string eventName, Dictionary<string, string>? properties = null)
    {
        if (!_isInitialized) return;
        Analytics.TrackEvent(eventName, properties);
    }

    public void TrackError(Exception exception, Dictionary<string, string>? properties = null)
    {
        if (!_isInitialized) return;
        Crashes.TrackError(exception, properties);
    }
}

