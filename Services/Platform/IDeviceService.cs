namespace MauiHybridApp.Services.Platform;

public interface IDeviceService
{
    string GetDeviceId();
    string GetDeviceModel();
    string GetDevicePlatform();
    string GetDeviceVersion();
    string GetAppVersion();
}

public class DeviceService : IDeviceService
{
    public string GetDeviceId()
    {
        // Generate a unique device ID or retrieve from secure storage
        var deviceId = Preferences.Get("DeviceUniqueId", string.Empty);
        if (string.IsNullOrEmpty(deviceId))
        {
            deviceId = Guid.NewGuid().ToString();
            Preferences.Set("DeviceUniqueId", deviceId);
        }
        return deviceId;
    }

    public string GetDeviceModel()
    {
        return DeviceInfo.Model;
    }

    public string GetDevicePlatform()
    {
        return DeviceInfo.Platform.ToString();
    }

    public string GetDeviceVersion()
    {
        return DeviceInfo.VersionString;
    }

    public string GetAppVersion()
    {
        return AppInfo.VersionString;
    }
}

