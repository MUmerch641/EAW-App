using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Utils.DataAccess;
using EatWork.Mobile.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region commands

        public ICommand LoginCommand { get; set; }
        public ICommand ForgotCredentialCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        public ICommand TimeEntryLogCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }

        #endregion commands

        #region properties

        private LoginHolder formHelper_;

        public LoginHolder FormHelper
        {
            get { return formHelper_; }
            set { formHelper_ = value; RaisePropertyChanged(() => FormHelper); }
        }

        private bool showClockWork_;

        public bool ShowClockWork
        {
            get { return showClockWork_; }
            set { showClockWork_ = value; RaisePropertyChanged(() => ShowClockWork); }
        }

        #endregion properties

        private readonly IAuthenticationDataService authenticationDataService_;
        private readonly IMainPageDataService mainPageService_;
        private readonly ClientSetupDataAccess clientSetup_;

        public LoginViewModel(IAuthenticationDataService authenticationDataService)
        {
            authenticationDataService_ = authenticationDataService;
            clientSetup_ = AppContainer.Resolve<ClientSetupDataAccess>();
            mainPageService_ = AppContainer.Resolve<IMainPageDataService>();
        }

        public void Init(INavigation navigation)
        {
            IsBusy = false;
            ShowClockWork = false;

            NavigationBack = navigation;
            FormHelper = new LoginHolder();

            LoginCommand = new Command(async () => await MainPage());
            ForgotCredentialCommand = new Command(async () => await NavigationPage(new ForgetPasswordPage()));
            RegisterCommand = new Command(async () => await NavigationPage(new SignupPage()));
            TimeEntryLogCommand = new Command(async () => await NavigationService.PushModalAsync(new OnlineTimeEntryPage()));
            PageAppearingCommand = new Command(async () => await ExecutePageAppearingCommand());

            PreferenceHelper.IsFirstLogin(true);
            RetrieveLoginCredential();
        }

        private async Task InitSetup()
        {
            try
            {
                var setup = await clientSetup_.RetrieveClientSetup();

                if (setup != null)
                {
                    if (!string.IsNullOrWhiteSpace(setup.LoginScreenImage))
                    {
                        var type = (string.IsNullOrWhiteSpace(setup.LoginScreenImageType) ? "jpeg" : setup.LoginScreenImageType);
                        var url = new UriBuilder(ApiConstants.BaseApiUrl)
                        {
                            Path = string.Format(ApiConstants.GetImageSetup, type, setup.LoginScreenImage)
                        };

                        FormHelper.HasImageSetup = true;
                        FormHelper.SourceImage = url.ToString();
                        PreferenceHelper.LoginImageSetup(url.ToString());
                    }

                    if (!string.IsNullOrWhiteSpace(setup.LogoImage))
                    {
                        var type2 = (string.IsNullOrWhiteSpace(setup.LogoImageType) ? "jpeg" : setup.LogoImageType);
                        var url2 = new UriBuilder(ApiConstants.BaseApiUrl)
                        {
                            Path = string.Format(ApiConstants.GetImageSetup, type2, setup.LogoImage)
                        };

                        FormHelper.HasLogo = true;
                        FormHelper.LogoSourceImage = url2.ToString();
                    }

                    if (!string.IsNullOrWhiteSpace(setup.BrandingImage))
                    {
                        var type3 = (string.IsNullOrWhiteSpace(setup.BrandingImageType) ? "jpeg" : setup.BrandingImageType);
                        var url3 = new UriBuilder(ApiConstants.BaseApiUrl)
                        {
                            Path = string.Format(ApiConstants.GetImageSetup, type3, setup.BrandingImage)
                        };

                        FormHelper.HasBranding = true;
                        FormHelper.BrandingSourceImage = url3.ToString();
                        /*PreferenceHelper.SplashScreenSetup(url3.ToString());*/
                    }

                    // get menus for mobile forms
                    await mainPageService_.RetrievePackageSetup(setup.ClientCode);

                    var formItem = MenuHelper.Forms().FirstOrDefault(x => x.FormCode == MenuItemType.Clockwork.ToString());
                    ShowClockWork = formItem != null;
                }

                await mainPageService_.CheckLatestVersion();
            }
            catch (Exception ex)
            {
                /*Error(false, ex.Message);*/
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
            }
        }

        private async void RetrieveLoginCredential()
        {
            try
            {
                IsBusy = true;
                await Task.Delay(500);

                if (!await authenticationDataService_.HasClientSetup())
                {
                    Application.Current.MainPage = new NavigationPage(new WalkthroughPage());
                    //await NavigationService.PopToRootAsync();
                }
                else
                {
                    AppLayout.Extensions.ApplyColorSet();
                    await InitSetup();
                    FormHelper = await authenticationDataService_.RetreiveLoginCredential();
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task MainPage()
        {
            try
            {
                FormHelper = await authenticationDataService_.Authenticate(FormHelper);

                if (FormHelper.IsSuccess)
                {
                    using (Dialogs.Loading())
                    {
                        await Task.Delay(500);
                    }

                    /*

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Application.Current.MainPage = new NavigationPage(new MainPage());
                    });
                    */

                    Application.Current.MainPage = new MainFlyoutPage();
                    MenuHelper.ClockWork(ShowClockWork);
                }
            }
            /*
            catch (HttpRequestExceptionEx ex)
            {
                var list = new ObservableCollection<string>(ex.Model.Errors.Values.Select(p => p[0]));
                Error(results: list, title: Messages.ValidationHeaderMessage, autoHide: false);
            }
            */
            catch (Exception ex)
            {
                /*
                var converted = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(ex.Message);
                var message = converted.Values.Select(p => p[0]).FirstOrDefault();
                Error(false, message);
                */

                var result = default(Dictionary<string, List<string>>);
                bool isjson = ex.Message.TryParseJson<Dictionary<string, List<string>>>(out result);

                if (isjson)
                {
                    var converted = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(ex.Message);
                    var message = converted.Values.Select(p => p[0]).FirstOrDefault();
                    Error(false, message);
                }
                else
                {
                    Error(false, ex.Message);
                }
            }
        }

        private async Task NavigationPage(Page Page)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await Task.Delay(1000);
                await NavigationService.PushPageAsync(Page);
                IsBusy = false;
            }
        }

        private async Task ExecutePageAppearingCommand()
        {
            try
            {
                /*await mainPageService_.CheckLatestVersion();*/
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " : " + ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}