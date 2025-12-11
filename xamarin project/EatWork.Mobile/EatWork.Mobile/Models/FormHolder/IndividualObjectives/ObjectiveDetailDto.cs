using EatWork.Mobile.Contants;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.IndividualObjectives
{
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
            StatusId = RequestStatusValue.Draft;
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
}