using EatWork.Mobile.Contants;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Validations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using APIM = EAW.API.DataContracts;

namespace EatWork.Mobile.Models.FormHolder.PerformanceEvaluation
{
    public class PEFormHolder : ExtendedBindableObject
    {
        public PEFormHolder()
        {
            Progress = 0;
            Objectives = new ObservableCollection<ObjectiveDetailDto>();
            ObjectivesDisplay = new ObservableCollection<MainObjectiveDto>();
            Violations = new ObservableCollection<ViolationDto>();
            JotterSource = new ObservableCollection<JotterDto>();
            Narratives = new ObservableCollection<NarrativeSectionDto>();
            SeparatedCriterias = new ObservableCollection<SeparatedCriteriaDto>();
            CompQuestions = new ObservableCollection<string>();
            HasError = false;
            Model = new APIM.Models.ActualPerformanceAppraisalDto();
            PATemplateCompetencyList = new List<APIM.Models.PATemplateCompetencyDto>();
            CombinedCriteriaList = new List<APIM.Models.CombinedCriteriaDto>();
            PATemplateKPIList = new List<APIM.Models.PATemplateKPIDto>();
            Page = 1;
            PageSize = 1;
            UserInfo = new UserModel();
            StartDate = Constants.NullDate;
            EndDate = Constants.NullDate;
            HasTargetRating = true;
            EmptyMessage = "No items available to display.";
            RatingItemCount = 5;
            /*RatingItemList = new ObservableCollection<Syncfusion.SfRating.XForms.SfRatingItem>();*/
            IsValidStarRating = true;
            StarRatingValue = 0;
            QuestionnaireRemarks = new ValidatableObject<string>();
            QuestionnaireRecommendations = new ValidatableObject<string>();
            CurrentPage = 1;
            WeightComputation = false;
            CriteriaRatingValues = new List<ComboBoxObject>();
            UploadedFilesDisplay = new ObservableCollection<PEUploadedFilesDto>();
            DisplayStarsById = false;
            KPIOutput = new ValidatableObject<string>();
            KPIRating = new ValidatableObject<string>();
            KpiCriteriaLookup = new List<KPICriteriaDto>();
            CurrentPACombinedAnswerList = new List<APIM.Models.CombinedAnswerDto>();
            ActualEvaluationModel = new APIM.Models.ActualEvaluationDto();
            CurrentCombinedCriteria = new APIM.Models.CombinedCriteriaDto();
            CombinedAnswerList = new List<APIM.Models.CombinedAnswerDto>();
            CombinedAnswer = new APIM.Models.CombinedAnswerDto();
            IsSuccess = false;
            PODetails = new List<APIM.Models.PerformanceObjectiveDetailDto>();
            NSQuestionList = new List<APIM.Models.QuestionListDto>();
            NSAnswerList = new List<APIM.Models.NarrativeSectionModel>();
            CompetencyUploadedFileList = new List<APIM.Models.PAFileUploadParamModel>();
            KPIUploadedFileList = new List<APIM.Models.PAFileUploadParamModel>();
            SelectedFile = new PEUploadedFilesDto();
            TrainingSource = new ObservableCollection<SelectableListModel>();
            SelectedTrainings = new ObservableCollection<object>();
            ActualEvaluationList = new List<APIM.Models.ActualEvaluationDto>();
            ToggleTraining = false;
            ForSubmission = false;
            CanSave = false;
            CanSubmit = false;
        }

        #region HEADER

        private string employeeName_;

        public string EmployeeName
        {
            get { return employeeName_; }
            set { employeeName_ = value; RaisePropertyChanged(() => EmployeeName); }
        }

        private string employeeNumber_;

        public string EmployeeNumber
        {
            get { return employeeNumber_; }
            set { employeeNumber_ = value; RaisePropertyChanged(() => EmployeeNumber); }
        }

        private string position_;

        public string Position
        {
            get { return position_; }
            set { position_ = value; RaisePropertyChanged(() => Position); }
        }

        private string department_;

