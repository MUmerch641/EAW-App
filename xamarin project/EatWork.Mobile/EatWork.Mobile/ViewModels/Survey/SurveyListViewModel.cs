using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder.Questionnaire;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.Survey
{
    public class SurveyListViewModel : ListViewModel
    {
        private SurveyListHolder holder_;

        public SurveyListHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly ISurveyDataService service_;

        public SurveyListViewModel()
        {
            service_ = AppContainer.Resolve<ISurveyDataService>();
        }

        public void Init(INavigation nav)
        {
            NavigationBack = nav;
            Holder = new SurveyListHolder();

            InitList();
        }

        private async void InitList()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    Holder.ItemSource = await service_.RetrieveSurveys();

                    ShowList = (Holder.ItemSource.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
                    NoItems = (Holder.ItemSource.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));
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
    }
}