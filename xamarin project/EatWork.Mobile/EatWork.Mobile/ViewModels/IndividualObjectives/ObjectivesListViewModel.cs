using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.IndividualObjectives
{
    public class ObjectivesListViewModel : BaseViewModel
    {
        public ICommand CloseModalCommand { get; set; }

        private ObservableCollection<MainObjectiveDto> objectives_;

        public ObservableCollection<MainObjectiveDto> Objectives
        {
            get { return objectives_; }
            set { objectives_ = value; RaisePropertyChanged(() => Objectives); }
        }

        public ObjectivesListViewModel()
        {
        }

        public void Init(INavigation nav, ObservableCollection<MainObjectiveDto> list)
        {
            NavigationBack = nav;
            Objectives = new ObservableCollection<MainObjectiveDto>();

            CloseModalCommand = new Command(async () => await NavigationService.PopModalAsync());

            InitList(list);
        }

        private async void InitList(ObservableCollection<MainObjectiveDto> list)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    Objectives = list;
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