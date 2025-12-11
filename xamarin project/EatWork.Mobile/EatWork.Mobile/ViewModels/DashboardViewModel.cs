using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Utils.DataAccess;
using EatWork.Mobile.Views;
using EatWork.Mobile.Views.Survey;
using Syncfusion.SfChart.XForms;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public ICommand TimeEntryLogCommand
        {
            protected set;
            get;
        }

        public ICommand NavigateSurveyCommand { get; set; }
        public ICommand EditSurveyCommand { get; set; }
        public ICommand ViewSurveyChartCommand { get; set; }

        private ObservableCollection<ChartDataPoint> chartdata_;

        public ObservableCollection<ChartDataPoint> ChartData
        {
            get { return chartdata_; }
            set { chartdata_ = value; RaisePropertyChanged(() => ChartData); }
        }

        private ObservableCollection<R.Models.PulseSurveyList> surveys_;

        public ObservableCollection<R.Models.PulseSurveyList> Surveys
        {
            get { return surveys_; }
            set { surveys_ = value; RaisePropertyChanged(() => Surveys); }
        }

        private DashboardFormHolder form_;

        public DashboardFormHolder Form
        {
            get { return form_; }
            set { form_ = value; RaisePropertyChanged(() => Form); }
        }

        private bool showClockWork_;

        public bool ShowClockWork
        {
            get { return showClockWork_; }
            set { showClockWork_ = value; RaisePropertyChanged(() => ShowClockWork); }
        }

        private bool showPulseSurvey_;

        public bool ShowPulseSurvey
        {
            get { return showPulseSurvey_; }
            set { showPulseSurvey_ = value; RaisePropertyChanged(() => ShowPulseSurvey); }
        }

        private readonly IDashboardDataService dashboardDataService_;
        private readonly ISurveyDataService surveyService_;
        private readonly IMainPageDataService mainpageService_;
        private readonly ClientSetupDataAccess clientSetup_;

        public DashboardViewModel(IDashboardDataService dashboardDataService)
        {
            dashboardDataService_ = dashboardDataService;
            surveyService_ = AppContainer.Resolve<ISurveyDataService>();
            mainpageService_ = AppContainer.Resolve<IMainPageDataService>();
            clientSetup_ = AppContainer.Resolve<ClientSetupDataAccess>();
        }

        public void Init(INavigation navigation)
        {
            ShowClockWork = false;
            ShowPulseSurvey = false;
            NavigationBack = navigation;
            TimeEntryLogCommand = new Command(async () => await NavigationService.PushModalAsync(new OnlineTimeEntryPage()));
            NavigateSurveyCommand = new Command(async () => await NavigationSurveyPage());

            Surveys = new ObservableCollection<R.Models.PulseSurveyList>();
            EditSurveyCommand = new Command<R.Models.PulseSurveyList>(ExecuteEditSurveyCommand);
            ViewSurveyChartCommand = new Command<R.Models.PulseSurveyList>(ExecuteViewSurveyChartCommand);

            GetChartData();
            GetSurveys();
            InitSetup();
        }

        private async void InitSetup()
        {
            try
            {
                var setup = await clientSetup_.RetrieveClientSetup();
                if (setup != null)
                {
                    await mainpageService_.RetrievePackageSetup(setup.ClientCode);

                    var formItem = MenuHelper.Forms().FirstOrDefault(x => x.FormCode == MenuItemType.Clockwork.ToString());
                    ShowClockWork = formItem != null;

                    var pulseForm = MenuHelper.Forms().FirstOrDefault(x => x.FormCode == MenuItemType.PulseSurveyForm.ToString());
                    ShowPulseSurvey = pulseForm != null;
                }

                await mainpageService_.SaveUserDeviceInfo(PreferenceHelper.UserId());
                PreferenceHelper.IsFirstLogin(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            finally
            {
            }
        }

        private async void GetChartData()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    Form = await dashboardDataService_.GetDashboardDefault();

                    DateTime dateTime = new DateTime(2019, 5, 1);
                    ChartData = new ObservableCollection<ChartDataPoint>()
                    {
                        new ChartDataPoint(dateTime, 15),
                        new ChartDataPoint(dateTime.AddMonths(1), 20),
                        new ChartDataPoint(dateTime.AddMonths(2), 17),
                        new ChartDataPoint(dateTime.AddMonths(3), 23),
                        new ChartDataPoint(dateTime.AddMonths(4), 18),
                        new ChartDataPoint(dateTime.AddMonths(5), 25),
                        new ChartDataPoint(dateTime.AddMonths(6), 19),
                        new ChartDataPoint(dateTime.AddMonths(7), 21),
                    };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task NavigationSurveyPage()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await Task.Delay(500);
                /*await NavigationService.PushPageAsync(new SurveryDetailPage());*/
                await NavigationService.PushPageAsync(new SurveyListPage());
            }
        }

        private async void GetSurveys()
        {
            try
            {
                Surveys = await surveyService_.RetrieveSurveys();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                Error(false, $"{ex.GetType().Name} : {ex.Message}");
            }
        }

        private async void ExecuteEditSurveyCommand(R.Models.PulseSurveyList item)
        {
            if (item != null)
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    await NavigationService.PushPageAsync(new SurveryDetailPage(item));
                }
            }
        }

        private async void ExecuteViewSurveyChartCommand(R.Models.PulseSurveyList item)
        {
            if (!IsBusy)
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    await NavigationService.PushModalAsync(new SurveyChartPage(item));
                }
            }
        }
    }
}