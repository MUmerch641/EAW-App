using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.FormHolder;
using EatWork.Mobile.Utils;
using Plugin.Geolocator;
using Plugin.Permissions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class OnlineTimeEntryDataService : IOnlineTimeEntryDataService
    {
        private readonly ICommonDataService commonDataService_;
        private readonly IDialogService dialogService_;
        private readonly IGenericRepository genericRepository_;
        private CancellationTokenSource cts;
        private readonly StringHelper string_;

        public OnlineTimeEntryDataService(IDialogService dialogService,
            ICommonDataService commonDataService,
            IGenericRepository genericRepository,
            StringHelper stringHelper)
        {
            dialogService_ = dialogService;
            commonDataService_ = commonDataService;
            genericRepository_ = genericRepository;
            string_ = stringHelper;
        }

        public async Task<OnlineTimeEntryHolder> InitForm()
        {
            var retValue = new OnlineTimeEntryHolder();
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);
            var userInfo = PreferenceHelper.UserInfo();

            try
            {
                var builder = new UriBuilder(url)
                {
                    Path = ApiConstants.GetOnlineTimeEntryFormHelper
                };

                var param = new R.Requests.GetOnlineTimeEntryInitForm();

                var request = string_.CreateUrl<R.Requests.GetOnlineTimeEntryInitForm>(builder.ToString(), param);

                var response = await genericRepository_.GetAsync<R.Responses.OnlineTimeEntryInitFormResponse>(request.ToString());

                if (response.IsSuccess)
                {
                    retValue.HasSetup = response.HasSetup;

                    if (!response.HasSetup)
                    {
                        retValue.UserError = response.UserError;
                    }
                    else
                    {
                        retValue.TimeClock = response.ServerTime;
                        retValue.ClockworkConfiguration = new ClockworkConfigurationModel();
                        PropertyCopier<R.Models.ClockworkConfigurationModel, ClockworkConfigurationModel>.Copy(response.ClockworkConfigurationModel, retValue.ClockworkConfiguration);
                        retValue.TimeInColor = retValue.ClockworkConfiguration.TimeInColor;
                        retValue.TimeOutColor = retValue.ClockworkConfiguration.TimeOutColor;
                        retValue.BreakInColor = retValue.ClockworkConfiguration.BreakInColor;
                        retValue.BreakOutColor = retValue.ClockworkConfiguration.BreakOutColor;
                        retValue.AllowImageCapture = response.ClockworkConfigurationModel.AllowImageCapture;
                        retValue.AllowLocationCapture = response.ClockworkConfigurationModel.AllowLocationCapture;
                    }
                }

                if (retValue.HasSetup)
                {
                    var location = await GetLocation(true);
                    retValue.IpAddress = DependencyService.Get<IIPAddressManager>().GetIPAddress();

                    retValue.TimeEntryLogModel = new Models.TimeEntryLogModel()
                    {
                        Source = Constants.SourceOnlineTimeEntry,
                        StatusId = 0,
                        Latitude = location?.Latitude.ToString(),
                        Longitude = location?.Longitude.ToString(),
                        IPAddress = retValue.IpAddress,
                        ProfileId = (FormSession.IsLoggedIn ? userInfo.ProfileId : 0),
                    };

                    retValue.EmployeeNumber = (FormSession.IsLoggedIn ? userInfo.EmployeeNo : string.Empty);
                    retValue.AccessCode = (FormSession.IsLoggedIn ? userInfo.AccessId : string.Empty);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retValue;
        }

        public async Task<OnlineTimeEntryHolder> Transact(OnlineTimeEntryHolder form)
        {
            var retValue = form;

            try
            {
                var errror = new List<int>();
                form.ErrorAccessCode = false;
                form.ErrorEmployeeNumber = false;
                form.ResponseMesage = string.Empty;
                form.UserErrorList = new System.Collections.ObjectModel.ObservableCollection<string>();
                form.IsSuccess = false;

                //validate
                if (string.IsNullOrEmpty(form.EmployeeNumber))
                {
                    errror.Add(1);
                    form.ErrorEmployeeNumber = true;
                }

                if (string.IsNullOrEmpty(form.AccessCode))
                {
                    errror.Add(1);
                    form.ErrorAccessCode = true;
                }

                if (errror.Count == 0)
                {
                    var imageString = string.Empty;
                    var imageFile = await commonDataService_.TakePhotoAsync("OTE", false);
                    form.TimeEntryLogModel.TimeEntry = form.TimeClock;

                    if (imageFile != null)
                        imageString = imageFile.Base64String;

                    await dialogService_.ShowLoading();

                    var url = await commonDataService_.RetrieveClientUrl();
                    await commonDataService_.HasInternetConnection(url);

                    var builder = new UriBuilder(url)
                    {
                        Path = ApiConstants.SubmitOnlineTimeEntryRequest
                    };

                    var data = new R.Models.TimeEntryLog();
                    PropertyCopier<TimeEntryLogModel, R.Models.TimeEntryLog>.Copy(form.TimeEntryLogModel, data);

                    var param = new R.Requests.SubmitOnlineClockwork()
                    {
                        Data = data,
                        AccessId = form.AccessCode,
                        EmployeeNo = form.EmployeeNumber,
                        UserImage = imageString,
                        IsLoggedIn = FormSession.IsLoggedIn,
                    };

                    var response = await genericRepository_.PostAsync<R.Requests.SubmitOnlineClockwork, R.Responses.OnlineTimeEntryResponse>(builder.ToString(), param);

                    if (response.IsSuccess)
                    {
                        retValue.ResponseMesage = response.UserSuccess;

                        if (!FormSession.IsLoggedIn)
                        {
                            if (retValue.ClockworkConfiguration.ClearEmployeeNo)
                                retValue.EmployeeNumber = string.Empty;

                            retValue.AccessCode = string.Empty;
                        }

                        retValue.TimeEntryLogModel.Remark = string.Empty;
                    }
                    else
                        retValue.UserErrorList = new System.Collections.ObjectModel.ObservableCollection<string>(response.UserErrorList);

                    /*form.ShowMessage = true;*/
                    form.LeaveWarningOnly = response.LeaveWarningOnly;
                    form.IsSuccess = response.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dialogService_.HideLoading();
            }

            return retValue;
        }

        public async Task<Location> GetLocation(bool required, int gpsTimeOut = 15)
        {
            var retValue = new Location();

            try
            {
                var locator = CrossGeolocator.Current;
                var isGpsAvail = (locator.IsGeolocationAvailable || locator.IsGeolocationEnabled);

                var locationStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();

                if (locationStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    locationStatus = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
                }

                if (locationStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted && required)
                {
                    throw new Exception(Messages.LOCATIONDENIED);
                }
                else if (!locator.IsGeolocationEnabled && required)
                {
                    throw new Exception(Messages.GPSERROR);
                }

                if (locator.IsGeolocationAvailable)
                {
                    if (locationStatus == Plugin.Permissions.Abstractions.PermissionStatus.Granted && locator.IsGeolocationEnabled)
                    {
                        var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(gpsTimeOut));
                        cts = new CancellationTokenSource();

                        var location = await Geolocation.GetLocationAsync(request, cts.Token) ?? throw new Exception(Messages.GPSERROR);

                        if (location.IsFromMockProvider)
                            throw new Exception($"MOCK : {Messages.GPSERROR}");

                        retValue = location;
                    }
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                throw fnsEx;
            }
            /*
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                throw fneEx;
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                throw pEx;
            }
            */
            catch (Exception ex)
            {
                if (required)
                {
                    throw ex;
                }
            }

            return retValue;
        }
    }
}