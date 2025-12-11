using EatWork.Mobile.Contants;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Validations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Models.FormHolder.IndividualObjectives
{
    public class ObjectiveDetailHolder : ExtendedBindableObject
    {
        public ObjectiveDetailHolder()
        {
            Goal = new ValidatableObject<string>();
            GoalDescription = new ValidatableObject<string>();
            Objectives = new ValidatableObject<string>();
            KPI = new ValidatableObject<long>();
            CustomKPI = new ValidatableObject<string>();
            Measure = new ValidatableObject<long>();
            Target = new ValidatableObject<string>();
            UnitOfMeasure = new ValidatableObject<string>();
            Baseline = new ValidatableObject<string>();
            KPISource = new ObservableCollection<R.Models.KPIDataDto>();
            KPISelectedItem = new R.Models.KPIDataDto();
            IsCustomKPI = false;
            MeasureSource = new ObservableCollection<SelectableListModel>();
            MeasureSelectedItem = new SelectableListModel();
            RateScaleSource = new ObservableCollection<RateScaleDto>();
            Model = new ObjectiveDetailDto();
            EffectiveYear = string.Empty;
            DepartmentName = string.Empty;
            ParentObjectiveList = new List<R.Models.ObjectiveDataDto>();
            OtherObjectiveList = new List<R.Models.OrganizationGoalDto2>();
            SelectedGoalDetail = new GoalDetailDto();
            Valid = false;
            ShowKPI = false;
            BaseLineHelper = new R.Models.FieldLookupHelperModel();
            KPIHelper = new R.Models.FieldLookupHelperModel();
            ToggleUnitOfMeasure = true;
            ToggleTargetGoal = false;
            ToggleAddRateScaleButton = false;
            RateScaleSourceDisplay = new ObservableCollection<RateScaleDto>();
            IsEditable = false;
            Weight = new ValidatableObject<string>();
            ShowWeight = false;
            GoalHeaderDetails = new ObservableCollection<GoalHeaderDetailDto>();
            SelectedKPIDataDto = new R.Models.KPIDataDto();
            ExistingRateScaleSource = new ObservableCollection<RateScaleDto>();
        }

        private ValidatableObject<string> goal_;

        public ValidatableObject<string> Goal
        {
            get { return goal_; }
            set { goal_ = value; RaisePropertyChanged(() => Goal); }
        }

        private ValidatableObject<string> goalDescription_;

        public ValidatableObject<string> GoalDescription
        {
            get { return goalDescription_; }
            set { goalDescription_ = value; RaisePropertyChanged(() => GoalDescription); }
        }

        private long goalId_;

        public long GoalId
        {
            get { return goalId_; }
            set { goalId_ = value; RaisePropertyChanged(() => GoalId); }
        }

        private ValidatableObject<string> objectives_;

        public ValidatableObject<string> Objectives
        {
            get { return objectives_; }
            set { objectives_ = value; RaisePropertyChanged(() => Objectives); }
        }

        private ValidatableObject<long> kpi_;

        public ValidatableObject<long> KPI
        {
            get { return kpi_; }
            set { kpi_ = value; RaisePropertyChanged(() => KPI); }
        }

        private ValidatableObject<string> customKPI_;

        public ValidatableObject<string> CustomKPI
        {
            get { return customKPI_; }
            set { customKPI_ = value; RaisePropertyChanged(() => CustomKPI); }
        }

        private ValidatableObject<long> measure_;

        public ValidatableObject<long> Measure
        {
            get { return measure_; }
            set { measure_ = value; RaisePropertyChanged(() => Measure); }
        }

        private ValidatableObject<string> target_;

        public ValidatableObject<string> Target
        {
            get { return target_; }
            set { target_ = value; RaisePropertyChanged(() => Target); }
        }

        private ValidatableObject<string> unitOfMeasure_;

        public ValidatableObject<string> UnitOfMeasure
        {
            get { return unitOfMeasure_; }
            set { unitOfMeasure_ = value; RaisePropertyChanged(() => UnitOfMeasure); }
        }

        private ValidatableObject<string> baseline_;

        public ValidatableObject<string> Baseline
        {
            get { return baseline_; }
            set { baseline_ = value; RaisePropertyChanged(() => Baseline); }
        }

        private ObservableCollection<R.Models.KPIDataDto> kpiSource_;

        public ObservableCollection<R.Models.KPIDataDto> KPISource
        {
            get { return kpiSource_; }
            set { kpiSource_ = value; RaisePropertyChanged(() => KPISource); }
        }

        private R.Models.KPIDataDto kpiSelectedItem_;

        public R.Models.KPIDataDto KPISelectedItem
        {
            get { return kpiSelectedItem_; }
            set { kpiSelectedItem_ = value; RaisePropertyChanged(() => KPISelectedItem); }
        }

        private bool isCustomKPI_;

        public bool IsCustomKPI
        {
            get { return isCustomKPI_; }
            set { isCustomKPI_ = value; RaisePropertyChanged(() => IsCustomKPI); }
        }

        private ObservableCollection<SelectableListModel> measureSource_;

        public ObservableCollection<SelectableListModel> MeasureSource
        {
            get { return measureSource_; }
            set { measureSource_ = value; RaisePropertyChanged(() => MeasureSource); }
        }

        private SelectableListModel measureSelectedItem_;

        public SelectableListModel MeasureSelectedItem
        {
            get { return measureSelectedItem_; }
            set { measureSelectedItem_ = value; RaisePropertyChanged(() => MeasureSelectedItem); }
        }

        private ObservableCollection<RateScaleDto> rateScaleSource_;

        public ObservableCollection<RateScaleDto> RateScaleSource
        {
            get { return rateScaleSource_; }
            set { rateScaleSource_ = value; RaisePropertyChanged(() => RateScaleSource); }
        }

        private ObservableCollection<RateScaleDto> rateScaleSourceDisplay_;

        public ObservableCollection<RateScaleDto> RateScaleSourceDisplay
        {
            get { return rateScaleSourceDisplay_; }
            set { rateScaleSourceDisplay_ = value; RaisePropertyChanged(() => RateScaleSourceDisplay); }
        }

        private ObservableCollection<RateScaleDto> existingRateScaleSource_;

        public ObservableCollection<RateScaleDto> ExistingRateScaleSource
        {
            get { return existingRateScaleSource_; }
            set { existingRateScaleSource_ = value; RaisePropertyChanged(() => ExistingRateScaleSource); }
        }

        private string effectiveYear_;

        public string EffectiveYear
        {
            get { return effectiveYear_; }
            set { effectiveYear_ = value; RaisePropertyChanged(() => EffectiveYear); }
        }

        private string departmentName_;

        public string DepartmentName
        {
            get { return departmentName_; }
            set { departmentName_ = value; RaisePropertyChanged(() => DepartmentName); }
        }

        private List<R.Models.ObjectiveDataDto> parentObjectiveList_;

        public List<R.Models.ObjectiveDataDto> ParentObjectiveList
        {
            get { return parentObjectiveList_; }
            set { parentObjectiveList_ = value; RaisePropertyChanged(() => ParentObjectiveList); }
        }

        private List<R.Models.OrganizationGoalDto2> otherObjectiveList_;

        public List<R.Models.OrganizationGoalDto2> OtherObjectiveList
        {
            get { return otherObjectiveList_; }
            set { otherObjectiveList_ = value; RaisePropertyChanged(() => OtherObjectiveList); }
        }

        private GoalDetailDto selectedGoalDetail_;

        public GoalDetailDto SelectedGoalDetail
        {
            get { return selectedGoalDetail_; }
            set { selectedGoalDetail_ = value; RaisePropertyChanged(() => SelectedGoalDetail); }
        }

        private bool valid_;

        public bool Valid
        {
            get { return valid_; }
            set { valid_ = value; RaisePropertyChanged(() => Valid); }
        }

        private bool showKPI_;

        public bool ShowKPI
        {
            get { return showKPI_; }
            set { showKPI_ = value; RaisePropertyChanged(() => ShowKPI); }
        }

        private R.Models.FieldLookupHelperModel baseLineHelper_;

        public R.Models.FieldLookupHelperModel BaseLineHelper
        {
            get { return baseLineHelper_; }
            set { baseLineHelper_ = value; RaisePropertyChanged(() => BaseLineHelper); }
        }

        private R.Models.FieldLookupHelperModel kpiHelper_;

        public R.Models.FieldLookupHelperModel KPIHelper
        {
            get { return kpiHelper_; }
            set { kpiHelper_ = value; RaisePropertyChanged(() => KPIHelper); }
        }

        private bool toggleUnitOfMeasure_;

        public bool ToggleUnitOfMeasure
        {
            get { return toggleUnitOfMeasure_; }
            set { toggleUnitOfMeasure_ = value; RaisePropertyChanged(() => ToggleUnitOfMeasure); }
        }

        private bool toggleTargetGoal_;

        public bool ToggleTargetGoal
        {
            get { return toggleTargetGoal_; }
            set { toggleTargetGoal_ = value; RaisePropertyChanged(() => ToggleTargetGoal); }
        }

        private bool toggleAddRateScaleButton_;

        public bool ToggleAddRateScaleButton
        {
            get { return toggleAddRateScaleButton_; }
            set { toggleAddRateScaleButton_ = value; RaisePropertyChanged(() => ToggleAddRateScaleButton); }
        }

        private bool isEditable_;

        public bool IsEditable
        {
            get { return isEditable_; }
            set { isEditable_ = value; }
        }

        private ValidatableObject<string> weight_;

        public ValidatableObject<string> Weight
        {
            get { return weight_; }
            set { weight_ = value; RaisePropertyChanged(() => Weight); }
        }

        private bool showWeight_;

        public bool ShowWeight
        {
            get { return showWeight_; }
            set { showWeight_ = value; RaisePropertyChanged(() => ShowWeight); }
        }

        private ObjectiveDetailDto model_;

        public ObjectiveDetailDto Model
        {
            get { return model_; }
            set { model_ = value; }
        }

        public long TempRowId { get; set; }
        public long PODetailId { get; set; }
        public long POHeaderId { get; set; }
        public string ParentObjective { get; set; }

        private string objectiveDetail_;

        public string ObjectiveDetail
        {
            get { return objectiveDetail_; }
            set { objectiveDetail_ = value; RaisePropertyChanged(() => ObjectiveDetail); }
        }

        private ObservableCollection<GoalHeaderDetailDto> goalHeaderDetails_;

        public ObservableCollection<GoalHeaderDetailDto> GoalHeaderDetails
        {
            get { return goalHeaderDetails_; }
            set { goalHeaderDetails_ = value; RaisePropertyChanged(() => GoalHeaderDetails); }
        }

        private R.Models.KPIDataDto selectedKPIDataDto_;

        public R.Models.KPIDataDto SelectedKPIDataDto
        {
            get { return selectedKPIDataDto_; }
            set { selectedKPIDataDto_ = value; RaisePropertyChanged(() => SelectedKPIDataDto); }
        }

        public bool ExecuteSave(bool IsDelete = false)
        {
            var measurename = string.Empty;
            var kpiName = string.Empty;
            if (KPISelectedItem != null && KPISelectedItem.DisplayId > 0)
            {
                kpiName = KPISelectedItem.DisplayField;
                measurename = KPISelectedItem.DisplayData;
            }
            else
            {
                if (MeasureSelectedItem != null)
                    measurename = MeasureSelectedItem.DisplayText;

                if (!string.IsNullOrWhiteSpace(CustomKPI.Value))
                    kpiName = CustomKPI.Value;
            }

            /*var target = Convert.ToDecimal(Target.Value);*/
            var targetIsNumber = decimal.TryParse(Target.Value, out decimal target);
            var targetVal = targetIsNumber ? Target.Value : target.ToString("n2");

            var baselineIsNumber = decimal.TryParse(Baseline.Value, out decimal baseline);
            var baselineVal = !baselineIsNumber ? Baseline.Value : baseline.ToString("N0");

            var rateScales = new ObservableCollection<RateScaleDto>();

            Model = new ObjectiveDetailDto()
            {
                BaseLine = baselineVal,
                CustomKPI = CustomKPI.Value,
                GoalId = GoalId,
                KPIId = (KPISelectedItem != null ? KPISelectedItem.DisplayId : 0),
                MeasureId = (MeasureSelectedItem != null ? MeasureSelectedItem.Id : 0),
                /*ObjectiveDescription = Objectives.Value,*/
                ObjectiveDescription = GoalDescription.Value,
                Target = Target.Value,
                UnitOfMeasure = UnitOfMeasure.Value,
                RateScaleDto = RateScaleSource,
                /*KPIName = (KPISelectedItem != null ? KPISelectedItem.DisplayData : string.Empty),*/
                KPIName = kpiName, /*(KPISelectedItem != null ? KPISelectedItem.DisplayField : string.Empty),*/
                /*MeasureName = (MeasureSelectedItem != null ? MeasureSelectedItem.DisplayText : string.Empty),*/
                MeasureName = measurename,
                ObjectiveDetail = (SelectedGoalDetail != null ? SelectedGoalDetail.Name : string.Empty),
                ObjectiveHeader = (SelectedGoalDetail != null ? SelectedGoalDetail.HeaderDetailName : string.Empty),
                SelectedGoalDetail = SelectedGoalDetail,
                IsDelete = IsDelete,
                TempRowId = TempRowId,
                PODetailId = PODetailId,
                POHeaderId = POHeaderId,
                ParentObjective = (PODetailId > 0 ? ParentObjective : (SelectedGoalDetail != null ? SelectedGoalDetail.HeaderDetailName : Constants.NoOrgGoalsConstant)),
                /*ParentObjective = (PODetailId > 0 ? ParentObjective : (SelectedGoalDetail != null ? Constants.OrgGoalsConstant : Constants.NoOrgGoalsConstant)),*/
                Weight = (Convert.ToDecimal(Weight.Value)).ToString("N0"),
                ActualWeightVal = (Convert.ToDecimal(Weight.Value)).ToString("N0"),
                /*TargetGoalSetup = $"{target.ToString("n2")} {UnitOfMeasure.Value}",*/
                TargetGoalSetup = $"{(measurename == "Amount" ? (UnitOfMeasure.Value + " " + targetVal) : (targetVal + " " + UnitOfMeasure.Value))}",
                KPINameDisplay = kpiName,
                Objectives = Objectives.Value,
            };

            if (IsDelete)
                Valid = true;
            else
                Valid = IsValid();

            return Valid;
        }

        public bool IsValid()
        {
            Goal.Validations.Clear();
            /*
            Goal.Validations.Add(new IsNotNullOrEmptyRule<string>()
            {
                ValidationMessage = string.Empty
            });
            */
            Objectives.Validations.Clear();
            Objectives.Validations.Add(new IsNotNullOrEmptyRule<string>()
            {
                ValidationMessage = string.Empty
            });

            KPI.Validations.Clear();
            /*
            KPI.Validations.Add(new NumberRule<long>()
            {
                ValidationMessage = string.Empty
            });
            */

            Measure.Validations.Clear();
            /*
            Measure.Validations.Add(new NumberRule<long>()
            {
                ValidationMessage = string.Empty
            });
            */

            Target.Validations.Clear();
            /*
            Target.Validations.Add(new IsNotNullOrEmptyRule<string>()
            {
                ValidationMessage = string.Empty
            });
            */

            UnitOfMeasure.Validations.Clear();
            /*
            UnitOfMeasure.Validations.Add(new IsNotNullOrEmptyRule<string>()
            {
                ValidationMessage = string.Empty
            });
            */

            Baseline.Validations.Clear();
            /*
            Baseline.Validations.Add(new IsNotNullOrEmptyRule<string>()
            {
                ValidationMessage = string.Empty
            });
            */

            CustomKPI.Validations.Clear();

            Goal.Validate();
            Objectives.Validate();
            KPI.Validate();
            Measure.Validate();
            Target.Validate();
            UnitOfMeasure.Validate();
            Baseline.Validate();
            CustomKPI.Validate();

            if (ShowKPI)
            {
                if (KPISelectedItem != null)
                {
                    if (KPISelectedItem.DisplayId == 0)
                    {
                        KPI.Errors.Add("This field is required.");
                    }

                    if (KPISelectedItem.DisplayId < 0)
                    {
                        if (string.IsNullOrWhiteSpace(CustomKPI.Value))
                        {
                            CustomKPI.Errors.Add("This field is required.");
                        }

                        if (MeasureSelectedItem != null)
                        {
                            if (MeasureSelectedItem.Id == 0)
                                Measure.Errors.Add("This field is required.");
                        }

                        if (string.IsNullOrWhiteSpace(Weight.Value) && ShowWeight)
                        {
                            Weight.Errors.Add("This field is required.");
                        }
                    }

                    /*
                    if (MeasureSelectedItem != null)
                    {
                        if (MeasureSelectedItem.Id == 0 && KPISelectedItem.DisplayId < 0)
                        {
                            Measure.Errors.Add("This field is required.");
                        }
                    }
                    */
                }

                if (string.IsNullOrWhiteSpace(Target.Value))
                {
                    Target.Errors.Add("This field is required.");
                }

                if (string.IsNullOrWhiteSpace(Baseline.Value))
                {
                    Baseline.Errors.Add("This field is required.");
                }

                if (string.IsNullOrWhiteSpace(Baseline.Value))
                {
                    Baseline.Errors.Add("This field is required.");
                }

                if (string.IsNullOrWhiteSpace(UnitOfMeasure.Value))
                {
                    UnitOfMeasure.Errors.Add("This field is required.");
                }
            }

            if (BaseLineHelper.RequiredTag
                && string.IsNullOrWhiteSpace(Baseline.Value)
                && !BaseLineHelper.HideTag)
            {
                Baseline.Errors.Add("This field is required.");
            }

            return Goal.IsValid &&
                   Objectives.IsValid &&
                   KPI.IsValid &&
                   Measure.IsValid &&
                   UnitOfMeasure.IsValid &&
                   Baseline.IsValid &&
                   Target.IsValid &&
                   CustomKPI.IsValid;
        }
    }
}