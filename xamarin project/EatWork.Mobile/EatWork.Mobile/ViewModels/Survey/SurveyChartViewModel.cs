using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder.Questionnaire;
using EAW.API.DataContracts.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.Survey
{
    public class SurveyChartViewModel : BaseViewModel
    {
        public ICommand CloseModalCommand { get; set; }

        private SurveyChartHolder holder_;

        public SurveyChartHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly ISurveyDataService service_;

        public SurveyChartViewModel()
        {
            service_ = AppContainer.Resolve<ISurveyDataService>();
        }

        public void Init(INavigation nav, PulseSurveyList item)
        {
            NavigationBack = nav;
            Holder = new SurveyChartHolder();

            CloseModalCommand = new Command(async () => await NavigationService.PopModalAsync());

            InitForm(item);
        }

        private async void InitForm(PulseSurveyList item)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    ShowPage = false;

                    /*

                    if (item.PublishResult)
                    {
                        Holder = await service_.RetrieveChartSurvey(item.FormHeaderId);
                        ShowPage = true;
                    }
                    else
                        ShowPage = false;
                    */
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                    Error(false, ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}