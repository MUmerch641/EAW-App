using MauiHybridApp.Models;

namespace MauiHybridApp.Services;

public interface IPreferenceHelper
{
    void SetCRLimitCompany(int value);
    int GetCRLimitCompany();
    
    void SetCRLimitBranch(int value);
    int GetCRLimitBranch();
    
    void SetCRLimitDepartment(int value);
    int GetCRLimitDepartment();
    
    void SetCRLimitTeam(int value);
    int GetCRLimitTeam();
    
    void SetLoginImageSetup(string value);
    string GetLoginImageSetup();
    
    void SetHomeScreenSetup(string value);
    string GetHomeScreenSetup();
    
    void SetSplashScreenSetup(string value);
    string GetSplashScreenSetup();
    
    void SetMaxFileSize(int value);
    int GetMaxFileSize();
    
    UserModel GetUserInfo();
    
    void SetDateFormatSetup(string value);
    string GetDateFormatSetup();
    
    void SetLeaveFormContinue(bool value);
    bool GetLeaveFormContinue();
    
    void SetWarningPageClosed(bool value);
    bool GetWarningPageClosed();
    
    void SetUserId(long val);
    long GetUserId();
    
    void SetIsFirstLogin(bool val);
    bool GetIsFirstLogin();
    
    void SetDeviceId(string val);
    string GetDeviceId();
}

public class PreferenceHelper : IPreferenceHelper
{
    public void SetCRLimitCompany(int value) => Preferences.Set("CRLimitCompany", value);
    public int GetCRLimitCompany() => Preferences.Get("CRLimitCompany", 0);

    public void SetCRLimitBranch(int value) => Preferences.Set("CRLimitBranch", value);
    public int GetCRLimitBranch() => Preferences.Get("CRLimitBranch", 0);

    public void SetCRLimitDepartment(int value) => Preferences.Set("CRLimitDepartment", value);
    public int GetCRLimitDepartment() => Preferences.Get("CRLimitDepartment", 0);

    public void SetCRLimitTeam(int value) => Preferences.Set("CRLimitTeam", value);
    public int GetCRLimitTeam() => Preferences.Get("CRLimitTeam", 0);

    public void SetLoginImageSetup(string value) => Preferences.Set("LoginImageSetup", value);
    public string GetLoginImageSetup() => Preferences.Get("LoginImageSetup", string.Empty);

    public void SetHomeScreenSetup(string value) => Preferences.Set("HomeScreenSetup", value);
    public string GetHomeScreenSetup() => Preferences.Get("HomeScreenSetup", string.Empty);

    public void SetSplashScreenSetup(string value) => Preferences.Set("SplashScreenSetup", value);
    public string GetSplashScreenSetup() => Preferences.Get("SplashScreenSetup", string.Empty);

    public void SetMaxFileSize(int value) => Preferences.Set("MaxFileSize", value);
    public int GetMaxFileSize() => Preferences.Get("MaxFileSize", 0);

    public UserModel GetUserInfo()
    {
        var userJson = Utils.FormSession.UserInfo;
        return Newtonsoft.Json.JsonConvert.DeserializeObject<UserModel>(userJson) ?? new UserModel();
    }

    public void SetDateFormatSetup(string value) => Preferences.Set("DateFormatSetup", value);
    public string GetDateFormatSetup() => Preferences.Get("DateFormatSetup", "MM/dd/yyyy");

    public void SetLeaveFormContinue(bool value) => Preferences.Set("LeaveFormContinue", value);
    public bool GetLeaveFormContinue() => Preferences.Get("LeaveFormContinue", false);

    public void SetWarningPageClosed(bool value) => Preferences.Set("WarningPageClosed", value);
    public bool GetWarningPageClosed() => Preferences.Get("WarningPageClosed", false);

    public void SetUserId(long val) => Preferences.Set("UserId", val);
    public long GetUserId() => Preferences.Get("UserId", default(long));

    public void SetIsFirstLogin(bool val) => Preferences.Set("IsFirstLogin", val);
    public bool GetIsFirstLogin() => Preferences.Get("IsFirstLogin", true);

    public void SetDeviceId(string val) => Preferences.Set("DeviceId", val);
    public string GetDeviceId() => Preferences.Get("DeviceId", string.Empty);
}

