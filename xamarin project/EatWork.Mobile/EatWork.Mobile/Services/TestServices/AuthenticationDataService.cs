using Acr.UserDialogs;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Excemptions;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataAccess;
using EatWork.Mobile.Models.FormHolder;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Utils.DataAccess;
using EAW.API.DataContracts.Requests;
using EAW.API.DataContracts.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services.TestServices
{
    public class AuthenticationDataService : IAuthenticationDataService
    {
        private readonly LoginDataAccess loginDataAccess_;
        private readonly ClientSetupDataAccess clientSetupDataAccess_;
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IDialogService dialogService_;
        private readonly IMainPageDataService mainPageDataService_;

        public AuthenticationDataService(LoginDataAccess loginDataAccess,
            IGenericRepository genericRepository,
            ClientSetupDataAccess clientSetupDataAccess,
            ICommonDataService commonDataService,
            IDialogService dialogService,
            IMainPageDataService mainPageDataService)
        {
            loginDataAccess_ = loginDataAccess;
            genericRepository_ = genericRepository;
            clientSetupDataAccess_ = clientSetupDataAccess;
            commonDataService_ = commonDataService;
            dialogService_ = dialogService;
            mainPageDataService_ = mainPageDataService;
        }

        public async Task<LoginHolder> Authenticate(LoginHolder form)
        {
            try
            {
                if (form.IsValidLogin())
                {
                    form.CopyModel();

                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);

                        var url = await commonDataService_.RetrieveClientUrl();
                        await commonDataService_.HasInternetConnection(url);

                        var builder = new UriBuilder(url)
                        {
                            Path = ApiConstants.Authenticate
                        };

                        var param = new R.Requests.AuthenticationRequest
                        {
                            Username = form.UserModel.Username,
                            Password = form.UserModel.Password
                        };

                        var response = await genericRepository_.PostAsync<R.Requests.AuthenticationRequest, R.Responses.AuthenticationResponse>(builder.ToString(), param);

                        if (response.IsSuccess)
                        {
                            var ACCESS_BUILDER = new UriBuilder(url)
                            {
                                Path = string.Format(ApiConstants.GetAccessMobile, response.User.UserSecurityId)
                            };

                            var ACCESS_RESPONSE = await genericRepository_.GetAsync<bool>(ACCESS_BUILDER.ToString());

                            if (!ACCESS_RESPONSE)
                            {
                                var message = new Dictionary<string, List<string>>
                                {
                                    { "", new List<string>() { Messages.MobileAccessDenied } }
                                };

                                throw new Exception(JsonConvert.SerializeObject(message));
                            }

                            await loginDataAccess_.DeleteAllData();
                            if (form.RememberCredential)
                            {
                                var model = new LoginDataModel()
                                {
                                    Password = form.UserModel.Password.Encrypt(),
                                    UserName = form.UserModel.Username
                                };

                                await loginDataAccess_.SaveRecord(model);
                            }

                            FormSession.ClearEverything();
                            FormSession.IsLoggedIn = true;
                            FormSession.TokenBearer = response.Token;

                            #region employee info

                            var EMPLOYEE_BUILDER = new UriBuilder(url)
                            {
                                Path = string.Format(ApiConstants.GetEmployeeBySecurityId, response.User.UserSecurityId)
                            };

                            var EMPLOYEE_RESPONSE = await genericRepository_.GetAsync<R.Models.EmployeeInformation>(EMPLOYEE_BUILDER.ToString(), response.Token);

                            var employeeInfo = new EmployeeInformationModel()
                            {
                                ProfileId = 0,
                                Department = string.Empty,
                                EmployeeName = form.UserModel.Username,
                                EmployeeNo = string.Empty,
                                Position = string.Empty,
                                FirstName = form.UserModel.FirstName,
                                LastName = form.UserModel.LastName,
                            };

                            if (EMPLOYEE_RESPONSE != null)
                            {
                                employeeInfo = new EmployeeInformationModel()
                                {
                                    ProfileId = EMPLOYEE_RESPONSE.ProfileId,
                                    Department = EMPLOYEE_RESPONSE.Department,
                                    EmployeeName = EMPLOYEE_RESPONSE.EmployeeName,
                                    EmployeeNo = EMPLOYEE_RESPONSE.EmployeeNo,
                                    Position = EMPLOYEE_RESPONSE.Position,
                                    CompanyId = EMPLOYEE_RESPONSE.CompanyId.GetValueOrDefault(0),
                                    BranchId = EMPLOYEE_RESPONSE.BranchId.GetValueOrDefault(0),
                                    AccessId = EMPLOYEE_RESPONSE.AccessId,
                                    DepartmentId = EMPLOYEE_RESPONSE.DepartmentId.GetValueOrDefault(0),
                                    OfficeId = EMPLOYEE_RESPONSE.OfficeId.GetValueOrDefault(0),
                                    TeamId = EMPLOYEE_RESPONSE.TeamId.GetValueOrDefault(0),
                                    FirstName = EMPLOYEE_RESPONSE.FirstName,
                                    LastName = EMPLOYEE_RESPONSE.LastName,
                                    Company = EMPLOYEE_RESPONSE.Company,
                                    Branch = EMPLOYEE_RESPONSE.Branch,
                                    JobRankId = EMPLOYEE_RESPONSE.JobRankId,
                                    JobGradeId = EMPLOYEE_RESPONSE.JobGradeId,
                                    JobLevelId = EMPLOYEE_RESPONSE.JobLevelId,
                                    PositionId = EMPLOYEE_RESPONSE.PositionId,
                                    Birthdate = EMPLOYEE_RESPONSE.Birthdate,
                                };
                            }

                            FormSession.UserInfo = JsonConvert.SerializeObject(new UserModel()
                            {
                                Username = response.User.UserName,
                                UserSecurityId = Convert.ToInt64(response.User.UserSecurityId),
                                ProfileId = employeeInfo.ProfileId,
                                Department = employeeInfo.Department,
                                EmployeeName = employeeInfo.EmployeeName,
                                EmployeeNo = employeeInfo.EmployeeNo,
                                Position = employeeInfo.Position,
                                UserTypeId = Convert.ToInt64(response.User.UserTypeId),
                                CompanyId = employeeInfo.CompanyId.GetValueOrDefault(),
                                BranchId = employeeInfo.BranchId.GetValueOrDefault(),
                                AccessId = employeeInfo.AccessId,
                                DepartmentId = employeeInfo.DepartmentId.GetValueOrDefault(),
                                OfficeId = employeeInfo.OfficeId.GetValueOrDefault(),
                                TeamId = employeeInfo.TeamId.GetValueOrDefault(),
                                FirstName = employeeInfo.FirstName,
                                LastName = employeeInfo.LastName,
                                Company = employeeInfo.Company,
                                Branch = employeeInfo.Branch,
                                JobRankId = employeeInfo.JobRankId.GetValueOrDefault(),
                                JobGradeId = employeeInfo.JobGradeId.GetValueOrDefault(),
                                JobLevelId = employeeInfo.JobLevelId.GetValueOrDefault(),
                                PositionId = employeeInfo.PositionId.GetValueOrDefault(),
                                BirthDate = employeeInfo.Birthdate.GetValueOrDefault(),
                            });

                            #endregion employee info

                            //==CALL MIME TYPE SETTING
                            await mainPageDataService_.GetMimeTypeList();

                            //==SAVE DEVICE INFO
                            await mainPageDataService_.SaveDeviceInfo();

                            //get date format
                            await mainPageDataService_.GetDateFormat(url);

                            PreferenceHelper.UserId(response.User.UserSecurityId);
                        }
                        else
                        {
                            var message = new Dictionary<string, List<string>>
                            {
                                { "", new List<string>() { response.ErrorMessage } }
                            };

                            throw new Exception(JsonConvert.SerializeObject(message));
                        }

                        form.IsSuccess = response.IsSuccess;
                    }
                }
            }
            catch (HttpRequestExceptionEx ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }

            return form;
        }

        public async Task<LoginHolder> ForgotPassword(LoginHolder form)
        {
            form.IsSuccess = false;

            try
            {
                if (form.IsValidForgotPassword())
                {
                    form.CopyModel();

                    var url = ApiConstants.BaseApiUrl;

                    if (await HasClientSetup())
                        url = await commonDataService_.RetrieveClientUrl();

                    await commonDataService_.HasInternetConnection(url);

                    if (await dialogService_.ConfirmDialogAsync(Messages.Submit))
                    {
                        using (UserDialogs.Instance.Loading())
                        {
                            await Task.Delay(500);

                            var builder = new UriBuilder(url)
                            {
                                Path = $"{ApiConstants.AuthenticationApi}/forgot-password"
                            };

                            var param = new R.Requests.ForgotPasswordRequest()
                            {
                                EmailAddress = form.EmailAddress.Value,
                                Username = form.Username.Value
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.ForgotPasswordRequest, R.Responses.BaseResponse>(builder.ToString(), param);

                            if (!response.IsSuccess)
                            {
                                if (!string.IsNullOrWhiteSpace(response.ValidationMessage))
                                    throw new Exception(response.ValidationMessage);
                            }

                            form.IsSuccess = response.IsSuccess;
                        }
                    }
                }
            }
            catch (HttpRequestExceptionEx ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }

            return form;
        }

        public async Task<LoginHolder> Registration(LoginHolder form)
        {
            try
            {
                if (form.IsValidRegistation())
                {
                    form.CopyModel();

                    var url = ApiConstants.BaseApiUrl;

                    if (await HasClientSetup())
                        url = await commonDataService_.RetrieveClientUrl();

                    await commonDataService_.HasInternetConnection(url);

                    if (await dialogService_.ConfirmDialogAsync(Messages.Submit))
                    {
                        using (UserDialogs.Instance.Loading())
                        {
                            await Task.Delay(500);

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.EmployeeRegistration
                            };

                            var param = new R.Requests.EmployeeRegistrationRequest()
                            {
                                Username = form.UserModel.Username,
                                EmailAddress = form.UserModel.EmailAddress,
                                Password = form.UserModel.Password,
                                ConfirmPassword = form.UserModel.ConfirmPassword,
                                EmployeeNo = form.UserModel.EmployeeNo
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.EmployeeRegistrationRequest, R.Responses.EmployeeRegistationResponse>(builder.ToString(), param);

                            if (!response.IsSuccess)
                                throw new Exception(Messages.ErrorRegistration);
                        }
                    }

                    form.IsSuccess = true;
                }
            }
            catch (HttpRequestExceptionEx ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }

            return form;
        }

        public async Task<LoginHolder> RetreiveLoginCredential()
        {
            var retValue = new LoginHolder();

            var response = await loginDataAccess_.RetrieveLoginCredential();

            retValue.TogglePassVisibility = true;

            if (response != null)
            {
                retValue.Username.Value = response.UserName;
                retValue.Password.Value = response.Password.Decrypt();
                /*
                retValue.UserModel = new Models.UserModel()
                {
                    Username = response.UserName,
                    Password = response.Password.Decrypt()
                };

                */

                retValue.RememberCredential = true;
                retValue.TogglePassVisibility = false;
            }

            return retValue;
        }

        public async Task<ConnectionHolder> RetrieveClientSetup(ConnectionHolder form)
        {
            var retValue = form;
            retValue.ErrorClientCode = false;
            retValue.ErrorPassKey = false;
            try
            {
                await commonDataService_.HasInternetConnection();

                if (string.IsNullOrWhiteSpace(retValue.ClientSetupModel.ClientCode))
                {
                    retValue.ErrorClientCode = true;
                    retValue.ErrorClientCodeMessage = "Please enter your Client Code";
                }

                if (string.IsNullOrWhiteSpace(retValue.ClientSetupModel.Passkey))
                    retValue.ErrorPassKey = true;

                if (!retValue.ErrorClientCode && !retValue.ErrorPassKey)
                {
                    using (UserDialogs.Instance.Loading("Retrieving client setup..."))
                    {
                        await Task.Delay(100);

                        var builder = new UriBuilder(ApiConstants.BaseApiUrl)
                        {
                            Path = ApiConstants.SetupClientApi
                        };

                        var param = new ClientSetupRequest
                        {
                            ClientCode = retValue.ClientSetupModel.ClientCode,
                            PassKey = retValue.ClientSetupModel.Passkey
                        };

                        var response = await genericRepository_.PostAsync<ClientSetupRequest, ClientSetupResponse>(builder.ToString(), param);

                        if (response.IsSuccess)
                        {
                            retValue.ClientSetupModel.APILink = response.ClientSetup.ApiUrl.Encrypt();
                            retValue.ClientSetupModel.ClientCode = response.ClientSetup.ClientCode;
                            retValue.ClientSetupModel.ClientId = response.ClientSetup.ClientSetupId;
                            retValue.ClientSetupModel.Passkey = response.ClientSetup.PassKey.Encrypt();
                            retValue.ClientSetupModel.LoginScreenImage = response.ClientSetup.LoginScreenImage;
                            retValue.ClientSetupModel.HomeScreenImage = response.ClientSetup.HomeScreenImage;
                            retValue.ClientSetupModel.LoginScreenImageType = response.ClientSetup.LoginScreenImageType;
                            retValue.ClientSetupModel.HomeScreenImageType = response.ClientSetup.HomeScreenImageType;
                            retValue.ClientSetupModel.LogoImage = response.ClientSetup.LogoImage;
                            retValue.ClientSetupModel.LogoImageType = response.ClientSetup.LogoImageType;
                            retValue.ClientSetupModel.ThemeConfigId = response.ClientSetup.ThemeConfigId;
                            retValue.ClientSetupModel.BrandingImage = response.ClientSetup.BrandingImage;
                            retValue.ClientSetupModel.BrandingImageType = response.ClientSetup.BrandingImageType;
                            retValue.ClientSetupModel.HomePageImage = response.ClientSetup.HomePageImage;
                            retValue.ClientSetupModel.HomePageImageType = response.ClientSetup.HomePageImageType;

                            await clientSetupDataAccess_.DeleteAllData();
                            await clientSetupDataAccess_.SaveRecord(retValue.ClientSetupModel);

                            await mainPageDataService_.GetThemeSetup(response.ClientSetup.ThemeConfigId.GetValueOrDefault(0));

                            retValue.Success = response.IsSuccess;
                        }
                        else
                            throw new ArgumentException(response.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

            return retValue;
        }

        public async Task<bool> HasClientSetup()
        {
            var retVal = false;

            try
            {
                var setup = await clientSetupDataAccess_.RetrieveClientSetup();

                if (setup != null)
                    retVal = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.GetType().Name + " : " + e.Message}");
                throw;
            }

            return retVal;
        }
    }
}