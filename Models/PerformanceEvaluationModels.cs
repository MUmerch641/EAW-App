using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace MauiHybridApp.Models.PerformanceEvaluation;

public class PEListDto
{
    public long RecordId { get; set; }
    
    [JsonProperty("paTypeTitle")]
    public string EvaluationType { get; set; } = string.Empty;
    
    [JsonProperty("evaluationStatus")]
    public string Status { get; set; } = string.Empty;
    
    public long StatusId { get; set; }
    
    [JsonProperty("evaluationPeriod")]
    public string PeriodCovered { get; set; } = string.Empty;
    
    public string ScheduledDate { get; set; } = string.Empty;
    public string DueDate_String { get; set; } = string.Empty;
    public long ProfileId { get; set; }
    
    [JsonProperty("periodCoveredStartDate")]
    public DateTime? PeriodStartDate { get; set; }
    
    [JsonProperty("periodCoveredEndDate")]
    public DateTime? PeriodEndDate { get; set; }
    
    [JsonProperty("paScheduleStartDate")]
    public DateTime? ScheduledStartDate { get; set; }
    
    [JsonProperty("paScheduleEndDate")]
    public DateTime? ScheduledEndDate { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    [JsonProperty("employeeName")]
    public string EmployeeName { get; set; } = string.Empty;
}

public class PEFormHolder
{
    public long RecordId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string HireDate { get; set; } = string.Empty;
    public string AppraisalType { get; set; } = string.Empty;
    public string EvaluatorType { get; set; } = string.Empty;
    public string EvaluatorName { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public double Progress { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    
    public ObservableCollection<ObjectiveDetailDto> Objectives { get; set; } = new();
    public ObservableCollection<MainObjectiveDto> ObjectivesDisplay { get; set; } = new();
    
    // Questionnaire & Narrative placeholders for now
    public ObservableCollection<string> CompQuestions { get; set; } = new();
    public bool HasError { get; set; }
    public bool ForSubmission { get; set; }
    public bool CanSave { get; set; }
    public bool CanSubmit { get; set; }
}

public class ObjectiveDetailDto
{
    public long PODetailId { get; set; }
    public long POHeaderId { get; set; }
    public string ObjectiveHeader { get; set; } = string.Empty;
    public string KPIName { get; set; } = string.Empty;
    public string KPINameDisplay { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;
    public string ActualWeightVal { get; set; } = string.Empty;
    
    // Inputs
    public string EmployeeReview { get; set; } = string.Empty;
    public string Actual { get; set; } = string.Empty;
    public decimal EmployeeRating { get; set; }
    
    public string ManagerReview { get; set; } = string.Empty;
    public string ManagerActual { get; set; } = string.Empty;
    public decimal ManagerReviewRating { get; set; }
}

public class MainObjectiveDto
{
    public string Header { get; set; } = string.Empty;
    public ObservableCollection<ObjectiveDetailHeaderDto> ObjectiveDetailHeaderDto { get; set; } = new();
}

public class ObjectiveDetailHeaderDto
{
    public string ObjectiveHeader { get; set; } = string.Empty;
    public ObservableCollection<ObjectiveDetailDto> ObjectiveDetailDto { get; set; } = new();
}
