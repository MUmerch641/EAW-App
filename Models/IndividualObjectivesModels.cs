using System.Collections.ObjectModel;
using System.ComponentModel;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Utils;
using MauiHybridApp.Models.Validation;

namespace MauiHybridApp.Models.IndividualObjectives
{
    public class IndividualObjectivesDto : INotifyPropertyChanged
    {
        public long IndividualOjbectiveId { get; set; }
        public long ProfileId { get; set; }
        public long StatusId { get; set; }
        public string Status { get; set; }
        public string Period { get; set; }
        public DateTime DatePrepared { get; set; }
        public int EffectiveYear { get; set; }
        public string Details { get; set; }
        public string Header { get; set; }
        public string DatePrepared_String { get; set; }
        public string Icon { get; set; }
        public string Icon2 { get; set; }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked")); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class ObjectiveDetailHeaderDto
    {
        public ObjectiveDetailHeaderDto()
        {
            ObjectiveDetailDto = new ObservableCollection<ObjectiveDetailDto>();
            IsEditable = true;
            IsDeleted = false;
            IsOpened = false;
        }

        public string ObjectiveHeader { get; set; }
        public bool IsEditable { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsOpened { get; set; }

        public ObservableCollection<ObjectiveDetailDto> ObjectiveDetailDto { get; set; }
    }

    public class ObjectiveDetailDto
    {
        public ObjectiveDetailDto()
        {
            SelectedGoalDetail = new GoalDetailDto();
            StandardCustomCriteria = new ObservableCollection<RateScaleDto>();
            RateScaleDto = new ObservableCollection<RateScaleDto>();
            ShowLine = true;
            IsRetrievedFromTemplate = false;
            IsDelete = false;
            StatusId = 0; // Draft
        }

        public long GoalId { get; set; }
        public long KPIId { get; set; }
        public string KPIName { get; set; }
        public string CustomKPI { get; set; }
        public long MeasureId { get; set; }
        public string MeasureName { get; set; }
        public string Target { get; set; }
        public string UnitOfMeasure { get; set; }
        public string BaseLine { get; set; }
        public string ObjectiveDetail { get; set; }
        public string ObjectiveDescription { get; set; }
        public string ObjectiveHeader { get; set; }
        public long PODetailId { get; set; }
        public long POHeaderId { get; set; }
        public long TempRowId { get; set; }
        public string ParentObjective { get; set; }
        public bool ShowLine { get; set; }
        public bool IsRetrievedFromTemplate { get; set; }
        public short RetrievedType { get; set; }
        public long RetrievedPATemplateId { get; set; }
        public long RetrievedCompKPIId { get; set; }
        public long StatusId { get; set; }
        public bool IsDelete { get; set; }
        public string Weight { get; set; }
        public string ActualWeightVal { get; set; }
        public string EmployeeReview { get; set; }
        public string Actual { get; set; }
        public decimal EmployeeRating { get; set; }
        public string ManagerReview { get; set; }
        public string ManagerActual { get; set; }
        public decimal ManagerReviewRating { get; set; }
        public string TargetGoalSetup { get; set; }
        public string KPINameDisplay { get; set; }
        public GoalDetailDto SelectedGoalDetail { get; set; }
        public ObservableCollection<RateScaleDto> RateScaleDto { get; set; }
        public ObservableCollection<RateScaleDto> StandardCustomCriteria { get; set; }
        public string Objectives { get; set; }
    }

    public class MainObjectiveDto
    {
        public MainObjectiveDto()
        {
            ObjectiveDetailHeaderDto = new ObservableCollection<ObjectiveDetailHeaderDto>();
            IsEditable = true;
            IsDeleted = false;
            IsOpened = false;
        }

        public string Header { get; set; }
        public bool IsEditable { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsOpened { get; set; }

        public ObservableCollection<ObjectiveDetailHeaderDto> ObjectiveDetailHeaderDto { get; set; }
    }

    public class ObjectiveGroupingResponse
    {
        public ObjectiveGroupingResponse()
        {
            Objectives = new ObservableCollection<MainObjectiveDto>();
            ObjectivesLimited = new ObservableCollection<MainObjectiveDto>();
            ObjectivesToSave = new ObservableCollection<ObjectiveDetailDto>();
            IsExceeded = false;
        }

        public bool IsExceeded { get; set; }
        public ObservableCollection<MainObjectiveDto> Objectives { get; set; }
        public ObservableCollection<MainObjectiveDto> ObjectivesLimited { get; set; }
        public ObservableCollection<ObjectiveDetailDto> ObjectivesToSave { get; set; }
    }

    // Placeholder for GoalDetailDto if not already defined elsewhere
    public class GoalDetailDto
    {
        public long DetailId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HeaderDetailName { get; set; }
    }
    
    // Placeholder for RateScaleDto if not already defined elsewhere
    public class RateScaleDto
    {
        public long CriteriaId { get; set; }
        public string Criteria { get; set; }
        public string DisplayText1 { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public decimal Rating { get; set; }
        public long TempId { get; set; }
        public bool IsDelete { get; set; }
    }
    
    // Placeholder for GoalHeaderDetailDto
    public class GoalHeaderDetailDto
    {
        // Add properties as needed
    }

    public class IndividualObjectiveItemHolder : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public IndividualObjectiveItemHolder()
        {
            Objectives = new ObservableCollection<MainObjectiveDto>();
            ObjectivesLimited = new ObservableCollection<MainObjectiveDto>();
            ObjectivesToSave = new ObservableCollection<ObjectiveDetailDto>();
            EffectiveYear = new ValidatableObject<string>();
            IsExceeded = false;
            IsSaveOnly = false;
            Success = false;
            ActionTypeId = 0;
            Msg = string.Empty;
            AddedObjectiveCount = 0;
        }

        public ObservableCollection<MainObjectiveDto> Objectives { get; set; }
        public ObservableCollection<MainObjectiveDto> ObjectivesLimited { get; set; }
        public ObservableCollection<ObjectiveDetailDto> ObjectivesToSave { get; set; }
        public ValidatableObject<string> EffectiveYear { get; set; }
        public bool IsExceeded { get; set; }
        public bool IsSaveOnly { get; set; }
        public bool Success { get; set; }
        public int ActionTypeId { get; set; }
        public string Msg { get; set; }
        public int AddedObjectiveCount { get; set; }
        public bool IsEnabled { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool ShowButton { get; set; }
        public bool ShowCancelButton { get; set; }

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class KPISelectionResponse
    {
        public ObservableCollection<RateScaleDto> RateScales { get; set; }
        public string KPIObjective { get; set; }
    }

    public class ObjectiveDetailHolder : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

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
            KPISource = new ObservableCollection<MauiHybridApp.Models.DataObjects.SelectableListModel>(); // Adapted type if needed
            // KPISelectedItem = new R.Models.KPIDataDto(); // Need to define KPIDataDto
            IsCustomKPI = false;
            MeasureSource = new ObservableCollection<MauiHybridApp.Models.DataObjects.SelectableListModel>();
            MeasureSelectedItem = new MauiHybridApp.Models.DataObjects.SelectableListModel();
            RateScaleSource = new ObservableCollection<RateScaleDto>();
            Model = new ObjectiveDetailDto();
            EffectiveYear = string.Empty;
            DepartmentName = string.Empty;
            // ParentObjectiveList = new List<R.Models.ObjectiveDataDto>();
            // OtherObjectiveList = new List<R.Models.OrganizationGoalDto2>();
            SelectedGoalDetail = new GoalDetailDto();
            Valid = false;
            ShowKPI = false;
            // BaseLineHelper = new R.Models.FieldLookupHelperModel();
            // KPIHelper = new R.Models.FieldLookupHelperModel();
            ToggleUnitOfMeasure = true;
            ToggleTargetGoal = false;
            ToggleAddRateScaleButton = false;
            RateScaleSourceDisplay = new ObservableCollection<RateScaleDto>();
            IsEditable = false;
            Weight = new ValidatableObject<string>();
            ShowWeight = false;
            GoalHeaderDetails = new ObservableCollection<GoalHeaderDetailDto>();
            // SelectedKPIDataDto = new R.Models.KPIDataDto();
            ExistingRateScaleSource = new ObservableCollection<RateScaleDto>();
        }

        public ValidatableObject<string> Goal { get; set; }
        public ValidatableObject<string> GoalDescription { get; set; }
        public long GoalId { get; set; }
        public ValidatableObject<string> Objectives { get; set; }
        public ValidatableObject<long> KPI { get; set; }
        public ValidatableObject<string> CustomKPI { get; set; }
        public ValidatableObject<long> Measure { get; set; }
        public ValidatableObject<string> Target { get; set; }
        public ValidatableObject<string> UnitOfMeasure { get; set; }
        public ValidatableObject<string> Baseline { get; set; }
        
        // Simplified for now
        public ObservableCollection<MauiHybridApp.Models.DataObjects.SelectableListModel> KPISource { get; set; }
        public bool IsCustomKPI { get; set; }
        public ObservableCollection<MauiHybridApp.Models.DataObjects.SelectableListModel> MeasureSource { get; set; }
        public MauiHybridApp.Models.DataObjects.SelectableListModel MeasureSelectedItem { get; set; }
        public ObservableCollection<RateScaleDto> RateScaleSource { get; set; }
        public ObservableCollection<RateScaleDto> RateScaleSourceDisplay { get; set; }
        public ObservableCollection<RateScaleDto> ExistingRateScaleSource { get; set; }
        
        public string EffectiveYear { get; set; }
        public string DepartmentName { get; set; }
        public GoalDetailDto SelectedGoalDetail { get; set; }
        public bool Valid { get; set; }
        public bool ShowKPI { get; set; }
        public bool ToggleUnitOfMeasure { get; set; }
        public bool ToggleTargetGoal { get; set; }
        public bool ToggleAddRateScaleButton { get; set; }
        public bool IsEditable { get; set; }
        public ValidatableObject<string> Weight { get; set; }
        public bool ShowWeight { get; set; }
        public ObjectiveDetailDto Model { get; set; }
        public ObservableCollection<GoalHeaderDetailDto> GoalHeaderDetails { get; set; }

        public long TempRowId { get; set; }
        public long PODetailId { get; set; }
        public long POHeaderId { get; set; }
        public string ParentObjective { get; set; }
        public string ObjectiveDetail { get; set; }

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