        public string Department
        {
            get { return department_; }
            set { department_ = value; RaisePropertyChanged(() => Department); }
        }

        private string hireDate_;

        public string HireDate
        {
            get { return hireDate_; }
            set { hireDate_ = value; RaisePropertyChanged(() => HireDate); }
        }

        private string appraisalType_;

        public string AppraisalType
        {
            get { return appraisalType_; }
            set { appraisalType_ = value; RaisePropertyChanged(() => AppraisalType); }
        }

        private string evaluatorType_;

        public string EvaluatorType
        {
            get { return evaluatorType_; }
            set { evaluatorType_ = value; RaisePropertyChanged(() => EvaluatorType); }
        }

        private string evaluatorName_;

        public string EvaluatorName
        {
            get { return evaluatorName_; }
            set { evaluatorName_ = value; RaisePropertyChanged(() => EvaluatorName); }
        }

        private string period_;

        public string Period
        {
            get { return period_; }
            set { period_ = value; RaisePropertyChanged(() => Period); }
        }

        private double progress_;

        public double Progress
        {
            get { return progress_; }
            set { progress_ = value; RaisePropertyChanged(() => Progress); }
        }

        private string status_;

        public string Status
        {
            get { return status_; }
            set { status_ = value; RaisePropertyChanged(() => Status); }
        }

        private UserModel user_;

        public UserModel UserInfo
        {
            get { return user_; }
            set { user_ = value; RaisePropertyChanged(() => UserInfo); }
        }

        private DateTime startDate_;

        public DateTime StartDate
        {
            get { return startDate_; }
            set { startDate_ = value; RaisePropertyChanged(() => StartDate); }
        }

        private DateTime endDate_;

        public DateTime EndDate
        {
            get { return endDate_; }
            set { endDate_ = value; RaisePropertyChanged(() => EndDate); }
        }

        private string reviewerName_;

        public string ReviewerName
        {
            get { return reviewerName_; }
            set { reviewerName_ = value; RaisePropertyChanged(() => ReviewerName); }
        }

        #endregion HEADER

        #region INDIVIDUAL OBJECTIVES

        private ObservableCollection<ObjectiveDetailDto> objectives_;

        public ObservableCollection<ObjectiveDetailDto> Objectives
        {
            get { return objectives_; }
            set { objectives_ = value; RaisePropertyChanged(() => Objectives); }
        }

        private ObservableCollection<MainObjectiveDto> objectivesDisplay_;

        public ObservableCollection<MainObjectiveDto> ObjectivesDisplay
        {
            get { return objectivesDisplay_; }
            set { objectivesDisplay_ = value; RaisePropertyChanged(() => Objectives); }
        }

        /*

        private ObservableCollection<ObjectiveDetailHeaderDto> objectivesDisplay_;

        public ObservableCollection<ObjectiveDetailHeaderDto> ObjectivesDisplay
        {
            get { return objectivesDisplay_; }
            set { objectivesDisplay_ = value; RaisePropertyChanged(() => Objectives); }
        }
        */
        public List<APIM.Models.PerformanceObjectiveDetailDto> PODetails { get; set; }

        #endregion INDIVIDUAL OBJECTIVES

        #region QUESTIONNAIRE

        private ObservableCollection<SeparatedCriteriaDto> separatedCriterias_;

        public ObservableCollection<SeparatedCriteriaDto> SeparatedCriterias
        {
            get { return separatedCriterias_; }
            set { separatedCriterias_ = value; RaisePropertyChanged(() => SeparatedCriterias); }
        }

        private ObservableCollection<string> compQuestions_;

        public ObservableCollection<string> CompQuestions
        {
            get { return compQuestions_; }
            set { compQuestions_ = value; RaisePropertyChanged(() => CompQuestions); }
        }

        private string _competency;

        public string Competency
        {
            get { return _competency; }
            set { _competency = value; RaisePropertyChanged(() => Competency); }
        }

        private string competencyDescription_;

