using System;
using System.Collections.Generic;

namespace MauiHybridApp.Models.IndividualObjectives
{
    public class EmployeeIndividualObjectiveList
    {
        public long RecordId { get; set; }
        public long StatusId { get; set; }
        public string Status { get; set; }
        public string Period { get; set; }
        public DateTime DatePrepared { get; set; }
        public int? EffectiveYear { get; set; }
        public string Details { get; set; }
        public string Header { get; set; }
        public long ProfileId { get; set; }
    }

    public class PerformanceObjectiveResponse
    {
        public PerformanceObjectiveHeader PerformanceObjectiveHeader { get; set; }
        public EmployeeInfo EmployeeInfo { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
    }

    public class EmployeeInfo
    {
        public string FullNameMiddleInitialOnly { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
    }

    public class PerformanceObjectiveHeader
    {
        public long PerformanceObjectiveHeaderId { get; set; }
        public long ProfileId { get; set; }
        public DateTime? DatePrepared { get; set; }
        public int StatusId { get; set; }
        public int? EffectiveYear { get; set; }
        public int PeriodType { get; set; }
        public short SourceId { get; set; }
        public List<PerformanceObjectiveDetail> PerformanceObjectiveDetail { get; set; } = new();
    }

    public class PerformanceObjectiveDetail
    {
        public long PerformanceObjectiveDetailId { get; set; }
        public long? PerformanceObjectiveHeaderId { get; set; }
        public long? OrganizationGoalId { get; set; }
        public long? KeyPerformanceIndicatorId { get; set; }
        public string CustomKPI { get; set; }
        public int? Measurement { get; set; }
        public string Objectives { get; set; }
        public string ObjectiveName { get; set; }
        public string ObjectiveDescription { get; set; }
        public string ParentObjective { get; set; }
        public decimal? TargetGoal { get; set; }
        public string UnitOfMeasure { get; set; }
        public string BaseLine { get; set; }
        public decimal? Weight { get; set; }
        public List<POCustomCriteria> POCustomCriteria { get; set; } = new();
        public bool? IsRetrievedFromTemplate { get; set; }
        public short? RetrievedType { get; set; }
        public long? RetrievedPATemplateId { get; set; }
        public long? RetrievedCompKPIId { get; set; }
        public bool IsDelete { get; set; }
        
        // Extra fields for submit
        public decimal EmployeeRating { get; set; }
        public string EmployeeReview { get; set; }
        public string Actual { get; set; }
        public string ManagerActual { get; set; }
        public string ManagerReview { get; set; }
        public decimal ManagerReviewRating { get; set; }
        public string ManagerReviewRemarks { get; set; }
        public decimal Rating { get; set; }
        public bool UseCustomCriteria { get; set; }
    }

    public class POCustomCriteria
    {
        public long CriteriaId { get; set; }
        public string Criteria { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public decimal? TargetScore { get; set; }
        public long? PODetailId { get; set; }
        public bool IsDelete { get; set; }
        public long RetrievedCompKPIId { get; set; }
        public decimal Score { get; set; } // Alias for TargetScore in some contexts
    }

    public class InitIndividualObjectiveRequest
    {
        public long CompanyId { get; set; }
        public long DepartmentId { get; set; }
        public long JobLevelId { get; set; }
        public long PositionId { get; set; }
        public long ProfileId { get; set; }
        public short EffectiveYear { get; set; }
        public long Id { get; set; }
    }

    public class IndividualObjectiveInitResponse
    {
        public List<KPIDataDto> KPIDataList { get; set; } = new();
        public List<ObjectiveDataDto> ParentObjectiveList { get; set; } = new();
        public List<OrganizationGoalDto2> OtherObjectiveList { get; set; } = new();
        public List<FormFieldDto> FormFieldList { get; set; } = new();
        public bool ShowOtherKPIFields { get; set; }
        public bool WeightComputation { get; set; }
        
        // Standard Objectives
        public List<StandardObjectiveDto> StandardObjectiveList { get; set; } = new();
        public List<POCustomCriteria> StandardCriteriaList { get; set; } = new();
    }

    public class KPIDataDto
    {
        public long DisplayId { get; set; }
        public string DisplayField { get; set; }
        public string DisplayData { get; set; } // Measure Name
        public string DisplaySource { get; set; } // Unit of Measure
        public string Criteria { get; set; }
    }

    public class ObjectiveDataDto
    {
        // Add properties if needed, likely just ID and Name
    }

    public class OrganizationGoalDto2
    {
        // Add properties if needed
    }

    public class FormFieldDto
    {
        public string FormFieldName { get; set; }
        public bool HideTag { get; set; }
        public bool RequiredTag { get; set; }
    }

    public class SubmitPerformanceObjectiveRequest
    {
        public PerformanceObjectiveHeader Data { get; set; }
        public bool IsSaveOnly { get; set; }
    }
    
    public class StandardObjectiveDto
    {
        public string BaseLine { get; set; }
        public string CustomKPI { get; set; }
        public long OrganizationGoalId { get; set; }
        public long KeyPerformanceIndicatorId { get; set; }
        public string KPI { get; set; }
        public long MeasureId { get; set; }
        public string Measure { get; set; }
        public string Objectives { get; set; }
        public string OrgGoalDescription { get; set; }
        public string OrganizationGoal { get; set; }
        public decimal TargetGoal { get; set; }
        public string UnitOfMeasure { get; set; }
        public long PerformanceObjectiveDetailId { get; set; }
        public long PerformanceObjectiveHeaderId { get; set; }
        public long TempRowId { get; set; }
        public bool IsRetrievedFromTemplate { get; set; }
        public short RetrievedType { get; set; }
        public long RetrievedPATemplateId { get; set; }
        public long RetrievedCompKPIId { get; set; }
        public string ParentGoal { get; set; }
        public decimal Weight { get; set; }
    }
    
    public class KPISelectionResponseApi
    {
        public List<KPISelectionCriteria> KPICriterias { get; set; } = new();
        public string KPIObjective { get; set; }
    }

    public class KPISelectionCriteria
    {
        public long CriteriaId { get; set; }
        public string txtCriteria { get; set; }
        public decimal txtMin { get; set; }
        public decimal txtMax { get; set; }
        public decimal txtScore { get; set; }
    }
}
