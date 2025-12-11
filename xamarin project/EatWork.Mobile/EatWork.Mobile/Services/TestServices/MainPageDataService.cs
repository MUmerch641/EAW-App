using Acr.UserDialogs;
using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataAccess;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Utils.DataAccess;
using EatWork.Mobile.Views;
using EatWork.Mobile.Views.AttendanceViewTemplate2;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using DEVICE = Plugin.DeviceInfo;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services.TestServices
{
    public class MainPageDataService : IMainPageDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly ThemeDataAccess themeDataAccess_;
        private readonly ClientSetupDataAccess clientSetup_;
        private readonly UserDeviceInfoDataAccess userDeviceInfoDataAccess_;
        private readonly IDialogService dialogService_;

        public MainPageDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            ThemeDataAccess themeDataAccess)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            themeDataAccess_ = themeDataAccess;
            clientSetup_ = AppContainer.Resolve<ClientSetupDataAccess>();
            dialogService_ = AppContainer.Resolve<IDialogService>();
            userDeviceInfoDataAccess_ = AppContainer.Resolve<UserDeviceInfoDataAccess>();
        }

        public async Task<ObservableCollection<MenuItemModel>> InitMenuList()
        {
            try
            {
                var client = await clientSetup_.RetrieveClientSetup();
                await RetrievePackageSetup(client.ClientCode);

                var configMenu = MenuHelper.Menus();
                var configSubMenu = MenuHelper.SubMenus();

                #region submenus

                var attendance = new ObservableCollection<MenuItemModel>();
                var payroll = new ObservableCollection<MenuItemModel>();
                var expenses = new ObservableCollection<MenuItemModel>();
                var performance = new ObservableCollection<MenuItemModel>();
                var employeerelations = new ObservableCollection<MenuItemModel>();

                var submenus = new ObservableCollection<MenuItemModel>()
                {
                    new MenuItemModel { Id = MenuItemType.MySchedule, Icon="far-calendar-alt", Title="My Schedule", TargetType = typeof(MySchedulePage),MenuGroupId = MenuGroup.Attendance },
                    new MenuItemModel { Id = MenuItemType.MyTimeLog, Icon="far-clock", Title="My Time Logs", TargetType = typeof(MyTimeLogsPage),MenuGroupId = MenuGroup.Attendance  },
                    new MenuItemModel { Id = MenuItemType.MyAttendance, Icon="fas-user-clock", Title="My Attendance", TargetType = typeof(IndividualAttendancePage),MenuGroupId = MenuGroup.Attendance },
                    new MenuItemModel { Id = MenuItemType.MyAttendance, Icon="fas-user-clock", Title="Detailed Attendance", TargetType = typeof(AttendanceTemplate2ListPage),MenuGroupId = MenuGroup.Attendance },
                    new MenuItemModel { Id = MenuItemType.MyPayslip, Icon="fas-money-check-alt", Title="My Payslips", TargetType = typeof(Views.Payslips.MyPayslipPage),MenuGroupId = MenuGroup.Payroll},
                    new MenuItemModel { Id = MenuItemType.YTDPayslipTemplate, Icon="fas-money-check-alt", Title="My Payslips (YTD Breakdown)", TargetType = typeof(Views.Payslips.YTDOTPaymentBreakdownListPage),MenuGroupId = MenuGroup.Payroll},
                    new MenuItemModel { Id = MenuItemType.CashAdvanceRequest, Icon="far-money-bill-alt", Title="Cash Advance Request", TargetType = typeof(Views.CashAdvance.CashAdvancesPage),MenuGroupId = MenuGroup.Expenses },
                    new MenuItemModel { Id = MenuItemType.MyExpenses, Icon="fas-receipt", Title="My Expenses", TargetType = typeof(Views.Expenses.MyExpensesPage),MenuGroupId = MenuGroup.Expenses },
                    new MenuItemModel { Id = MenuItemType.MyExpenses, Icon="fas-file-invoice-dollar", Title="My Expense Reports", TargetType = typeof(Views.Expenses.MyExpenseReportsPage),MenuGroupId = MenuGroup.Expenses},
                    new MenuItemModel { Id = MenuItemType.IndividualObjectives, Icon="fas-user-edit", Title="Individual Objectives", TargetType = typeof(Views.IndividualObjectives.IndividualObjectivesPage),MenuGroupId = MenuGroup.Performance},
                    new MenuItemModel { Id = MenuItemType.PerformanceEvaluation, Icon="fas-chalkboard-teacher", Title="Performance Evaluation", TargetType = typeof(Views.PerformanceEvaluation.PerformanceEvaluationListPage),MenuGroupId = MenuGroup.Performance},
                    new MenuItemModel { Id = MenuItemType.SuggestionCorner, Icon="far-comment", Title="Suggestion Corner", TargetType = typeof(Views.SuggestionCorner.SuggestionsPage),MenuGroupId = MenuGroup.EmployeeRelations},
                };

                foreach (var item in submenus.ToList())
                {
#if CLIENT_DEBUG
                    var eType = (MenuItemType)item.MenuGroupId;
                    if (eType == MenuItemType.Attendance)
                        attendance.Add(item);

                    if (eType == MenuItemType.Payroll)
                        payroll.Add(item);

                    if (eType == MenuItemType.Expenses)
                        expenses.Add(item);

                    if (eType == MenuItemType.PerformanceEvaluation)
                        performance.Add(item);

                    if (eType == MenuItemType.EmployeeRelations)
                        employeerelations.Add(item);
#else
                    var eType = (MenuItemType)item.Id;
                    var cMenu = configSubMenu.FirstOrDefault(x => x.SubMenuCode == eType.ToString());
                    if (cMenu != null)
                    {
                        Enum.TryParse<MenuItemType>(cMenu.MenuCode, out MenuItemType menuType);

                        if (menuType == MenuItemType.Attendance)
                            attendance.Add(item);

                        if (menuType == MenuItemType.Payroll)
                            payroll.Add(item);

                        if (menuType == MenuItemType.Expenses)
                            expenses.Add(item);

                        if (menuType == MenuItemType.PerformanceEvaluation)
                            performance.Add(item);

                        if (menuType == MenuItemType.EmployeeRelations)
                            employeerelations.Add(item);
                    }
#endif
                }

                #endregion submenus

                var menus = new ObservableCollection<MenuItemModel>()
                {
                    new MenuItemModel { Id = MenuItemType.Home, Icon="fas-home", Title="Home", TargetType = typeof(DashboardPage), IsDefault = true},
                    new MenuItemModel { Id = MenuItemType.MyRequest, Icon="far-list-alt", Title="My Requests", TargetType = typeof(MyRequestsPage)},
                    new MenuItemModel { Id = MenuItemType.MyApproval, Icon="fas-user-check", Title="My Approvals", TargetType = typeof(MyApprovalPage) },
                    new MenuItemModel { Id = MenuItemType.Attendance, Icon="\uf017", Title="Attendance", TargetType = typeof(MyTimeLogsPage), TrailingIcon = "fas-chevron-right", SubItems = attendance,IsVisible= true },
                    new MenuItemModel { Id = MenuItemType.Payroll, Icon="\uf3d1", Title="Payroll", TrailingIcon = "fas-chevron-right", SubItems = payroll,IsVisible= true },
                    new MenuItemModel { Id = MenuItemType.Expenses, Icon="\uf571", Title="Expenses", TrailingIcon = "fas-chevron-right", SubItems = expenses,IsVisible= true },
                    new MenuItemModel { Id = MenuItemType.PerformanceEvaluation, Icon="\uf51c", Title="Performance", TrailingIcon = "fas-chevron-right", SubItems = performance,IsVisible= true },
                    new MenuItemModel { Id = MenuItemType.EmployeeRelations, Icon="\uf500", Title="Employee Relations", TrailingIcon = "fas-chevron-right", SubItems = employeerelations,IsVisible= true },
                    new MenuItemModel { Id = MenuItemType.Settings, Icon="fas-cogs", Title="Settings", TargetType = typeof(SettingsPage), IsDefault = true },
                    new MenuItemModel { Id = MenuItemType.LogOut, Icon="fas-power-off", Title="Sign out", IsDefault = true},
                };

#if !CLIENT_DEBUG
                foreach (var xitem in menus.Where(x => x.IsDefault == false).ToList())
                {
                    var eType = (MenuItemType)xitem.Id;
                    var cMenu = configMenu.Where(x => x.MenuCode == eType.ToString()).FirstOrDefault();

                    if (cMenu == null)
                        menus.Remove(xitem);
                }
#endif

                return await Task.FromResult(new ObservableCollection<MenuItemModel>(menus));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }
        }

        public async Task<ObservableCollection<Boarding>> GetBoardingList()
        {
            var retValue = new ObservableCollection<Boarding>();

            try
            {
                await commonDataService_.HasInternetConnection(ApiConstants.BaseApiUrl);

                var builder = new UriBuilder(ApiConstants.BaseApiUrl)
                {
                    Path = ApiConstants.GetWalkthrough
                };

                var response = await genericRepository_.GetAsync<string>(builder.ToString());

                if (!string.IsNullOrWhiteSpace(response))
                {
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Boardings>(response);

                    foreach (var item in data.Boarding)
                    {
                        retValue.Add(new Boarding()
                        {
                            ImagePath = item.ImagePath,
                            Header = item.Header,
                            Content = item.Content,
                            RotatorItem = new WalkthroughItemPage()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retValue;
        }

        public async Task<MimeTypes> GetMimeTypeList()
        {
            var retValue = new MimeTypes();
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = ApiConstants.GetMimeType
                };

                var response = await genericRepository_.GetAsync<string>(builder.ToString());

                if (!string.IsNullOrWhiteSpace(response))
                {
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<MimeType>(response);

                    retValue = new MimeTypes()
                    {
                        Android = data.MimeTypes.Android,
                        iOS = data.MimeTypes.iOS,
                        UWP = data.MimeTypes.UWP
                    };

                    FormSession.MimeTypes = response;
                    //FormSession.MimeTypes = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                }

                await GetMaxFileSize();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " : " + ex.Message}");
            }

            return retValue;
        }

        public async Task<ThemeConfigModel> GetThemeSetup(long setupId)
        {
            var retValue = new ThemeConfigModel();
            try
            {
                await commonDataService_.HasInternetConnection(ApiConstants.BaseApiUrl);

                var builder = new UriBuilder(ApiConstants.BaseApiUrl)
                {
                    Path = string.Format(ApiConstants.GetThemeSetup, setupId)
                };

                var response = await genericRepository_.GetAsync<R.Responses.ThemeConfigResponse>(builder.ToString());

                if (response.IsSuccess)
                {
                    var data = new ThemeConfigDataModel();
                    PropertyCopier<R.Models.ThemeConfig, ThemeConfigModel>.Copy(response.ThemeConfig, retValue);
                    PropertyCopier<R.Models.ThemeConfig, ThemeConfigDataModel>.Copy(response.ThemeConfig, data);

                    await themeDataAccess_.DeleteSetup();
                    await themeDataAccess_.SaveRecord(data);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " : " + ex.Message}");
            }

            return retValue;
        }

        public async Task<int> GetMaxFileSize()
        {
            var retValue = 0;
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = ApiConstants.GetMaxFileSize
                };

                var response = await genericRepository_.GetAsync<int>(builder.ToString());

                if (response > 0)
                {
                    PreferenceHelper.MaxFileSize(response);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " : " + ex.Message}");
            }

            return retValue;
        }

        public async Task SaveDeviceInfo()
        {
            try
            {
                /*
                var url = await commonDataService_.RetrieveClientUrl();
                var setup = await clientSetup_.RetrieveClientSetup();

                var builder = new UriBuilder(url)
                {
                    Path = ApiConstants.Device
                };
                */
                /*var id = DEVICE.CrossDeviceInfo.Current.GenerateAppId();*/

                var setup = await clientSetup_.RetrieveClientSetup();

                var builder = new UriBuilder(ApiConstants.BaseApiUrl)
                {
                    Path = ApiConstants.Device
                };

                var param = new R.Requests.SubmitDeviceInfoRequest()
                {
                    Data = new R.Models.DeviceInfo()
                    {
                        ClientId = setup.ClientId,
                        DeviceIdiom = DeviceInfo.Idiom.ToString(),
                        DeviceModel = DeviceInfo.Model,
                        DeviceName = DeviceInfo.Name,
                        DevicePlatform = DeviceInfo.Platform.ToString(),
                        DeviceType = DeviceInfo.DeviceType.ToString(),
                        DeviceVersion = DeviceInfo.VersionString,
                        DeviceId = DEVICE.CrossDeviceInfo.Current.Id,
                    }
                };

                await genericRepository_.PostAsync<R.Requests.SubmitDeviceInfoRequest, R.Responses.BaseResponse<R.Models.DeviceInfo>>(builder.ToString(), param);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " : " + ex.Message}");
            }
        }

        public async Task CheckLatestVersion()
        {
            try
            {
                var isFirstLunch = VersionTracking.IsFirstLaunchEver;

                if (!isFirstLunch)
                {
                    var response = string.Empty;

                    //1.0.0.1
                    var currentVersion = VersionTracking.CurrentVersion;

                    //1
                    var currentBuild = VersionTracking.CurrentBuild;

                    //get version info
                    var setup = await clientSetup_.RetrieveClientSetup();

                    var appVersion = string.Empty;
                    var display = string.Empty;
                    bool isProd = false;
                    using (Acr.UserDialogs.UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);

                        var builder = new UriBuilder(ApiConstants.BaseApiUrl)
                        {
                            Path = $"{ApiConstants.Device}/get-version"
                        };

                        response = await genericRepository_.GetAsync<string>(builder.ToString());

                        if (!string.IsNullOrWhiteSpace(response))
                        {
                            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<AppVersion>(response);
                            isProd = data.ForProduction;

                            if (data != null)
                            {
                                switch (Device.RuntimePlatform)
                                {
                                    case Device.Android:
                                        appVersion = data.Android;
                                        display = data.Android;
                                        break;

                                    case Device.iOS:
                                        appVersion = data.iOS;
                                        display = $"{data.iOS} ({data.VersionName})";
                                        break;
                                }
                            }
                        }
                    }

                    if (currentVersion != appVersion && isProd)
                    {
                        await dialogService_.AlertAsync($"Version {display} is available on the AppStore", "New Version");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " : " + ex.Message}");
            }
        }

        public async Task GetDateFormat(string url)
        {
            try
            {
                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.Home}/setup-date-format"
                };

                var response = await genericRepository_.GetAsync<string>(builder.ToString());

                if (!string.IsNullOrWhiteSpace(response))
                {
                    PreferenceHelper.DateFormatSetup(response);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " : " + ex.Message}");
            }
        }

        public async Task RetrievePackageSetup(string clientCode)
        {
            try
            {
                var url = ApiConstants.BaseApiUrl;
                await commonDataService_.HasInternetConnection(url);

                using (UserDialogs.Instance.Loading())
                {
                    await Task.Delay(500);

                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.AuthenticationApi}/{clientCode}/get-package-setup"
                    };

                    var response = await genericRepository_.GetAsync<R.Responses.ClientPackageSetupResponse>(builder.ToString());

                    if (response != null)
                    {
                        MenuHelper.Menus(response.MobileMenus);
                        MenuHelper.SubMenus(response.MobileSubMenus);
                        MenuHelper.Forms(response.MobileForms);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }
        }

        public async Task SaveUserDeviceInfo(long userId)
        {
            try
            {
                if (!PreferenceHelper.IsFirstLogin())
                    return;

                var deviceId = PreferenceHelper.DeviceId();

                var isRegistered = await userDeviceInfoDataAccess_.IsDeviceRegistered(deviceId);

                if (isRegistered)
                    return;

                var saveDeviceResponse = await dialogService_.ConfirmDialogAsync(Messages.RegisterDeviceInfoMessage);

                if (!saveDeviceResponse)
                    return;

                var url = await commonDataService_.RetrieveClientUrl();
                var setup = await clientSetup_.RetrieveClientSetup();

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.Device}/save-device-info-per-user"
                };

                var param = new R.Requests.SubmitUserDeviceInfoRequest()
                {
                    Data = new R.Models.UserDeviceInfo()
                    {
                        UserId = userId,
                        DeviceId = deviceId,
                    }
                };

                var dataResponse = await genericRepository_.PostAsync<R.Requests.SubmitUserDeviceInfoRequest, R.Responses.BaseResponse<R.Models.UserDeviceInfo>>(builder.ToString(), param);

                if (dataResponse != null)
                {
                    //device already registered for this account
                    if (!dataResponse.IsSuccess && dataResponse.ValidationMessage?.Equals("Device exists.") == true)
                        return;

                    //register device
                    var userDeviceInfo = new UserDeviceInfoModel()
                    {
                        DeviceId = dataResponse.Model.DeviceId,
                        IsRegistered = true,
                    };

                    await userDeviceInfoDataAccess_.SaveRecord(userDeviceInfo);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }
            finally
            {
            }
        }
    }
}