        public string CompetencyDescription
        {
            get { return competencyDescription_; }
            set { competencyDescription_ = value; RaisePropertyChanged(() => CompetencyDescription); }
        }

        private string targetRating_;

        public string TargetRating
        {
            get { return targetRating_; }
            set { targetRating_ = value; RaisePropertyChanged(() => TargetRating); }
        }

        private bool hasTargetRating_;

        public bool HasTargetRating
        {
            get { return hasTargetRating_; }
            set { hasTargetRating_ = value; RaisePropertyChanged(() => HasTargetRating); }
        }

        private ObservableCollection<ViolationDto> violations_;

        public ObservableCollection<ViolationDto> Violations
        {
            get { return violations_; }
            set { violations_ = value; RaisePropertyChanged(() => Violations); }
        }

        private ObservableCollection<JotterDto> jotters_;

        public ObservableCollection<JotterDto> JotterSource
        {
            get { return jotters_; }
            set { jotters_ = value; RaisePropertyChanged(() => JotterSource); }
        }

        private ObservableCollection<PEUploadedFilesDto> uploadedFilesDisplay_;

        public ObservableCollection<PEUploadedFilesDto> UploadedFilesDisplay
        {
            get { return uploadedFilesDisplay_; }
            set { uploadedFilesDisplay_ = value; RaisePropertyChanged(() => UploadedFilesDisplay); }
        }

        private List<APIM.Models.PATemplateCompetencyDto> paTemplateCompetencyList_;

        public List<APIM.Models.PATemplateCompetencyDto> PATemplateCompetencyList
        {
            get { return paTemplateCompetencyList_; }
            set { paTemplateCompetencyList_ = value; RaisePropertyChanged(() => PATemplateCompetencyList); }
        }

        private List<APIM.Models.CombinedCriteriaDto> combinedCriteriaList_;

        public List<APIM.Models.CombinedCriteriaDto> CombinedCriteriaList
        {
            get { return combinedCriteriaList_; }
            set { combinedCriteriaList_ = value; RaisePropertyChanged(() => CombinedCriteriaList); }
        }

        private List<APIM.Models.CombinedAnswerDto> combinedAnswerList_;

        public List<APIM.Models.CombinedAnswerDto> CombinedAnswerList
        {
            get { return combinedAnswerList_; }
            set { combinedAnswerList_ = value; RaisePropertyChanged(() => CombinedAnswerList); }
        }

        private APIM.Models.CombinedAnswerDto combinedAnswer_;

        public APIM.Models.CombinedAnswerDto CombinedAnswer
        {
            get { return combinedAnswer_; }
            set { combinedAnswer_ = value; RaisePropertyChanged(() => CombinedAnswer); }
        }

        public APIM.Models.CombinedCriteriaDto CurrentCombinedCriteria { get; set; }

        private List<APIM.Models.PATemplateKPIDto> paTemplateKPIList_;

        public List<APIM.Models.PATemplateKPIDto> PATemplateKPIList
        {
            get { return paTemplateKPIList_; }
            set { paTemplateKPIList_ = value; RaisePropertyChanged(() => PATemplateKPIList); }
        }

        public List<APIM.Models.ActualEvaluationDto> ActualEvaluationList { get; set; }

        private int pageSize_;

        public int PageSize
        {
            get { return pageSize_; }
            set { pageSize_ = value; RaisePropertyChanged(() => PageSize); }
        }

        private int page_;

        public int Page
        {
            get { return page_; }
            set { page_ = value; RaisePropertyChanged(() => Page); }
        }

        private int currentPage_;

        public int CurrentPage
        {
            get { return currentPage_; }
            set { currentPage_ = value; RaisePropertyChanged(() => CurrentPage); }
        }

        private string emptyMessage_;

        public string EmptyMessage
        {
            get { return emptyMessage_; }
            set { emptyMessage_ = value; RaisePropertyChanged(() => EmptyMessage); }
        }

        private decimal maxRatingValue_;

        public decimal MaxRatingValue
        {
            get { return maxRatingValue_; }
            set { maxRatingValue_ = value; RaisePropertyChanged(() => MaxRatingValue); }
        }

