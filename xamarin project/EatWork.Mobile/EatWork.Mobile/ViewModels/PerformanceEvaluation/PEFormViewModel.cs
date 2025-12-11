using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.Models.FormHolder.PerformanceEvaluation;
using EatWork.Mobile.Views.PerformanceEvaluation;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.PerformanceEvaluation
{
    public class PEFormViewModel : BaseViewModel
    {
        public ICommand MoveToQuestionnaireCommand { get; set; }
        public ICommand ViewObjectiveItemCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }
        public ICommand UnFocusCommand { get; set; }

        private PEFormHolder holder_;

        public PEFormHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private ObjectiveDetailDto selected_;

        public ObjectiveDetailDto Selected
        {
            get { return selected_; }
            set { selected_ = value; RaisePropertyChanged(() => Selected); }
        }

        private readonly IPEFormDataService service_;
        private readonly IDialogService dialogService_;
        private readonly IIndividualObjectiveItemDataService individualObjectiveItemDataService_;

        public PEFormViewModel()
        {
            service_ = AppContainer.Resolve<IPEFormDataService>();
            dialogService_ = AppContainer.Resolve<IDialogService>();
            individualObjectiveItemDataService_ = AppContainer.Resolve<IIndividualObjectiveItemDataService>();
        }

        public void Init(INavigation nav, long id)
        {
            NavigationBack = nav;

            InitHelpers();
            InitForm(id);
        }

        private void InitHelpers()
        {
            Holder = new PEFormHolder();
            Selected = new ObjectiveDetailDto();

            MoveToQuestionnaireCommand = new Command(ExecuteMoveToQuestionnaireCommand);
            ViewObjectiveItemCommand = new Command<ObjectiveDetailDto>(ExecuteViewObjectiveItemCommand);
            PageAppearingCommand = new Command(ExecutePageAppearingCommand);
            PageDisappearingCommand = new Command(ExecutePageDisappearingCommand);

            UnFocusCommand = new Command<ObjectiveDetailDto>(ExecuteUnActiveCommand);
        }

        private async void InitForm(long id)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    Holder = await service_.InitForm(id);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.GetType().Name + " : " + ex.Message}");
                    Error(false, ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async void ExecuteMoveToQuestionnaireCommand()
        {
            if (await dialogService_.ConfirmDialogAsync("Move to Questionnaire?"))
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);

                    Holder = await service_.SavePODetails(Holder);
                    await NavigationService.PushPageAsync(new QuestionnairePage(Holder));
                }
            }
        }

        private async void ExecuteViewObjectiveItemCommand(ObjectiveDetailDto item)
        {
            if (item != null)
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);

                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new InputRatingModalPage(item));
                }
            }
        }

        private void ExecutePageAppearingCommand()
        {
            /*
            MessagingCenter.Subscribe<InputRatingViewModel, ObjectiveDetailDto>(this, "SaveRatingCompleted", async (s, param) =>
            {
                if (param != null)
                {
                    //display
                    var index = Holder.Objectives.ToList().FindIndex(x => x.PODetailId == param.PODetailId);
                    var exst = Holder.Objectives.FirstOrDefault(x => x.PODetailId == param.PODetailId);
                    if (index >= 0)
                    {
                        Holder.Objectives.Remove(exst);
                        Holder.Objectives.Insert(index, param);
                    }

                    var grouped = await individualObjectiveItemDataService_.GroupObjectives(Holder.Objectives);

                    Holder.ObjectivesDisplay = new ObservableCollection<MainObjectiveDto>(grouped.Objectives);

                    //actual lists to save
                    var po = Holder.PODetails.FirstOrDefault(x => x.PerformanceObjectiveDetailId == param.PODetailId);
                    po.EmployeeReview = param.EmployeeReview;
                    po.Actual = param.Actual;
                    po.EmployeeRating = param.EmployeeRating;

                    Holder = Holder;
                }
            });
            */
        }

        private void ExecutePageDisappearingCommand()
        {
            /*
            MessagingCenter.Unsubscribe<InputRatingViewModel, ObjectiveDetailDto>(this, "SaveRatingCompleted");
            */
        }

        private async void ExecuteUnActiveCommand(ObjectiveDetailDto param)
        {
            if (param != null)
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    //display
                    
                    var index = Holder.Objectives.ToList().FindIndex(x => x.PODetailId == param.PODetailId);
                    var exst = Holder.Objectives.FirstOrDefault(x => x.PODetailId == param.PODetailId);
                    if (index >= 0)
                    {
                        Holder.Objectives.Remove(exst);
                        Holder.Objectives.Insert(index, param);
                    }
                    /*
                    var grouped = await individualObjectiveItemDataService_.GroupObjectives(Holder.Objectives);

                    Holder.ObjectivesDisplay = new ObservableCollection<MainObjectiveDto>(grouped.Objectives);
                    */

                    //actual lists to save
                    var po = Holder.PODetails.FirstOrDefault(x => x.PerformanceObjectiveDetailId == param.PODetailId);
                    po.EmployeeReview = param.EmployeeReview;
                    po.Actual = param.Actual;
                    po.EmployeeRating = param.EmployeeRating;

                    Holder = Holder;
                }
            }
        }
    }
}