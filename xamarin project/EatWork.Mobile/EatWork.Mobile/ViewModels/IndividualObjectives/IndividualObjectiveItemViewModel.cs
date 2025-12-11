using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Excemptions;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Validations;
using EatWork.Mobile.Views.IndividualObjectives;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.IndividualObjectives
{
    public class IndividualObjectiveItemViewModel : BaseViewModel
    {
        public ICommand SubmitCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand AddObjectiveCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }
        public ICommand ViewAllObjectivesCommand { get; set; }
        public ICommand ViewObjectiveItemCommand { get; set; }

        private IndividualObjectiveItemHolder formHelper_;

        public IndividualObjectiveItemHolder FormHelper
        {
            get { return formHelper_; }
            set { formHelper_ = value; RaisePropertyChanged(() => FormHelper); }
        }

        private readonly IIndividualObjectiveItemDataService service_;

        public IndividualObjectiveItemViewModel()
        {
            service_ = AppContainer.Resolve<IIndividualObjectiveItemDataService>();
        }

        public void Init(INavigation navigation, long id)
        {
            NavigationBack = navigation;
            FormHelper = new IndividualObjectiveItemHolder();
            SubmitCommand = new Command(async () => await ExecuteSubmitCommand());
            SaveCommand = new Command(async () => await ExecuteSubmitCommand(true));
            AddObjectiveCommand = new Command(ExecuteAddObjectiveCommand);
            PageAppearingCommand = new Command(ExecutePageAppearingCommand);
            PageDisappearingCommand = new Command(ExecutePageDisappearingCommand);
            ViewAllObjectivesCommand = new Command(ExecuteViewAllObjectivesCommand);
            ViewObjectiveItemCommand = new Command<ObjectiveDetailDto>(ExecuteViewObjectiveItemCommand);
            CancelCommand = new Command(async () => await ExecuteCancelCommand());

            InitForm(id);
        }

        public async void InitForm(long id = 0)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true; 
                    await Task.Delay(500);
                    FormHelper = await service_.InitForm(id);
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

        private async Task ExecuteSubmitCommand(bool isSaveOnly = false)
        {
            try
            {
                FormHelper.IsSaveOnly = isSaveOnly;

                FormHelper = await service_.SubmitRequest(FormHelper);

                if (FormHelper.Success)
                {
                    Success(true, Messages.RecordSaved);
                    await NavigationService.PopToRootAsync();
                }
            }
            catch (HttpRequestExceptionEx ex)
            {
                var list = new ObservableCollection<string>(ex?.Model?.Errors?.Values?.Select(p => p[0]));
                Error(results: list, title: ex.Model?.Title?.ToUpper(), autoHide: false);
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
            finally
            {
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private async void ExecuteAddObjectiveCommand()
        {
            if (!IsBusy)
            {
                try
                {
                    FormHelper.EffectiveYear.Validations.Clear();
                    FormHelper.EffectiveYear.Validations.Add(new IsNotNullOrEmptyRule<string>
                    {
                        ValidationMessage = ""
                    });

                    FormHelper.EffectiveYear.Validate();

                    if (FormHelper.EffectiveYear.IsValid)
                    {
                        IsBusy = true;
                        await Task.Delay(500);
                        var objectiveDetailPage = new ObjectiveDetailPage(FormHelper.EffectiveYear.Value);
                        await NavigationService.PushPageAsync(objectiveDetailPage);
                        objectiveDetailPage.OnPageClosed += ObjectiveDetailOnPageClosed;
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async void ExecuteViewAllObjectivesCommand()
        {
            using (Dialogs.Loading())
            {
                await Task.Delay(500);
                await NavigationService.PushModalAsync(new ObjectivesListPage(FormHelper.Objectives));
            }
        }

        private async void ExecuteViewObjectiveItemCommand(ObjectiveDetailDto item)
        {
            if (item != null)
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);

                    var objectiveDetailPage = new ObjectiveDetailPage(FormHelper.EffectiveYear.Value, item);
                    await NavigationService.PushPageAsync(objectiveDetailPage);
                    objectiveDetailPage.OnPageClosed += ObjectiveDetailOnPageClosed;
                }
            }
        }

        private void ObjectiveDetailOnPageClosed(object sender, bool isDetailAdded = false)
        {
            if (!isDetailAdded)
                return;

            Debug.WriteLine($"ObjectiveDetailOnPageClosed OnPageClosed invoked received {isDetailAdded}");

            ConsolidateObjectives();
        }

        private async Task ExecuteCancelCommand()
        {
            try
            {
                FormHelper.ActionTypeId = ActionTypeId.Cancel;
                FormHelper.Msg = Messages.Cancel;

                FormHelper = await service_.CancelRequest(FormHelper);

                if (FormHelper.Success)
                {
                    Success(true, Messages.ApprovalFormSuccessMessage);
                    await NavigationService.PopToRootAsync();
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        private async void ConsolidateObjectives()
        {
            if (IndividualObjectiveHelper.ObjectiveDetailChanged())
            {
                var values = IndividualObjectiveHelper.ObjectiveDetailDto();

                var tempId = (values.PODetailId == 0 ? values.TempRowId : values.PODetailId);

                var item = new ObjectiveDetailDto()
                {
                    BaseLine = values.BaseLine,
                    CustomKPI = values.CustomKPI,
                    GoalId = values.GoalId,
                    KPIId = values.KPIId,
                    KPIName = values.KPIName,
                    MeasureId = values.MeasureId,
                    MeasureName = values.MeasureName,
                    ObjectiveDescription = values.ObjectiveDescription,
                    ObjectiveDetail = values.ObjectiveDetail,
                    ObjectiveHeader = values.ObjectiveHeader,
                    Target = values.Target,
                    UnitOfMeasure = values.UnitOfMeasure,
                    RateScaleDto = values.RateScaleDto,
                    SelectedGoalDetail = values.SelectedGoalDetail,
                    IsRetrievedFromTemplate = values.IsRetrievedFromTemplate,
                    ParentObjective = values.ParentObjective,
                    PODetailId = values.PODetailId,
                    POHeaderId = values.POHeaderId,
                    RetrievedCompKPIId = values.RetrievedCompKPIId,
                    RetrievedPATemplateId = values.RetrievedPATemplateId,
                    RetrievedType = values.RetrievedType,
                    StandardCustomCriteria = values.StandardCustomCriteria,
                    StatusId = values.StatusId,
                    ShowLine = values.ShowLine,
                    TempRowId = (tempId == 0 ? Convert.ToInt64(DateTime.Now.ToString("MMddyyyyhhmmss")) : tempId),
                    IsDelete = values.IsDelete,
                    Weight = values.Weight,
                    TargetGoalSetup = values.TargetGoalSetup,
                    KPINameDisplay = values.KPINameDisplay,
                    Actual = values.Actual,
                    EmployeeRating = values.EmployeeRating,
                    EmployeeReview = values.EmployeeReview,
                    ManagerActual = values.ManagerActual,
                    ManagerReview = values.ManagerReview,
                    ManagerReviewRating = values.ManagerReviewRating,
                    Objectives = values.Objectives,
                    ActualWeightVal = values.ActualWeightVal,
                };

                var exists = item.PODetailId == 0
                    ? FormHelper.ObjectivesToSave.FirstOrDefault(x => x.TempRowId == item.TempRowId)
                    : FormHelper.ObjectivesToSave.FirstOrDefault(x => x.PODetailId == item.PODetailId);

                if (exists != null)
                    FormHelper.ObjectivesToSave.Remove(exists);

                if (!item.IsDelete || item.PODetailId != 0)
                {
                    FormHelper.ObjectivesToSave.Add(item);
                    FormHelper.AddedObjectiveCount++;
                }

                var list = new ObservableCollection<ObjectiveDetailDto>(
                        FormHelper.ObjectivesToSave.Where(x => !x.IsDelete).ToList()
                    );

                var groupings = await service_.GroupObjectives(list);
                FormHelper.Objectives = groupings.Objectives;
                FormHelper.ObjectivesLimited = groupings.ObjectivesLimited;
                FormHelper.IsExceeded = groupings.IsExceeded;
            }
        }

        private void ExecutePageAppearingCommand()
        {
            //ConsolidateObjectives();
        }

        private void ExecutePageDisappearingCommand()
        {
            IndividualObjectiveHelper.ObjectiveDetailChanged(false);
            IndividualObjectiveHelper.StandardObjectiveListChanged(false);
        }
    }
}