        private int ratingItemCount_;

        public int RatingItemCount
        {
            get { return ratingItemCount_; }
            set { ratingItemCount_ = value; RaisePropertyChanged(() => RatingItemCount); }
        }

        private List<ComboBoxObject> criteriaRatingValues_;

        public List<ComboBoxObject> CriteriaRatingValues
        {
            get { return criteriaRatingValues_; }
            set { criteriaRatingValues_ = value; RaisePropertyChanged(() => CriteriaRatingValues); }
        }

        /*
        private ObservableCollection<Syncfusion.SfRating.XForms.SfRatingItem> ratingItemList_;

        public ObservableCollection<Syncfusion.SfRating.XForms.SfRatingItem> RatingItemList
        {
            get { return ratingItemList_; }
            set { ratingItemList_ = value; RaisePropertyChanged(() => RatingItemList); }
        }
        */

        private bool isValidRatingStar_;

        public bool IsValidStarRating
        {
            get { return isValidRatingStar_; }
            set { isValidRatingStar_ = value; RaisePropertyChanged(() => IsValidStarRating); }
        }

        private int starRatingValue_;

        public int StarRatingValue
        {
            get { return starRatingValue_; }
            set { starRatingValue_ = value; RaisePropertyChanged(() => StarRatingValue); }
        }

        private decimal starRatingActualValue_;

        public decimal StarRatingActualValue
        {
            get { return starRatingActualValue_; }
            set { starRatingActualValue_ = value; RaisePropertyChanged(() => StarRatingActualValue); }
        }

        private ValidatableObject<string> questionnaireRemarks_;

        public ValidatableObject<string> QuestionnaireRemarks
        {
            get { return questionnaireRemarks_; }
            set { questionnaireRemarks_ = value; RaisePropertyChanged(() => QuestionnaireRemarks); }
        }

        private ValidatableObject<string> questionnaireRecommendations_;

        public ValidatableObject<string> QuestionnaireRecommendations
        {
            get { return questionnaireRecommendations_; }
            set { questionnaireRecommendations_ = value; RaisePropertyChanged(() => QuestionnaireRecommendations); }
        }

        private bool weightComputation_;

        public bool WeightComputation
        {
            get { return weightComputation_; }
            set { weightComputation_ = value; RaisePropertyChanged(() => WeightComputation); }
        }

        private string weightRating_;

        public string WeightRating
        {
            get { return weightRating_; }
            set { weightRating_ = value; RaisePropertyChanged(() => WeightRating); }
        }

        private decimal actualWeightRating_;

        public decimal ActualWeightRating
        {
            get { return actualWeightRating_; }
            set { actualWeightRating_ = value; RaisePropertyChanged(() => ActualWeightRating); }
        }

        private bool methodMaximumScorePerItem_;

        public bool MethodMaximumScorePerItem
        {
            get { return methodMaximumScorePerItem_; }
            set { methodMaximumScorePerItem_ = value; RaisePropertyChanged(() => MethodMaximumScorePerItem); }
        }

        private decimal methodMaximumScorePerItemWeight_;

        public decimal MethodMaximumScorePerItemWeight
        {
            get { return methodMaximumScorePerItemWeight_; }
            set { methodMaximumScorePerItemWeight_ = value; RaisePropertyChanged(() => MethodMaximumScorePerItemWeight); }
        }

        private string weightedScore_;

        public string WeightedScore
        {
            get { return weightedScore_; }
            set { weightedScore_ = value; RaisePropertyChanged(() => WeightedScore); }
        }

        private string starRatingHint_;

        public string StarRatingHint
        {
            get { return starRatingHint_; }
            set { starRatingHint_ = value; RaisePropertyChanged(() => StarRatingHint); }
        }

        private bool displayStarsById_;

        public bool DisplayStarsById
        {
            get { return displayStarsById_; }
            set { displayStarsById_ = value; RaisePropertyChanged(() => DisplayStarsById); }
        }

