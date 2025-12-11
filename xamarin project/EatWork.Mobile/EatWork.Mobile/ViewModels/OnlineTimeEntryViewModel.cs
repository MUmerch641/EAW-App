using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class OnlineTimeEntryViewModel : BaseViewModel
    {
        #region commands

        public ICommand CloseCommand
        {
            protected set;
            get;
        }

        public ICommand TransactCommand
        {
            protected set;
            get;
        }

        #endregion commands

        #region properties

        private OnlineTimeEntryHolder formHelper_;

        public OnlineTimeEntryHolder FormHelper
        {
            get { return formHelper_; }
            set { formHelper_ = value; RaisePropertyChanged(() => FormHelper); }
        }

        #endregion properties

        private readonly IOnlineTimeEntryDataService onlineTimeEntryDataService_;

        public OnlineTimeEntryViewModel(IOnlineTimeEntryDataService onlineTimeEntryDataService)
        {
            onlineTimeEntryDataService_ = onlineTimeEntryDataService;
        }

        public void Init(INavigation navigation)
        {
            NavigationBack = navigation;
            IsBusy = false;
            FormHelper = new OnlineTimeEntryHolder();
            TransactCommand = new Command<string>(Transact);
            CloseCommand = new Command(async () => await NavigationService.PopModalAsync());

            InitForm();
        }

        private async void InitForm()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    PauseTimer();

                    FormHelper = await onlineTimeEntryDataService_.InitForm();
                }
                catch (Exception ex)
                {
                    FormHelper.ShowForm = false;
                    Error(false, ex.Message);
                }
                finally
                {
                    IsBusy = false;
                    StartTime();
                }
            }
        }

        private async void Transact(string type)
        {
            try
            {
                //IsBusy = true;
                await Task.Delay(500);
                FormHelper.EnableButtons = false;
                FormHelper.TimeEntryLogModel.Type = type;

                FormHelper = await onlineTimeEntryDataService_.Transact(FormHelper);

                if (FormHelper.IsSuccess)
                {
                    Success(autoHide: false, content: FormHelper.ResponseMesage);
                }
                else if (FormHelper.UserErrorList.Count > 0)
                {
                    Error(autoHide: false, results: FormHelper.UserErrorList);
                }
            }
            catch (Xamarin.Essentials.FeatureNotSupportedException ex)
            {
                Error(false, ex.Message);
            }
            catch (Xamarin.Essentials.PermissionException ex)
            {
                Error(false, ex.Message);
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
            finally
            {
                FormHelper.EnableButtons = true;
                //IsBusy = false;
            }
        }

        private void StartTime()
        {
            FormHelper.Timer = new System.Timers.Timer();
            FormHelper.Timer.Elapsed += Timer_Elapsed;
            FormHelper.Timer.AutoReset = true;
            FormHelper.Timer.Enabled = true;
        }

        private void PauseTimer()
        {
            if (FormHelper.Timer != null)
            {
                FormHelper.Timer.Stop();
                FormHelper.Timer.Dispose();
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            FormHelper.TimeClock = FormHelper.TimeClock.AddMilliseconds(FormHelper.Timer.Interval);
        }
    }
}