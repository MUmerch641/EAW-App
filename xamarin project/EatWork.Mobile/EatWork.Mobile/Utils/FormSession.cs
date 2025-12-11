using EatWork.Mobile.Models;
using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Utils
{
    public class FormSession
    {
        public FormSession()
        {
        }

        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        //UserInfo Settings settings
        private const string IdUserInfo = "UserInfo";

        private static readonly string UserInfoDefault = SetDefaultUser();

        public static string UserInfo
        {
            get
            {
                return AppSettings.GetValueOrDefault(IdUserInfo, UserInfoDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(IdUserInfo, value);
            }
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

        //IsLoggedIn Settings settings
        private const string IdIsLoggedIn = "IsLoggedIn";

        private static readonly bool IsLoggedInDefault = false;

        public static bool IsLoggedIn
        {
            get
            {
                return AppSettings.GetValueOrDefault(IdIsLoggedIn, IsLoggedInDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(IdIsLoggedIn, value);
            }
        }

        //TokenBearer Settings settings
        private const string IdTokenBearer = "TokenBearer";

        private static readonly string TokenBearerDefault = string.Empty;

        public static string TokenBearer
        {
            get
            {
                return AppSettings.GetValueOrDefault(IdTokenBearer, TokenBearerDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(IdTokenBearer, value);
            }
        }

        //MyApprovalSelectedItem Settings settings
        private const string IdMyApprovalSelectedItem = "MyApprovalSelectedItem";

        private static readonly string MyApprovalSelectedItemDefault = SetDefaultMyApprovalSelectedItem();

        public static string MyApprovalSelectedItem
        {
            get
            {
                return AppSettings.GetValueOrDefault(IdMyApprovalSelectedItem, MyApprovalSelectedItemDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(IdMyApprovalSelectedItem, value);
            }
        }

        private static string SetDefaultMyApprovalSelectedItem()
        {
            return JsonConvert.SerializeObject(new MyApprovalListModel());
        }

        //MyApprovalSelectedItemUpdated Settings settings
        private const string IdMyApprovalSelectedItemUpdated = "MyApprovalSelectedItemUpdated";

        private static readonly bool MyApprovalSelectedItemUpdatedDefault = false;

        public static bool MyApprovalSelectedItemUpdated
        {
            get
            {
                return AppSettings.GetValueOrDefault(IdMyApprovalSelectedItemUpdated, MyApprovalSelectedItemUpdatedDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(IdMyApprovalSelectedItemUpdated, value);
            }
        }

        //MyApprovalSelectedItemUpdated Settings settings
        private const string IdMyApprovalSelectedItemStatus = "MyApprovalSelectedItemStatus";

        private static readonly string MyApprovalSelectedItemStatusDefault = SetDefaultMyApprovalSelectedItemStatus();

        public static string MyApprovalSelectedItemStatus
        {
            get
            {
                return AppSettings.GetValueOrDefault(IdMyApprovalSelectedItemStatus, MyApprovalSelectedItemStatusDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(IdMyApprovalSelectedItemStatus, value);
            }
        }

        public static string SetDefaultMyApprovalSelectedItemStatus(string retValue = "")
        {
            if (!string.IsNullOrWhiteSpace(retValue))
            {
                switch (retValue)
                {
                    case Contants.ActionType.Disapprove:
                        retValue = Contants.RequestStatus.Disapproved;
                        break;

                    case Contants.ActionType.Approve:
                        retValue = Contants.RequestStatus.Approved;
                        break;

                    case Contants.ActionType.ForApproval:
                        retValue = Contants.RequestStatus.ForApproval;
                        break;

                    case Contants.ActionType.Cancel:
                        retValue = Contants.RequestStatus.Cancelled;
                        break;

                    case Contants.ActionType.Draft:
                        retValue = Contants.RequestStatus.Draft;
                        break;

                    case Contants.ActionType.PartiallyApprove:
                        retValue = Contants.RequestStatus.PartiallyApproved;
                        break;
                }
            }

            return retValue;
        }

        //IsLoggedIn Settings settings
        private const string IdIsSubmitted = "IsSubmitted";

        private static readonly bool IsSubmittedDefaultValue = false;

        public static bool IsSubmitted
        {
            get
            {
                return AppSettings.GetValueOrDefault(IdIsSubmitted, IsSubmittedDefaultValue);
            }
            set
            {
                AppSettings.AddOrUpdateValue(IdIsSubmitted, value);
            }
        }

        //Boardings Settings settings
        private const string IdBoardings = "Boardings";

        private static readonly string BoardingsDefault = SetDefaultBoardings();

        public static string Boardings
        {
            get
            {
                return AppSettings.GetValueOrDefault(IdBoardings, BoardingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(IdBoardings, value);
            }
        }

        public static string SetDefaultBoardings(ObservableCollection<Boarding> collection = null)
        {
            var retValue = new ObservableCollection<Boarding>();

            if (collection != null)
                retValue = collection;

            return JsonConvert.SerializeObject(retValue, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        //GET MIMETYPES
        //MimeTypes settings
        private const string IdMimeTypes = "MimeTypes";

        private static readonly string MimeTypesDefault = SetDefaultMimeTypes();

        public static string MimeTypes
        {
            get
            {
                return AppSettings.GetValueOrDefault(IdMimeTypes, MimeTypesDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(IdMimeTypes, value);
            }
        }

        public static string SetDefaultMimeTypes(MimeType collection = null)
        {
            var retValue = new MimeType();

            if (collection != null)
                retValue = collection;

            return JsonConvert.SerializeObject(retValue, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        //MyScheduleSelectedDate Settings settings
        private const string IdMyScheduleSelectedDate = "MyScheduleSelectedDate";

        private static readonly string MyScheduleSelectedDateDefault = DateTime.Now.ToString("MM/dd/yyyy");

        public static string MyScheduleSelectedDate
        {
            get
            {
                return AppSettings.GetValueOrDefault(IdMyScheduleSelectedDate, MyScheduleSelectedDateDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(IdMyScheduleSelectedDate, value);
            }
        }

        //MyScheduleSelectedDate Settings settings
        private const string IdIsMySchedule = "IsMySchedule";

        private static readonly bool IsMyScheduleDefault = false;

        public static bool IsMySchedule
        {
            get
            {
                return AppSettings.GetValueOrDefault(IdIsMySchedule, IsMyScheduleDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(IdIsMySchedule, value);
            }
        }

        //MyScheduleSelectedDate Settings settings
        private const string IdIsEditTimeLogs = "IsEditTimeLogs";

        private static readonly bool IsEditTimeLogsDefault = false;

        public static bool IsEditTimeLogs
        {
            get
            {
                return AppSettings.GetValueOrDefault(IdIsEditTimeLogs, IsEditTimeLogsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(IdIsEditTimeLogs, value);
            }
        }

        //END

        public static void ClearApprovalCached()
        {
            MyApprovalSelectedItem = MyApprovalSelectedItemDefault;
            MyApprovalSelectedItemStatus = MyApprovalSelectedItemStatusDefault;
            MyApprovalSelectedItemUpdated = MyApprovalSelectedItemUpdatedDefault;
        }

        public static void ClearEverything()
        {
            AppSettings.Clear();
        }
    }
}