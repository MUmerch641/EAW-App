using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Excemptions;
using EatWork.Mobile.Models.FormHolder;
using EatWork.Mobile.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class SignupViewModel : BaseViewModel
    {
        #region commands

        public ICommand RegisterCommand
        {
            protected set;
            get;
        }

        #endregion commands

        #region properties

        private LoginHolder formHelper_;

        public LoginHolder FormHelper
        {
            get { return formHelper_; }
            set { formHelper_ = value; RaisePropertyChanged(() => FormHelper); }
        }

        #endregion properties

        private readonly IAuthenticationDataService authenticationDataService_;

        public SignupViewModel(IAuthenticationDataService authenticationDataService)
        {
            authenticationDataService_ = authenticationDataService;
        }

        public void Init(INavigation navigation)
        {
            NavigationBack = navigation;
            FormHelper = new LoginHolder();
            RegisterCommand = new Command(async () => await Register());

            InitForm();
        }

        private void InitForm()
        {
            if (!string.IsNullOrWhiteSpace(PreferenceHelper.LoginImageSetup()))
            {
                FormHelper.HasImageSetup = true;
                FormHelper.SourceImage = PreferenceHelper.LoginImageSetup();
            }
        }

        private async Task Register()
        {
            try
            {
                FormHelper = await authenticationDataService_.Registration(FormHelper);

                if (FormHelper.IsSuccess)
                {
                    Success(autoHide: false, content: Messages.RegistrationSuccess, image: "MailSent.svg");
                }
            }
            catch (HttpRequestExceptionEx ex)
            {
                var list = new ObservableCollection<string>(ex.Model.Errors.Values.Select(p => p[0]));
                Error(results: list, title: ex.Model.Title.ToUpper(), autoHide: false);
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }
       
    }
}