        private ValidatableObject<string> kpiOutput_;

        public ValidatableObject<string> KPIOutput
        {
            get { return kpiOutput_; }
            set { kpiOutput_ = value; RaisePropertyChanged(() => KPIOutput); }
        }

        private ValidatableObject<string> kpiRating_;

        public ValidatableObject<string> KPIRating
        {
            get { return kpiRating_; }
            set { kpiRating_ = value; RaisePropertyChanged(() => KPIRating); }
        }

        private List<KPICriteriaDto> kpiCriteriaLookup_;

        public List<KPICriteriaDto> KpiCriteriaLookup
        {
            get { return kpiCriteriaLookup_; }
            set { kpiCriteriaLookup_ = value; RaisePropertyChanged(() => KpiCriteriaLookup); }
        }

        private short criteriaTypeId_;

        public short CriteriaTypeId
        {
            get { return criteriaTypeId_; }
            set { criteriaTypeId_ = value; RaisePropertyChanged(() => CriteriaTypeId); }
        }

        private List<APIM.Models.CombinedAnswerDto> currentPACombinedAnswerList_;

        public List<APIM.Models.CombinedAnswerDto> CurrentPACombinedAnswerList
        {
            get { return currentPACombinedAnswerList_; }
            set { currentPACombinedAnswerList_ = value; RaisePropertyChanged(() => CurrentPACombinedAnswerList); }
        }

        private APIM.Models.ActualEvaluationDto actualEvaluationModel_;

        public APIM.Models.ActualEvaluationDto ActualEvaluationModel
        {
            get { return actualEvaluationModel_; }
            set { actualEvaluationModel_ = value; RaisePropertyChanged(() => ActualEvaluationModel); }
        }

        public List<APIM.Models.PAFileUploadParamModel> CompetencyUploadedFileList { get; set; }
        public List<APIM.Models.PAFileUploadParamModel> KPIUploadedFileList { get; set; }

        private PEUploadedFilesDto selectedFile_;

        public PEUploadedFilesDto SelectedFile
        {
            get { return selectedFile_; }
            set { selectedFile_ = value; RaisePropertyChanged(() => SelectedFile); }
        }

        private ObservableCollection<SelectableListModel> trainingSource_;

        public ObservableCollection<SelectableListModel> TrainingSource
        {
            get { return trainingSource_; }
            set { trainingSource_ = value; RaisePropertyChanged(() => TrainingSource); }
        }

        private ObservableCollection<object> selectedTrainings_;

        public ObservableCollection<object> SelectedTrainings
        {
            get { return selectedTrainings_; }
            set { selectedTrainings_ = value; RaisePropertyChanged(() => SelectedTrainings); }
        }

        private bool toggleTraining_;

        public bool ToggleTraining
        {
            get { return toggleTraining_; }
            set { toggleTraining_ = value; RaisePropertyChanged(() => ToggleTraining); }
        }

        #endregion QUESTIONNAIRE

        #region NARRATIVE SECTION

        private string questionIds_;

        public string QuestionsIds
        {
            get { return questionIds_; }
            set { questionIds_ = value; RaisePropertyChanged(() => QuestionsIds); }
        }

        private ObservableCollection<NarrativeSectionDto> narratives_;

        public ObservableCollection<NarrativeSectionDto> Narratives
        {
            get { return narratives_; }
            set { narratives_ = value; RaisePropertyChanged(() => Narratives); }
        }

        public List<APIM.Models.QuestionListDto> NSQuestionList { get; set; }

        public List<APIM.Models.NarrativeSectionModel> NSAnswerList { get; set; }

        #endregion NARRATIVE SECTION

        private bool hasError_;

        public bool HasError
        {
            get { return hasError_; }
            set { hasError_ = value; RaisePropertyChanged(() => HasError); }
        }

        private APIM.Models.ActualPerformanceAppraisalDto model_;

