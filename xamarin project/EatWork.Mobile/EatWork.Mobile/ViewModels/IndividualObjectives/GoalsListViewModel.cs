using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.Utils;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.IndividualObjectives
{
    public class GoalsListViewModel : ListViewModel
    {
        public ICommand CloseModalCommand { get; set; }
        public ICommand ItemTappedCommand { get; set; }

        private GoalsHolder holder_;

        public GoalsHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly IGoalDataService service_;

        public GoalsListViewModel()
        {
            service_ = AppContainer.Resolve<IGoalDataService>();
        }

        public void Init(INavigation navigation, ObjectiveDetailHolder holder)
        {
            NavigationBack = navigation;
            Holder = new GoalsHolder();

            CloseModalCommand = new Command(async () => await NavigationService.PopModalAsync());
            ItemTappedCommand = new Command<GoalDetailDto>(ExecuteItemTappedCommand);

            InitForm(holder);
        }

        private async void InitForm(ObjectiveDetailHolder holder)
        {
            try
            {
                Holder = await service_.InitForm(holder);
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        private async void ExecuteItemTappedCommand(GoalDetailDto item)
        {
            if (item != null)
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    IndividualObjectiveHelper.SelectedGoalDetailDtoChanged(true);
                    IndividualObjectiveHelper.SelectedGoalDetailDto(item);
                    await NavigationService.PopModalAsync();
                }
            }
        }
    }
}