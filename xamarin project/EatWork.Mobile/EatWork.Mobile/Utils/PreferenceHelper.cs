using EatWork.Mobile.Models;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xamarin.Essentials;
using API = EAW.API.DataContracts.Models;

namespace EatWork.Mobile.Utils
{
    public class PreferenceHelper
    {
        public static void CRLimitCompany(int value)
        {
            Preferences.Set("CRLimitCompany", value);
        }

        public static int CRLimitCompany()
        {
            return Preferences.Get("CRLimitCompany", 0);
        }

        public static void CRLimitBranch(int value)
        {
            Preferences.Set("CRLimitBranch", value);
        }

        public static int CRLimitBranch()
        {
            return Preferences.Get("CRLimitBranch", 0);
        }

        public static void CRLimitDepartment(int value)
        {
            Preferences.Set("CRLimitDepartment", value);
        }

        public static int CRLimitDepartment()
        {
            return Preferences.Get("CRLimitDepartment", 0);
        }

        public static void CRLimitTeam(int value)
        {
            Preferences.Set("CRLimitTeam", value);
        }

        public static int CRLimitTeam()
        {
            return Preferences.Get("CRLimitTeam", 0);
        }

        public static void LoginImageSetup(string value)
        {
            Preferences.Set("LoginImageSetup", value);
        }

        public static string LoginImageSetup()
        {
            return Preferences.Get("LoginImageSetup", string.Empty);
        }

        public static void HomeScreenSetup(string value)
        {
            Preferences.Set("HomeScreenSetup", value);
        }

        public static string HomeScreenSetup()
        {
            return Preferences.Get("HomeScreenSetup", string.Empty);
        }

        public static void SplashScreenSetup(string value)
        {
            Preferences.Set("SplashScreenSetup", value);
        }

        public static string SplashScreenSetup()
        {
            return Preferences.Get("SplashScreenSetup", string.Empty);
        }

        public static void MaxFileSize(int value)
        {
            Preferences.Set("MaxFileSize", value);
        }

        public static int MaxFileSize()
        {
            return Preferences.Get("MaxFileSize", 0);
        }

        public static UserModel UserInfo()
        {
            return JsonConvert.DeserializeObject<UserModel>(FormSession.UserInfo);
        }

        public static void DateFormatSetup(string value)
        {
            Preferences.Set("DateFormatSetup", value);
        }

        public static string DateFormatSetup()
        {
            return Preferences.Get("DateFormatSetup", "MM/dd/yyyy");
        }

        public static void LeaveFormContinue(bool value)
        {
            Preferences.Set("LeaveFormContinue", value);
        }

        public static bool LeaveFormContinue()
        {
            return Preferences.Get("LeaveFormContinue", false);
        }

        public static void WarningPageClosed(bool value)
        {
            Preferences.Set("WarningPageClosed", value);
        }

        public static bool WarningPageClosed()
        {
            return Preferences.Get("WarningPageClosed", false);
        }

        public static void UserId(long val)
        {
            Preferences.Set("UserId", val);
        }

        public static long UserId()
        {
            return Preferences.Get("UserId", default(long));
        }

        public static void IsFirstLogin(bool val)
        {
            Preferences.Set("IsFirstLogin", val);
        }

        public static bool IsFirstLogin()
        {
            return Preferences.Get("IsFirstLogin", true);
        }

        public static void DeviceId(string val)
        {
            Preferences.Set("DeviceId", val);
        }

        public static string DeviceId()
        {
            return Preferences.Get("DeviceId", string.Empty);
        }
    }

    public class IndividualObjectiveHelper
    {
        public static void ObjectiveDetailChanged(bool val)
        {
            Preferences.Set("ObjectiveDetailChanged", val);
        }

        public static bool ObjectiveDetailChanged()
        {
            return Preferences.Get("ObjectiveDetailChanged", false);
        }

        public static void ObjectiveDetailDto(ObjectiveDetailDto value)
        {
            Preferences.Set("ObjectiveDetailDto", JsonConvert.SerializeObject(value));
        }

        public static ObjectiveDetailDto ObjectiveDetailDto()
        {
            return JsonConvert.DeserializeObject<ObjectiveDetailDto>(Preferences.Get("ObjectiveDetailDto", ""));
        }

        public static void SelectedGoalDetailDto(GoalDetailDto value)
        {
            Preferences.Set("SelectedGoalDetailDto", JsonConvert.SerializeObject(value));
        }

        public static GoalDetailDto SelectedGoalDetailDto()
        {
            return JsonConvert.DeserializeObject<GoalDetailDto>(Preferences.Get("SelectedGoalDetailDto", ""));
        }

        public static void SelectedGoalDetailDtoChanged(bool val)
        {
            Preferences.Set("SelectedGoalDetailDtoChanged", val);
        }

        public static bool SelectedGoalDetailDtoChanged()
        {
            return Preferences.Get("SelectedGoalDetailDtoChanged", false);
        }

        public static void StandardObjectiveListChanged(bool val)
        {
            Preferences.Set("StandardObjectiveListChanged", val);
        }

        public static bool StandardObjectiveListChanged()
        {
            return Preferences.Get("StandardObjectiveListChanged", false);
        }

        public static void StandardObjectiveList(List<API.PerformanceObjectiveDetailDto> value)
        {
            Preferences.Set("StandardObjectiveList", JsonConvert.SerializeObject(value));
        }

        public static List<API.PerformanceObjectiveDetailDto> StandardObjectiveList()
        {
            return JsonConvert.DeserializeObject<List<API.PerformanceObjectiveDetailDto>>(Preferences.Get("StandardObjectiveList", ""));
        }
    }

    public class MenuHelper
    {
        public static void Menus(List<API.MobileMenuDto> value)
        {
            Preferences.Set("PackageMenu", JsonConvert.SerializeObject(value));
        }

        public static List<API.MobileMenuDto> Menus()
        {
            return JsonConvert.DeserializeObject<List<API.MobileMenuDto>>(Preferences.Get("PackageMenu", ""));
        }

        public static void SubMenus(List<API.MobileSubMenuDto> value)
        {
            Preferences.Set("PackageSubMenu", JsonConvert.SerializeObject(value));
        }

        public static List<API.MobileSubMenuDto> SubMenus()
        {
            return JsonConvert.DeserializeObject<List<API.MobileSubMenuDto>>(Preferences.Get("PackageSubMenu", ""));
        }

        public static void Forms(List<API.MobileFormDto> value)
        {
            Preferences.Set("PackageForms", JsonConvert.SerializeObject(value));
        }

        public static List<API.MobileFormDto> Forms()
        {
            return JsonConvert.DeserializeObject<List<API.MobileFormDto>>(Preferences.Get("PackageForms", ""));
        }

        public static void ClockWork(bool value)
        {
            Preferences.Set("ClockWork", value);
        }

        public static bool ClockWork()
        {
            return Preferences.Get("ClockWork", false);
        }
    }
}