        public APIM.Models.ActualPerformanceAppraisalDto Model
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => Model); }
        }

        public bool IsValidQuestionAnswer()
        {
            if (DisplayStarsById)
            {
                if (StarRatingValue > 0)
                    IsValidStarRating = true;
                else
                    IsValidStarRating = false;
            }
            else
            {
                KPIOutput.Validations.Clear();
                KPIOutput.Validations.Add(new IsNotNullOrEmptyRule<string>
                {
                    ValidationMessage = ""
                });

                KPIRating.Validations.Clear();
                KPIRating.Validations.Add(new IsNotNullOrEmptyRule<string>
                {
                    ValidationMessage = ""
                });
            }

            QuestionnaireRemarks.Validations.Clear();
            QuestionnaireRemarks.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            QuestionnaireRecommendations.Validations.Clear();
            QuestionnaireRecommendations.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            QuestionnaireRemarks.Validate();
            QuestionnaireRecommendations.Validate();
            KPIOutput.Validate();
            KPIRating.Validate();

            return IsValidStarRating &&
                   QuestionnaireRemarks.IsValid &&
                   QuestionnaireRecommendations.IsValid &&
                   KPIOutput.IsValid &&
                   KPIRating.IsValid;
        }

        private bool forSubmission_;

        public bool ForSubmission
        {
            get { return forSubmission_; }
            set { forSubmission_ = value; RaisePropertyChanged(() => ForSubmission); }
        }

        private bool canSave_;

        public bool CanSave
        {
            get { return canSave_; }
            set { canSave_ = value; RaisePropertyChanged(() => CanSave); }
        }

        private bool canSubmit_;

        public bool CanSubmit

        {
            get { return canSubmit_; }
            set { canSubmit_ = value; RaisePropertyChanged(() => CanSubmit); }
        }

        private bool isEnabled_;

        public bool IsEnabled
        {
            get { return isEnabled_; }
            set { isEnabled_ = value; RaisePropertyChanged(() => IsEnabled); }
        }

        private bool isSuccess_;

        public bool IsSuccess
        {
            get { return isSuccess_; }
            set { isSuccess_ = value; RaisePropertyChanged(() => IsSuccess); }
        }
    }

    public class RotatorItem
    {
        public string Name { get; set; }
        public View Page { get; set; }
    }

    public class ViolationDto
    {
        public long ViolationId { get; set; }
        public DateTime ViolationDate { get; set; }
        public string ViolationDate_String { get; set; }
        public string ViolationNo { get; set; }
        public int OffenseCount { get; set; }
        public string ViolationDetail { get; set; }
    }

    public class JotterDto
    {
        public long RecordId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string TimeStamp_String { get; set; }
        public string ReportedBy { get; set; }
        public string Message { get; set; }
        public string Icon { get; set; }
        public string ConnecterOne { get; set; }
        public string ConnectorTwo { get; set; }
    }

    public class EvaluationAnswerDto
    {
    }

    public class PEUploadedFilesDto : FileUploadResponse
    {
        public long HeaderId { get; set; }
        public string FileUpload { get; set; }
    }

    public class NarrativeSectionDto
    {
        public NarrativeSectionDto()
        {
            IsOpened = false;
            HasError = false;
        }

        public string NarrativeSectionId { get; set; }
        public long ActualNarrativeSectionId { get; set; }
        public long QuestionId { get; set; }
        public string Question { get; set; }
        public long EvaluationId { get; set; }
        public string Answer { get; set; }
        public bool IsOpened { get; set; }
        public bool IsEnabled { get; set; }

        public string AccordionId { get; set; }
        public string FormAnswer { get; set; }
        public string CollapseFlag { get; set; }
        public bool IsRequired { get; set; }
        public bool IsHidden { get; set; }
        public string Id { get; set; }
        public bool HasError { get; set; }
        public int SortOrder { get; set; }
    }

    public class SeparatedCriteriaDto
    {
        public string Header { get; set; }
        public string Detail { get; set; }
    }

    public class KPICriteriaDto
    {
        public float Min { get; set; }
        public float Max { get; set; }
        public float Score { get; set; }
    }
}