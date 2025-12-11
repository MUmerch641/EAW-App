using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views.IndividualObjectives;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.ViewModels.IndividualObjectives
{
    public class ObjectiveDetailViewModel : BaseViewModel
    {
        public ICommand SubmitCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand KPISelectionChangedCommand { get; set; }
        public ICommand ToggleRateScaleCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }
        public ICommand ToggleGoalsListCommand { get; set; }
        public ICommand ViewRateScaleCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        private ObjectiveDetailHolder holder_;

        public ObjectiveDetailHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly IIndividualObjectiveItemDataService service_;
        private readonly IDialogService dialogService_;

        public ObjectiveDetailViewModel()
        {
            service_ = AppContainer.Resolve<IIndividualObjectiveItemDataService>();
            dialogService_ = AppContainer.Resolve<IDialogService>();
        }

        public void Init(INavigation navigation, string year = "", ObjectiveDetailDto item = null)
        {
            NavigationBack = navigation;
            Holder = new ObjectiveDetailHolder();

            SubmitCommand = new Command(async () => await ExecuteSubmitCommand());
            DeleteItemCommand = new Command(async () => await ExecuteSubmitCommand(true));
            KPISelectionChangedCommand = new Command<R.Models.KPIDataDto>(ExecuteKPISelectionChangedCommand);
            ToggleRateScaleCommand = new Command(async () => await ExecuteToggleRateScaleCommand());
            PageAppearingCommand = new Command(ExecutePageAppearingCommand);
            PageDisappearingCommand = new Command(ExecutePageDisappearingCommand);
            ToggleGoalsListCommand = new Command(async () => await ExecuteToggleGoalsListCommand());
            ViewRateScaleCommand = new Command<RateScaleDto>(ExecuteViewRateScaleCommand);
            CloseCommand = new Command(ExecuteCloseCommand);

            InitForm(0, Convert.ToInt16(year), item);
        }

        private async void InitForm(long id = 0, short year = 0, ObjectiveDetailDto item = null)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    Holder = await service_.InitObjectiveDetailForm(id, year);
                    await SetValueObjectiveForm(item);
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

        private async Task SetValueObjectiveForm(ObjectiveDetailDto item)
        {
            if (item != null)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    Holder = await service_.SetValueObjectiveDetailForm(item, Holder);
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

        private async Task ExecuteSubmitCommand(bool IsDelete = false)
        {
            try
            {
                Holder.ExecuteSave(IsDelete);
                Holder = Holder;
                /*Holder = await service_.SaveObjectiveDetail(Holder);*/
                if (Holder.Valid)
                {
                    using (Dialogs.Loading())
                    {
                        await Task.Delay(500);
                        IndividualObjectiveHelper.ObjectiveDetailChanged(true);
                        IndividualObjectiveHelper.ObjectiveDetailDto(Holder.Model);
                        await NavigationService.PopPageAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        private async void ExecuteKPISelectionChangedCommand(R.Models.KPIDataDto item)
        {
            if (item != null)
            {
                Holder.ToggleTargetGoal = false;
                Holder.UnitOfMeasure.Value = string.Empty;
                Holder.ToggleUnitOfMeasure = true;
                Holder.IsCustomKPI = false;
                Holder.ToggleAddRateScaleButton = false;
                Holder.SelectedKPIDataDto = item;

                if (item.DisplayId > 0)
                {
                    if (!string.IsNullOrWhiteSpace(item.DisplaySource))
                    {
                        Holder.UnitOfMeasure.Value = item.DisplaySource;
                        Holder.ToggleUnitOfMeasure = false;
                    }

                    Holder.ToggleTargetGoal = true;

                    var response = await service_.RetrieveKPICriteria(item.DisplayId, Holder.RateScaleSource);
                    Holder.RateScaleSource = response.RateScales;
                    Holder.RateScaleSourceDisplay = response.RateScales;

                    if (!string.IsNullOrWhiteSpace(response.KPIObjective) && string.IsNullOrWhiteSpace(Holder.Objectives.Value))
                        Holder.Objectives.Value = response.KPIObjective;
                }
                else if (item.DisplayId < 0)
                {
                    Holder.Objectives = new Validations.ValidatableObject<string>();
                    Holder.IsCustomKPI = true;
                    Holder.ToggleTargetGoal = true;
                    Holder.ToggleUnitOfMeasure = true;
                    Holder.ToggleAddRateScaleButton = true;
                    Holder.RateScaleSource = new ObservableCollection<RateScaleDto>();
                    Holder.RateScaleSourceDisplay = new ObservableCollection<RateScaleDto>();
                }
            }
        }

        private async Task ExecuteToggleRateScaleCommand()
        {
            using (Dialogs.Loading())
            {
                await Task.Delay(500);
                var rateScaleModal = new RateScaleModalPage();
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(rateScaleModal);
                rateScaleModal.OnPageClosed += OnPageClosedRateScaleModalPage;
            }
        }

        private void OnPageClosedRateScaleModalPage(object sender, RateScaleDto model)
        {
            if (model == null)
                return;

            ManageRateScale(model);
        }

        private async Task ExecuteToggleGoalsListCommand()
        {
            var goalsListPage = new GoalsListPage(Holder);
            await NavigationService.PushModalAsync(goalsListPage);
            goalsListPage.OnPageClosed += GoalListOnPageClosed;
        }

        private void GoalListOnPageClosed(object sender, bool response)
        {
            if (!response)
                return;

            ManageGoal();
        }

        private async void ExecuteViewRateScaleCommand(RateScaleDto item)
        {
            if (item != null)
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    var isEditable = ((Holder.KPISelectedItem.DisplayId) < 0);
                    var modalPage = new RateScaleModalPage(item, isEditable);
                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(modalPage);
                    modalPage.OnPageClosed += OnPageClosedRateScaleModalPage;
                }
            }
        }

        private void ManageRateScale(RateScaleDto item)
        {
            if (item != null)
            {
                RateScaleDto exst = item.CriteriaId > 0
                    ? Holder.RateScaleSource.FirstOrDefault(x => x.CriteriaId == item.CriteriaId)
                    : Holder.RateScaleSource.FirstOrDefault(x => x.TempId == item.TempId);

                var index = item.CriteriaId > 0
                    ? Holder.RateScaleSource.ToList().FindIndex(x => x.CriteriaId == item.CriteriaId)
                    : Holder.RateScaleSource.ToList().FindIndex(x => x.TempId == item.TempId);

                if (exst != null)
                {
                    var newItem = new RateScaleDto()
                    {
                        Criteria = item.Criteria,
                        CriteriaId = exst.CriteriaId,
                        DisplayText1 = exst.DisplayText1,
                        Max = item.Max,
                        Min = item.Min,
                        Rating = item.Rating,
                        TempId = item.TempId,
                        IsDelete = item.IsDelete,
                    };

                    Holder.RateScaleSource.Remove(exst);
                    Holder.RateScaleSource.Insert(index, newItem);
                }
                else
                {
                    Holder.RateScaleSource.Add(new RateScaleDto()
                    {
                        Min = item.Min,
                        Max = item.Max,
                        Criteria = item.Criteria,
                        Rating = item.Rating,
                        DisplayText1 = string.Format("{0} {2} Rating: {1}",
                            (item.Min == item.Max ? item.Min.ToString() : item.Min.ToString() + " - " + item.Max.ToString()),
                            item.Rating.ToString(),
                            (!string.IsNullOrWhiteSpace(Holder.UnitOfMeasure.Value) ? Holder.UnitOfMeasure.Value : "unit")
                        ),
                        CriteriaId = item.CriteriaId,
                        TempId = Convert.ToInt64(DateTime.Now.ToString("MMddyyyyhhmmss")),
                        //IsDelete = item.IsDelete,
                    });
                }

                Holder.RateScaleSourceDisplay = new ObservableCollection<RateScaleDto>(
                        Holder.RateScaleSource.Where(x => x.IsDelete == false).ToList()
                    );

                Holder = Holder;
            }
        }

        private void ManageGoal()
        {
            if (IndividualObjectiveHelper.SelectedGoalDetailDtoChanged())
            {
                var goal = IndividualObjectiveHelper.SelectedGoalDetailDto();
                Holder.GoalId = goal.DetailId;
                /*Holder.Goal.Value = goal.Name;*/
                Holder.Goal.Value = goal.HeaderDetailName;
                Holder.ObjectiveDetail = goal.Name;
                Holder.GoalDescription.Value = goal.Description;
                Holder.SelectedGoalDetail = goal;
                Holder.ParentObjective = goal.HeaderDetailName;
            }
        }

        private void ExecutePageAppearingCommand()
        {
            //MessagingCenter.Subscribe<RateScaleViewModel, RateScaleDto>(this, "RateScaleCompleted", (s, param) =>
            //{
            //    ManageRateScale(param);
            //});

            MessagingCenter.Subscribe<ObjectiveDetailPage>(this, "onback", (sender) =>
            {
                BackItemPage();
            });

            /*ManageGoal();*/
        }

        private void ExecutePageDisappearingCommand()
        {
            MessagingCenter.Unsubscribe<RateScaleViewModel, RateScaleDto>(this, "RateScaleCompleted");

            IndividualObjectiveHelper.SelectedGoalDetailDtoChanged(false);
        }

        private async void ExecuteCloseCommand()
        {
            bool hasUnsavedChanges = Holder.POHeaderId == 0 &&
                                     (!string.IsNullOrWhiteSpace(Holder.Goal.Value) ||
                                      !string.IsNullOrWhiteSpace(Holder.Objectives.Value) ||
                                      !string.IsNullOrWhiteSpace(Holder.KPISelectedItem.DisplayData) ||
                                      Holder.RateScaleSourceDisplay.Count > 0);

            if (hasUnsavedChanges)
            {
                if (await dialogService_.ConfirmDialogAsync(Messages.LEAVEPAGE))
                {
                    base.BackItemPage();
                }
            }
            else
            {
                base.BackItemPage();
            }
        }

        protected override void BackItemPage()
        {
            ExecuteCloseCommand();
        }
    }
}