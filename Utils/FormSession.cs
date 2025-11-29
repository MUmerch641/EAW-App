using MauiHybridApp.Models;
using Newtonsoft.Json;

namespace MauiHybridApp.Utils;

public static class FormSession
{
    // UserInfo Settings
    private const string IdUserInfo = "UserInfo";
    private static readonly string UserInfoDefault = SetDefaultUser();

    public static string UserInfo
    {
        get => Preferences.Get(IdUserInfo, UserInfoDefault);
        set => Preferences.Set(IdUserInfo, value);
    }

    private static string SetDefaultUser()
    {
        return JsonConvert.SerializeObject(new UserModel
        {
            UserSecurityId = 0,
            EmployeeName = string.Empty,
            ProfileId = 0,
            EmployeeNo = string.Empty,
            EmailAddress = string.Empty,
            Username = string.Empty,
        });
    }

    // IsLoggedIn Settings
    private const string IdIsLoggedIn = "IsLoggedIn";
    private static readonly bool IsLoggedInDefault = false;

    public static bool IsLoggedIn
    {
        get => Preferences.Get(IdIsLoggedIn, IsLoggedInDefault);
        set => Preferences.Set(IdIsLoggedIn, value);
    }

    // TokenBearer Settings
    private const string IdTokenBearer = "TokenBearer";
    private static readonly string TokenBearerDefault = string.Empty;

    public static string TokenBearer
    {
        get => Preferences.Get(IdTokenBearer, TokenBearerDefault);
        set => Preferences.Set(IdTokenBearer, value);
    }

    // MyApprovalSelectedItemUpdated Settings
    private const string IdMyApprovalSelectedItemUpdated = "MyApprovalSelectedItemUpdated";
    private static readonly bool MyApprovalSelectedItemUpdatedDefault = false;

    public static bool MyApprovalSelectedItemUpdated
    {
        get => Preferences.Get(IdMyApprovalSelectedItemUpdated, MyApprovalSelectedItemUpdatedDefault);
        set => Preferences.Set(IdMyApprovalSelectedItemUpdated, value);
    }

    // MyApprovalSelectedItemStatus Settings
    private const string IdMyApprovalSelectedItemStatus = "MyApprovalSelectedItemStatus";
    private static readonly string MyApprovalSelectedItemStatusDefault = string.Empty;

    public static string MyApprovalSelectedItemStatus
    {
        get => Preferences.Get(IdMyApprovalSelectedItemStatus, MyApprovalSelectedItemStatusDefault);
        set => Preferences.Set(IdMyApprovalSelectedItemStatus, value);
    }

    public static string SetDefaultMyApprovalSelectedItemStatus(string retValue = "")
    {
        if (!string.IsNullOrWhiteSpace(retValue))
        {
            retValue = retValue switch
            {
                ActionType.Disapprove => RequestStatus.Disapproved,
                ActionType.Approve => RequestStatus.Approved,
                ActionType.ForApproval => RequestStatus.ForApproval,
                ActionType.Cancel => RequestStatus.Cancelled,
                ActionType.Draft => RequestStatus.Draft,
                ActionType.PartiallyApprove => RequestStatus.PartiallyApproved,
                _ => retValue
            };
        }

        return retValue;
    }

    // IsSubmitted Settings
    private const string IdIsSubmitted = "IsSubmitted";
    private static readonly bool IsSubmittedDefaultValue = false;

    public static bool IsSubmitted
    {
        get => Preferences.Get(IdIsSubmitted, IsSubmittedDefaultValue);
        set => Preferences.Set(IdIsSubmitted, value);
    }

    // MyScheduleSelectedDate Settings
    private const string IdMyScheduleSelectedDate = "MyScheduleSelectedDate";
    private static readonly string MyScheduleSelectedDateDefault = DateTime.Now.ToString("MM/dd/yyyy");

    public static string MyScheduleSelectedDate
    {
        get => Preferences.Get(IdMyScheduleSelectedDate, MyScheduleSelectedDateDefault);
        set => Preferences.Set(IdMyScheduleSelectedDate, value);
    }

    // IsMySchedule Settings
    private const string IdIsMySchedule = "IsMySchedule";
    private static readonly bool IsMyScheduleDefault = false;

    public static bool IsMySchedule
    {
        get => Preferences.Get(IdIsMySchedule, IsMyScheduleDefault);
        set => Preferences.Set(IdIsMySchedule, value);
    }

    // IsEditTimeLogs Settings
    private const string IdIsEditTimeLogs = "IsEditTimeLogs";
    private static readonly bool IsEditTimeLogsDefault = false;

    public static bool IsEditTimeLogs
    {
        get => Preferences.Get(IdIsEditTimeLogs, IsEditTimeLogsDefault);
        set => Preferences.Set(IdIsEditTimeLogs, value);
    }

    public static void ClearApprovalCached()
    {
        MyApprovalSelectedItemStatus = MyApprovalSelectedItemStatusDefault;
        MyApprovalSelectedItemUpdated = MyApprovalSelectedItemUpdatedDefault;
    }

    public static void ClearEverything()
    {
        Preferences.Clear();
    }
}

