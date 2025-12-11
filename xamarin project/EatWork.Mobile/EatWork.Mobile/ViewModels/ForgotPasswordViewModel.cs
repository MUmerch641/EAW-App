using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder;
using EatWork.Mobile.Utils;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class ForgotPasswordViewModel : BaseViewModel
    {
        #region commands

        public ICommand SubmitCommand
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

        public ForgotPasswordViewModel(IAuthenticationDataService authenticationDataService)
        {
            authenticationDataService_ = authenticationDataService;
        }

        public void Init(INavigation navigation)
        {
            NavigationBack = navigation;

            FormHelper = new LoginHolder();
            SubmitCommand = new Command(async () => await SubmitRequest());

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

        private async Task SubmitRequest()
        {
            try
            {
                FormHelper = await authenticationDataService_.ForgotPassword(FormHelper);

                if (FormHelper.IsSuccess)
                {
                    Success(false, Messages.ForgotPasswordResponse, "Password Reset Email Sent", 3, "MailSent.svg");
                }
            }
            /*
            catch (HttpRequestExceptionEx ex)
            {
                var list = new ObservableCollection<string>(ex.Model.Errors.Values.Select(p => p[0]));
                Error(results: list, title: ex.Model.Title.ToUpper(), autoHide: false);
            }
            */
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }
    }
}