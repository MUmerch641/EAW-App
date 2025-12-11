using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class ConnectionViewModel : BaseViewModel
    {
        #region commands

        public ICommand SetupCommand
        {
            protected set;
            get;
        }

        public ICommand ScanCommand
        {
            protected set;
            get;
        }

        public ICommand NavigateCommand
        {
            protected set;
            get;
        }

        #endregion commands

        #region properties

        private ConnectionHolder formHolder_;

        public ConnectionHolder FormHolder
        {
            get { return formHolder_; }
            set { formHolder_ = value; RaisePropertyChanged(() => FormHolder); }
        }

        private readonly IAuthenticationDataService authenticationDataService_;

        #endregion properties

        public ConnectionViewModel(IAuthenticationDataService authenticationDataService)
        {
            authenticationDataService_ = authenticationDataService;
        }

        public void Init(INavigation navigation)
        {
            NavigationBack = navigation;
            FormHolder = new ConnectionHolder();

            NavigateCommand = new Command(async () => await Navigate(new LoginPage()));
            SetupCommand = new Command(async () => await SubmitSetupRequest());

            InitForm();
        }

        private async void InitForm()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(1000);

                    if (await authenticationDataService_.HasClientSetup())
                    {
                        Application.Current.MainPage = new NavigationPage(new LoginPage());
                        await NavigationService.PopToRootAsync();
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
        }

        private async Task SubmitSetupRequest()
        {
            try
            {
                FormHolder = await authenticationDataService_.RetrieveClientSetup(FormHolder);

                if (FormHolder.Success)
                {
                    using (Dialogs.Loading())
                    {
                        await Task.Delay(1000);

                        //Application.Current.MainPage = new CustomIconNavigationPage(new LoginPage());
                        //await Navigation.PopToRootAsync(true);
                        await Navigate(new LoginPage(true));
                    }
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        private async Task Navigate(Page page)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await Task.Delay(1000);
                await NavigationService.PushPageAsync(page);
                IsBusy = false;
            }
        }
    }
}