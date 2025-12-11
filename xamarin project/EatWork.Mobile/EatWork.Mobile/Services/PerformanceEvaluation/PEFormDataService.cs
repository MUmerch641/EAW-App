using Acr.UserDialogs;
using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.Models.FormHolder.PerformanceEvaluation;
using EatWork.Mobile.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using APIM = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class PEFormDataService : IPEFormDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IIndividualObjectiveItemDataService individualObjectiveItemDataService_;
        private readonly StringHelper string_;

        public PEFormDataService()
        {
            genericRepository_ = AppContainer.Resolve<IGenericRepository>();
            commonDataService_ = AppContainer.Resolve<ICommonDataService>();
            individualObjectiveItemDataService_ = AppContainer.Resolve<IIndividualObjectiveItemDataService>();
            string_ = AppContainer.Resolve<StringHelper>();
        }

        public async Task<PEFormHolder> InitForm(long id)
        {
            var holder = new PEFormHolder();

            try
            {
                if (id > 0)
                {
                    var url = await commonDataService_.RetrieveClientUrl();
                    await commonDataService_.HasInternetConnection(url);
                    holder.UserInfo = PreferenceHelper.UserInfo();

                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.PerformanceEvaluation}/{id}"
                    };

                    var response = await genericRepository_.GetAsync<APIM.Responses.GetPerformanceEvaluationResponse>(builder.ToString());

                    if (response.IsSuccess)
                    {
                        #region Header

                        var model = response.PerformanceAppraisalSchedule;

                        holder.EmployeeName = response.EmployeeInfo.FullName;
                        holder.EmployeeNumber = response.EmployeeInfo.EmployeeNo;
                        holder.Position = response.EmployeeInfo.Position;

                        if (response.EmployeeInfo.HireDate.GetValueOrDefault(Constants.NullDate) > Constants.NullDate)
                            holder.HireDate = response.EmployeeInfo.HireDate.GetValueOrDefault().ToString(Constants.DateFormatMMDDYYYY);

                        holder.AppraisalType = model.PATypeTitle;
                        holder.StartDate = model.PeriodCoveredStartDate.GetValueOrDefault();
                        holder.EndDate = model.PeriodCoveredEndDate.GetValueOrDefault();
                        holder.Period = string.Concat($"{holder.StartDate.ToString(Constants.DateFormatMMDDYYYY)} - {holder.EndDate.ToString(Constants.DateFormatMMDDYYYY)}");

                        var evaluator = response.PerformanceAppraisalSchedule.Evaluations.Where(x => x.EvaluatorTypeId == Constants.SelfEvaluation).FirstOrDefault();

                        if (evaluator != null)
                        {
                            holder.EvaluatorName = holder.UserInfo.EmployeeName;
                            holder.Progress = Convert.ToDouble(evaluator.Progress);

                            var enumUrl = new UriBuilder(url)
                            {
                                Path = $"{ApiConstants.GetEnums}EvaluatorType/{evaluator.EvaluatorTypeId}"
                            };

                            var enumResponse = await genericRepository_.GetAsync<APIM.Models.Enums>(enumUrl.ToString());
                            holder.EvaluatorType = enumResponse.DisplayText;
                        }

                        #endregion Header

                        #region Details

                        //get objectives, questionnaires and criterias
                        builder = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.PerformanceEvaluation}/get-apa-details"
                        };

                        var param = new APIM.Requests.GetAPADetailsRequest
                        {
                            Id = id,
                            CompanyId = response.EmployeeInfo.CompanyId.GetValueOrDefault(0),
                            ProfileId = response.EmployeeInfo.ProfileId,
                        };

                        var request = string_.CreateUrl<APIM.Requests.GetAPADetailsRequest>(builder.ToString(), param);

                        var response1 = await genericRepository_.GetAsync<APIM.Models.GetActualPerformanceAppraisalResponse>(request);

                        if (response1 != null)
                        {
                            var fields = response1.ActualPerformanceAppraisalDto;
                            fields.ActualEvaluationModel = fields.ActualEvaluationList.FirstOrDefault(x => x.EvaluationId == evaluator.EvaluationId);

                            holder.Model = fields;
                            holder.ToggleTraining = response1.SuggestedTrainingsToggle;
                            holder.ReviewerName = fields.ActualEvaluationModel.Reviewer;

                            if (!string.IsNullOrWhiteSpace(response1.PEStatusSubmitLookup))
                            {
                                holder.CanSubmit = response1.PEStatusSubmitLookup.Split(',')
                                                      .Where(x => string.Compare(fields.ActualEvaluationModel.EvaluationStatusId.ToString(), x, true) == 0)
                                                      .Count() > 0;
                            }

                            if (!string.IsNullOrWhiteSpace(response1.PEStatusSaveLookup))
                            {
                                holder.CanSave = response1.PEStatusSaveLookup.Split(',')
                                                      .Where(x => string.Compare(fields.ActualEvaluationModel.EvaluationStatusId.ToString(), x, true) == 0)
                                                      .Count() > 0;
                            }

                            holder.ForSubmission = (holder.CanSubmit || holder.CanSave);

                            #region objectives list

                            holder.PODetails = fields.PODetailList;
                            var poDetailList = new List<APIM.Models.PerformanceObjectiveDetailDto>(fields.PODetailList);

                            if (poDetailList != null && poDetailList.Count > 0)
                            {
                                poDetailList.RemoveAll(x => x.IsRetrievedFromTemplate);

                                foreach (var x in poDetailList)
                                {
                                    var baselineIsNumber = decimal.TryParse(x.BaseLine, out decimal baseline);
                                    var baselineVal = !baselineIsNumber ? x.BaseLine : baseline.ToString("N0");

                                    holder.Objectives.Add(new ObjectiveDetailDto()
                                    {
                                        Actual = x.Actual,
                                        BaseLine = baselineVal,
                                        CustomKPI = x.CustomKPI,
                                        EmployeeRating = x.EmployeeRating,
                                        EmployeeReview = x.EmployeeReview,
                                        GoalId = x.OrganizationGoalId,
                                        IsRetrievedFromTemplate = x.IsRetrievedFromTemplate,
                                        KPIId = x.KeyPerformanceIndicatorId,
                                        KPIName = x.KPI,
                                        ManagerActual = x.ManagerActual,
                                        ManagerReview = x.ManagerReview,
                                        ManagerReviewRating = x.ManagerReviewRating,
                                        MeasureId = x.MeasureId,
                                        MeasureName = x.Measure,
                                        ParentObjective = x.ParentGoal,
                                        PODetailId = x.PerformanceObjectiveDetailId,
                                        POHeaderId = x.PerformanceObjectiveHeaderId,
                                        RetrievedCompKPIId = x.RetrievedCompKPIId,
                                        RetrievedType = x.RetrievedType,
                                        Target = x.TargetGoal.ToString("n2"),
                                        TempRowId = x.TempRowId,
                                        UnitOfMeasure = x.UnitOfMeasure,
                                        Weight = x.Weight.ToString("N0"),
                                        RetrievedPATemplateId = x.RetrievedPATemplateId,
                                        ObjectiveHeader = x.ParentGoal,
                                        ObjectiveDetail = x.OrganizationGoal,
                                        ObjectiveDescription = x.Objectives,
                                        ShowLine = true,
                                        TargetGoalSetup = $"{(x.Measure == "Amount" ? (x.UnitOfMeasure + " " + x.TargetGoal.ToString("n2")) : (x.TargetGoal.ToString("n2") + " " + x.UnitOfMeasure))}"
                                        /*TargetGoalSetup = $"{x.TargetGoal.ToString("n2")} {x.UnitOfMeasure}",*/
                                    });
                                }
                            }

                            var groupings = await individualObjectiveItemDataService_.GroupObjectives(holder.Objectives);
                            holder.ObjectivesDisplay = new ObservableCollection<MainObjectiveDto>(groupings.Objectives);

                            /*
                            var grouped = holder.Objectives.GroupBy(g => g.ParentObjective);
                            var firstItem = true;
                            var items = new List<ObjectiveDetailHeaderDto>();

                            foreach (var item in grouped)
                            {
                                var isOpened = false;

                                if (firstItem)
                                {
                                    isOpened = true;
                                    firstItem = false;
                                }

                                var obj = new ObjectiveDetailHeaderDto
                                {
                                    ObjectiveHeader = item.Select(g => g.ParentObjective).FirstOrDefault(),
                                    ObjectiveDetailDto = new ObservableCollection<ObjectiveDetailDto>(item),
                                    IsOpened = isOpened
                                };

                                items.Add(obj);
                            }

                            holder.ObjectivesDisplay = new ObservableCollection<ObjectiveDetailHeaderDto>(items);
                            */

                            #endregion objectives list

                            #region Competency Questionnaire

                            var actualEvaluationModel = fields.ActualEvaluationModel;
                            var patCompetencyCriteriaList = actualEvaluationModel.PATemplateCompetencyList;
                            var patKPICriteriaList = actualEvaluationModel.PATemplateKPIList;
                            var combinedCriteriaList = actualEvaluationModel.CombinedCriteriaList.OrderBy(p => p.SortOrder).ToList();
                            var combinedAnswerList = actualEvaluationModel.CombinedAnswerList.OrderBy(p => p.QuestionnaireSortOrder).ToList();
                            var totalNoOfQuestions = patCompetencyCriteriaList.Count + patKPICriteriaList.Count;

                            holder.CombinedCriteriaList = combinedCriteriaList;
                            holder.PATemplateCompetencyList = patCompetencyCriteriaList;
                            holder.PATemplateKPIList = patKPICriteriaList;
                            holder.CombinedAnswerList = combinedAnswerList;
                            holder.ActualEvaluationList = fields.ActualEvaluationList;

                            var criteria = combinedCriteriaList.FirstOrDefault();

                            if (combinedAnswerList.Count > 0)
                            {
                                holder.CurrentPACombinedAnswerList = combinedAnswerList;

                                foreach (var item in combinedAnswerList.Where(x => !string.IsNullOrWhiteSpace(x.AttachmentName)))
                                {
                                    if (item.QuestionnaireCriteriaTypeId == (short)APIM.Models.CriteriaType.Competency)
                                    {
                                        holder.CompetencyUploadedFileList.Add(new APIM.Models.PAFileUploadParamModel()
                                        {
                                            AttachmentName = item.AttachmentName,
                                            IsChanged = false,
                                            EvaluationId = item.EvaluationId,
                                            QuestionnaireAnswerId = item.QuestionnaireAnswerId,
                                            QuestionnaireCriteriaId = item.QuestionnaireCriteriaId,
                                            QuestionnaireCriteriaTypeId = item.QuestionnaireCriteriaTypeId,
                                            /*AttachmentFile = item.FileUpload,*/
                                        });
                                    }
                                    else
                                    {
                                        holder.KPIUploadedFileList.Add(new APIM.Models.PAFileUploadParamModel()
                                        {
                                            AttachmentName = item.AttachmentName,
                                            IsChanged = false,
                                            EvaluationId = item.EvaluationId,
                                            QuestionnaireAnswerId = item.QuestionnaireAnswerId,
                                            QuestionnaireCriteriaId = item.QuestionnaireCriteriaId,
                                            QuestionnaireCriteriaTypeId = item.QuestionnaireCriteriaTypeId,
                                            /*AttachmentFile = item.FileUpload,*/
                                        });
                                    }
                                }
                            }
                            /*
                            else
                            {
                                foreach (var item in combinedCriteriaList)
                                {
                                    holder.CurrentPACombinedAnswerList.Add(new APIM.Models.CombinedAnswerDto()
                                    {
                                        ActualOutput = 0,
                                        AttachmentName = string.Empty,
                                        EvaluationId = evaluator.EvaluationId,
                                        FileUpload = string.Empty,
                                        PATemplateId = item.PATemplateId,
                                        QuestionnaireAnswerId = item.QuestionnaireAnswerId,
                                        QuestionnaireCriteriaId = item.QuestionnaireCriteriaId,
                                        QuestionnaireCriteriaTypeId = item.CriteriaType,
                                        QuestionnaireSortOrder = item.SortOrder,
                                        RatingScore = item.RatingScore,
                                        Recommendations = item.txtReviewerRecommendations,
                                        Remarks = item.txtReviewerRemarks,
                                        StarRating = 0,
                                        SuggestedTrainings = string.Empty,
                                        txtActual = string.Empty,
                                        txtReviewerRating = 0,
                                        txtReviewerRecommendations = string.Empty,
                                        txtReviewerRemarks = string.Empty,
                                        WeightedScore = 0,
                                    });
                                }
                            }
                            */

                            holder.WeightComputation = fields.WeightComputation;
                            holder.MethodMaximumScorePerItem = (fields.MethodMaximumScorePerItem == 1);
                            holder.MethodMaximumScorePerItemWeight = fields.MethodMaximumScorePerItemWeight;
                            holder.ActualEvaluationModel = actualEvaluationModel;

                            holder = await OnCompetencyChanged(holder, criteria);

                            #endregion Competency Questionnaire

                            #region Narrative Section

                            if (actualEvaluationModel.NSQuestionList.Count > 0)
                            {
                                holder.NSQuestionList = actualEvaluationModel.NSQuestionList;
                                holder.NSAnswerList = actualEvaluationModel.NSAnswerList;

                                holder.QuestionsIds = string.Join(",", actualEvaluationModel.NSQuestionList.Where(p => !p.IsHidden).Select(p => p.QuestionId));
                                var firstQuestion = true;
                                var ctr = 1;

                                /*foreach (var item in actualEvaluationModel.NSQuestionList.OrderBy(x => x.txtQuestionOrder))*/
                                foreach (var item in actualEvaluationModel.NSQuestionList.OrderBy(x => x.txtQuestionOrder).Where(x => !x.IsHidden))
                                {
                                    if (!item.IsHidden && ctr == 1)
                                    {
                                        firstQuestion = true;
                                        ctr++;
                                    }

                                    var answered = new APIM.Models.NarrativeSectionModel();

                                    if (item.IsRequired)
                                        holder.HasError = true;

                                    if (actualEvaluationModel.NSAnswerList.Count > 0)
                                        answered = actualEvaluationModel.NSAnswerList.Where(x => x.QuestionId == item.QuestionId).FirstOrDefault();

                                    holder.Narratives.Add(new NarrativeSectionDto()
                                    {
                                        AccordionId = $"accordion{item.QuestionId}",
                                        Id = $"txtAnswer{item.QuestionId}",
                                        NarrativeSectionId = $"NarrativeSectionId{item.QuestionId}",
                                        IsOpened = firstQuestion,
                                        IsHidden = item.IsHidden,
                                        IsRequired = item.IsRequired,
                                        QuestionId = item.QuestionId,
                                        EvaluationId = evaluator.EvaluationId,
                                        Question = HttpUtility.HtmlDecode(item.txtQuestions),
                                        Answer = (answered != null ? HttpUtility.HtmlDecode(answered.Answer) : string.Empty),
                                        ActualNarrativeSectionId = (answered != null ? answered.NarrativeSectionId : 0),
                                        SortOrder = item.txtQuestionOrder,
                                        IsEnabled = (holder.ForSubmission)
                                    });

                                    firstQuestion = false;
                                }
                            }

                            #endregion Narrative Section
                        }

                        #endregion Details
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " - " + ex.Message}");
                throw ex;
            }

            return holder;
        }

        public async Task<PEFormHolder> OnCompetencyChanged(PEFormHolder holder, APIM.Models.CombinedCriteriaDto criteria)
        {
            try
            {
                holder.StarRatingValue = 0;
                holder.WeightedScore = string.Empty;
                holder.QuestionnaireRemarks.Value = string.Empty;
                holder.QuestionnaireRecommendations.Value = string.Empty;
                holder.UploadedFilesDisplay = new ObservableCollection<PEUploadedFilesDto>();
                holder.SeparatedCriterias = new ObservableCollection<SeparatedCriteriaDto>();
                holder.CompQuestions = new ObservableCollection<string>();

                holder.SelectedTrainings = new ObservableCollection<object>();

                long criteriaId = 0;

                if (criteria != null)
                {
                    holder.CriteriaTypeId = criteria.CriteriaType;
                    holder.CurrentCombinedCriteria = criteria;

                    if (criteria.CriteriaType == (short)APIM.Models.CriteriaType.Competency)
                    {
                        var competency = holder.PATemplateCompetencyList.Where(p => p.CompetencyId == criteria.PrimaryId).FirstOrDefault();
                        holder.Competency = competency.Competency;
                        holder.CompetencyDescription = competency.CompetencyDescription;
                        holder.TargetRating = competency.TargetRatingValue.ToString("n2");
                        criteriaId = competency.CriteriaId;

                        if (!string.IsNullOrWhiteSpace(competency.Criteria))
                        {
                            var criteriaList = competency.Criteria;
                            string[] criterias = criteriaList.Split('|');
                            foreach (var item in criterias)
                            {
                                string[] values = item.Split('`');
                                holder.SeparatedCriterias.Add(new SeparatedCriteriaDto()
                                {
                                    Header = HttpUtility.HtmlDecode(values[0]),
                                    Detail = HttpUtility.HtmlDecode(values[1])
                                });
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(competency.Questions))
                        {
                            string[] values = competency.Questions.Split(',');
                            foreach (var item in values)
                            {
                                holder.CompQuestions.Add(item);
                            }
                        }

                        #region violations

                        holder = await GetViolations(holder, competency.CompetencyId);

                        #endregion violations

                        #region jotter

                        holder = await GetJotters(holder, competency.CompetencyId);

                        #endregion jotter

                        holder.EmptyMessage = "No items available to display.";
                        holder.HasTargetRating = true;

                        holder.CriteriaRatingValues = new List<Models.DataObjects.ComboBoxObject>();
                        var ratingSetup = competency.CriteriaRatingValues.Split('|');
                        var tempId = 1;
                        foreach (var item in ratingSetup)
                        {
                            holder.CriteriaRatingValues.Add(new Models.DataObjects.ComboBoxObject()
                            {
                                Id = tempId,
                                Value = item,
                            });

                            tempId++;
                        }

                        holder.CriteriaRatingValues = holder.CriteriaRatingValues.OrderBy(x => x.Value).ToList();

                        holder.RatingItemCount = (ratingSetup.Count());
                        holder.MaxRatingValue = competency.MaxRatingValue;

                        holder.WeightRating = $"{competency.txtCompetencyWeight.ToString("n")}%";
                        holder.ActualWeightRating = competency.txtCompetencyWeight;
                        holder.DisplayStarsById = (competency.DisplayStarsById != DisplayStarsBy.NONE);

                        #region get uploaded files

                        /*
                        var files = holder.CompetencyUploadedFileList.Where(x => x.QuestionnaireCriteriaId == criteria.PrimaryId);

                        if (files != null && files.Count() > 0)
                        {
                            holder.UploadedFilesDisplay = new ObservableCollection<PEUploadedFilesDto>(
                                    files.Select(x => new PEUploadedFilesDto()
                                    {
                                        FileName = x.AttachmentName,
                                        HeaderId = x.QuestionnaireCriteriaId,
                                    })
                                );
                        }
                        */

                        var file = holder.CompetencyUploadedFileList.Where(x => x.QuestionnaireCriteriaId == criteria.PrimaryId).FirstOrDefault();
                        if (file != null)
                        {
                            var ans = holder.CurrentPACombinedAnswerList.Where(x => x.QuestionnaireCriteriaId == criteria.PrimaryId && x.QuestionnaireCriteriaTypeId == criteria.CriteriaType).FirstOrDefault();
                            holder.UploadedFilesDisplay = new ObservableCollection<PEUploadedFilesDto>()
                        {
                             new PEUploadedFilesDto(){ FileName = file.AttachmentName, HeaderId = file.QuestionnaireAnswerId, FileUpload = (ans != null ? ans.FileUpload : string.Empty)}
                        };
                        }

                        #endregion get uploaded files
                    }
                    else
                    {
                        holder.KpiCriteriaLookup = new List<KPICriteriaDto>();

                        var crteria = holder.PATemplateKPIList.Where(x => x.KeyPerformanceIndicatorId == criteria.PrimaryId).FirstOrDefault();

                        holder.Competency = crteria.KPICode;
                        holder.CompetencyDescription = crteria.KPIDescription;
                        criteriaId = crteria.CriteriaId;

                        if (!string.IsNullOrWhiteSpace(crteria.Criteria))
                        {
                            var criteriaList = crteria.Criteria;
                            string[] criterias = criteriaList.Split('|');
                            foreach (var item in criterias)
                            {
                                string[] values = item.Split('`');
                                holder.SeparatedCriterias.Add(new SeparatedCriteriaDto()
                                {
                                    Header = HttpUtility.HtmlDecode(values[0]),
                                    Detail = HttpUtility.HtmlDecode(values[1])
                                });
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(crteria.CriteriaRatingValues))
                        {
                            var list = crteria.CriteriaRatingValues.Split(',');

                            foreach (var item in list)
                            {
                                var split1 = item.Split('|');
                                var split2 = split1[0].Split('-');

                                var min = float.Parse(split2[0]);
                                var max = float.Parse(split2[1]);
                                var score = float.Parse(split1[1]);

                                holder.KpiCriteriaLookup.Add(new KPICriteriaDto()
                                {
                                    Min = min,
                                    Max = max,
                                    Score = score,
                                });
                            }
                        }

                        holder.Violations = new ObservableCollection<ViolationDto>();
                        holder.JotterSource = new ObservableCollection<JotterDto>();
                        holder.EmptyMessage = "Not applicable.";
                        holder.HasTargetRating = false;
                        holder.WeightRating = $"{crteria.txtKPIWeight.ToString("n")} %";
                        holder.DisplayStarsById = false;

                        #region get uploaded files

                        /*
                        var files = holder.KPIUploadedFileList.Where(x => x.QuestionnaireCriteriaId == criteria.PrimaryId);

                        if (files != null && files.Count() > 0)
                        {
                            holder.UploadedFilesDisplay = new ObservableCollection<PEUploadedFilesDto>(
                                    files.Select(x => new PEUploadedFilesDto()
                                    {
                                        FileName = x.AttachmentName,
                                        HeaderId = x.QuestionnaireCriteriaId,
                                    })
                                );
                        }
                        */
                        var file = holder.KPIUploadedFileList.Where(x => x.QuestionnaireCriteriaId == criteria.PrimaryId).FirstOrDefault();
                        if (file != null)
                        {
                            holder.UploadedFilesDisplay = new ObservableCollection<PEUploadedFilesDto>()
                            {
                                 new PEUploadedFilesDto(){ FileName = file.AttachmentName, HeaderId = file.QuestionnaireAnswerId,}
                            };
                        }

                        #endregion get uploaded files
                    }

                    #region get trainings

                    if (holder.ToggleTraining)
                    {
                        var url = await commonDataService_.RetrieveClientUrl();

                        var TRAININGS_BUILDER = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.PerformanceEvaluation}/{criteriaId}/get-trainings"
                        };

                        var TRAININGS_RESPONSE = await genericRepository_.GetAsync<List<APIM.Models.DataModelDto>>(TRAININGS_BUILDER.ToString());

                        if (TRAININGS_RESPONSE.Count > 0)
                        {
                            holder.TrainingSource = new ObservableCollection<Models.DataObjects.SelectableListModel>(
                                    TRAININGS_RESPONSE.Select(x => new Models.DataObjects.SelectableListModel()
                                    {
                                        Id = x.DisplayId,
                                        DisplayText = x.DisplayField
                                    })
                                );
                        }
                    }

                    #endregion get trainings

                    #region set answer

                    var starId = 0;
                    float weightedScore = 0;
                    var remarks = "";
                    var recommendations = "";
                    var answer = new APIM.Models.CombinedAnswerDto();
                    decimal actualOutput = 0;
                    decimal starRating = 0;

                    answer = holder.CurrentPACombinedAnswerList.Where(x => x.QuestionnaireCriteriaId == criteria.PrimaryId && x.QuestionnaireCriteriaTypeId == criteria.CriteriaType).FirstOrDefault();

                    /*
                    var current = holder.CurrentPACombinedAnswerList.Where(x => x.QuestionnaireCriteriaId == criteria.PrimaryId && x.QuestionnaireCriteriaTypeId == criteria.CriteriaType).FirstOrDefault();
                    var answered = holder.CombinedAnswerList.Where(x => x.QuestionnaireCriteriaId == criteria.PrimaryId && x.QuestionnaireCriteriaTypeId == criteria.CriteriaType).FirstOrDefault();

                    if (current != null)
                        answer = current;
                    else
                        answer = answered;
                    */

                    if (answer != null)
                    {
                        ComboBoxObject starRatingHint = new ComboBoxObject();
                        if (answer.QuestionnaireAnswerId > 0)
                        {
                            starRatingHint = holder.CriteriaRatingValues.FirstOrDefault(x => Convert.ToDecimal(x.Id) == answer.StarRating);
                            /*if (starRatingHint != null)*/
                            starId = Convert.ToInt32(answer.StarRating);
                            answer.RatingScore = Convert.ToDecimal(starRatingHint.Value);
                        }
                        else
                        {
                            starRatingHint = holder.CriteriaRatingValues.FirstOrDefault(x => Convert.ToDecimal(x.Value) == answer.RatingScore);
                            if (starRatingHint != null)
                                starId = Convert.ToInt32(starRatingHint.Id);
                        }

                        var _methodMaximumScorePerItemWeight = holder.MethodMaximumScorePerItemWeight.ToString();
                        var _weightRating = holder.ActualWeightRating.ToString();

                        var ws = float.Parse(starRatingHint.Value) * float.Parse(_weightRating) / 100;
                        /*var ws = float.Parse(answer.RatingScore.ToString()) * float.Parse(_weightRating) / 100;*/
                        var weightPercentage = float.Parse(_weightRating) / 100;

                        if (holder.MethodMaximumScorePerItem)
                            ws = (float.Parse(answer.RatingScore.ToString()) / float.Parse(_methodMaximumScorePerItemWeight) * weightPercentage) * 100;
                        else
                            ws = (float.Parse(answer.RatingScore.ToString()) * weightPercentage);

                        weightedScore = ws;
                        remarks = answer.Remarks;
                        recommendations = answer.Recommendations;
                        actualOutput = answer.ActualOutput;
                        starRating = answer.StarRating;

                        if (!string.IsNullOrWhiteSpace(answer.SuggestedTrainings))
                        {
                            /*var trainings = answer.SuggestedTrainings.Split(',').ToList();*/
                            var trainings = new ObservableCollection<SelectableListModel>();
                            foreach (var score in answer.SuggestedTrainings.Split(','))
                            {
                                var src = holder.TrainingSource.FirstOrDefault(x => x.Id == Convert.ToInt64(score));
                                trainings.Add(new SelectableListModel() { Id = Convert.ToInt64(score), DisplayText = src.DisplayText });
                            }

                            holder.SelectedTrainings = new ObservableCollection<object>(trainings);
                        }
                    }

                    holder.StarRatingValue = starId;
                    holder.WeightedScore = weightedScore.ToString("n2");
                    holder.QuestionnaireRemarks.Value = remarks;
                    holder.QuestionnaireRecommendations.Value = recommendations;
                    holder.KPIOutput.Value = actualOutput.ToString("n2");
                    holder.KPIRating.Value = starRating.ToString("n2");

                    #endregion set answer
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " - " + ex.Message}");
                throw ex;
            }

            return await Task.FromResult(holder);
        }

        public async Task<PEFormHolder> GetViolations(PEFormHolder holder, long competencyId)
        {
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                holder.Violations = new ObservableCollection<ViolationDto>();

                var VIOLATIONS_BUILDER = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.PerformanceEvaluation}/get-violations"
                };

                var VIOLATIONS_PARAM = new APIM.Requests.RetrievePAViolationsRequest()
                {
                    ProfileId = holder.UserInfo.ProfileId,
                    CompetencyId = competencyId,
                    StartDate = holder.StartDate,
                    EndDate = holder.EndDate,
                };

                var URL_VIOLATIONS = string_.CreateUrl<APIM.Requests.RetrievePAViolationsRequest>(VIOLATIONS_BUILDER.ToString(), VIOLATIONS_PARAM);

                var VIOLATIONS_RESPONSE = await genericRepository_.GetAsync<List<APIM.Models.PerformanceAppraisalViolationList>>(URL_VIOLATIONS);

                if (VIOLATIONS_RESPONSE.Count > 0)
                {
                    holder.Violations = new ObservableCollection<ViolationDto>(
                            VIOLATIONS_RESPONSE.Select(x => new ViolationDto()
                            {
                                OffenseCount = x.OffenseCount,
                                ViolationDate = x.ViolationDate,
                                ViolationDate_String = x.ViolationDate.ToString(Constants.DateFormatMMDDYYYY),
                                ViolationDetail = x.Violation,
                                ViolationNo = x.ViolationNo,
                            })
                        );
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " - " + ex.Message}");
                throw ex;
            }

            return holder;
        }

        public async Task<PEFormHolder> GetJotters(PEFormHolder holder, long competencyId)
        {
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();

                holder.JotterSource = new ObservableCollection<JotterDto>();

                var JOTTER_BUILDER = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.PerformanceEvaluation}/get-jotters"
                };

                var JOTTER_PARAM = new APIM.Requests.RetrieveJotterRequest()
                {
                    ProfileId = holder.UserInfo.ProfileId,
                    CompetencyId = competencyId,
                };

                var URL_JOTTER = string_.CreateUrl<APIM.Requests.RetrieveJotterRequest>(JOTTER_BUILDER.ToString(), JOTTER_PARAM);

                var JOTTER_RESPONSE = await genericRepository_.GetAsync<List<APIM.Models.PerformanceAppraisalJotterList>>(URL_JOTTER);

                if (JOTTER_RESPONSE.Count > 0)
                {
                    holder.JotterSource = new ObservableCollection<JotterDto>(
                            JOTTER_RESPONSE.Select(x => new JotterDto()
                            {
                                Message = x.Notes,
                                RecordId = x.JotterReviewId,
                                ReportedBy = x.SubmitterWithDetail,
                                TimeStamp = x.CreateDate,
                                ConnecterOne = x.ConnectorOne,
                                ConnectorTwo = x.ConnectorTwo,
                                Icon = x.MobileIcon,
                                TimeStamp_String = x.CreateDate.ToString("ddd, MM/dd/yyyy hh:mm tt"),
                            })
                        );
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " - " + ex.Message}");
                throw ex;
            }

            return holder;
        }

        public async Task<PEFormHolder> CollectQuestionAnswer(PEFormHolder holder)
        {
            try
            {
                var criteria = holder.CurrentCombinedCriteria;

                var answered = holder.CombinedAnswerList.Where(x => x.QuestionnaireCriteriaId == criteria.PrimaryId && x.QuestionnaireCriteriaTypeId == criteria.CriteriaType).FirstOrDefault();

                var rating = (criteria.CriteriaType == (short)APIM.Models.CriteriaType.Competency && holder.DisplayStarsById ? holder.StarRatingHint : holder.KPIRating.Value);

                decimal.TryParse(rating, out decimal actualRating);

                /*var weightedScore = Convert.ToDecimal(holder.WeightedScore);*/
                var weightedScore = (actualRating > 0 ? (actualRating * holder.ActualWeightRating / 100) : 0);

                var trainings = holder.SelectedTrainings.ToList().Cast<SelectableListModel>();

                /*var suggestedTrainings = (answered != null ? answered.SuggestedTrainings : (trainings.Count() > 0 ? string.Join(",", trainings.Select(x => x.Id)) : string.Empty));*/
                var suggestedTrainings = (trainings.Count() > 0 ? string.Join(",", trainings.Select(x => x.Id)) : string.Empty);

                var attachmentName = (answered != null ? answered.AttachmentName : string.Empty);
                var fileUpload = (answered != null ? answered.FileUpload : string.Empty);

                var actualOutput = (criteria.CriteriaType == (short)APIM.Models.CriteriaType.KPI ? Convert.ToDecimal(holder.KPIOutput.Value) : 0);

                var item = new APIM.Models.CombinedAnswerDto()
                {
                    ActualOutput = actualOutput,
                    AttachmentName = attachmentName,
                    EvaluationId = (answered != null ? answered.EvaluationId : holder.ActualEvaluationModel.EvaluationId),
                    FileUpload = fileUpload,
                    PATemplateId = (answered != null ? answered.PATemplateId : holder.ActualEvaluationModel.PATemplateId),
                    QuestionnaireAnswerId = (answered != null ? answered.QuestionnaireAnswerId : 0),
                    QuestionnaireCriteriaId = (answered != null ? answered.QuestionnaireCriteriaId : criteria.PrimaryId),
                    QuestionnaireCriteriaTypeId = (answered != null ? answered.QuestionnaireCriteriaTypeId : criteria.CriteriaType),
                    QuestionnaireSortOrder = (answered != null ? answered.QuestionnaireSortOrder : criteria.SortOrder),
                    Recommendations = holder.QuestionnaireRecommendations.Value,
                    Remarks = holder.QuestionnaireRemarks.Value,
                    SuggestedTrainings = suggestedTrainings,
                    WeightedScore = weightedScore,
                    /*StarRating = Convert.ToDecimal(holder.KPIRating.Value),*/
                    StarRating = holder.StarRatingValue,
                    txtReviewerRating = 0,
                    txtActual = string.Empty,
                    txtReviewerRecommendations = string.Empty,
                    txtReviewerRemarks = string.Empty,
                    RatingScore = actualRating,
                };

                var exst = holder.CurrentPACombinedAnswerList.Where(x => x.QuestionnaireCriteriaId == criteria.PrimaryId && x.QuestionnaireCriteriaTypeId == criteria.CriteriaType).FirstOrDefault();

                if (exst != null)
                    holder.CurrentPACombinedAnswerList.Remove(exst);

                holder.CurrentPACombinedAnswerList.Add(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " - " + ex.Message}");
                throw ex;
            }

            return await Task.FromResult(holder);
        }

        public async Task<PEFormHolder> SavePODetails(PEFormHolder holder)
        {
            try
            {
                using (UserDialogs.Instance.Loading())
                {
                    await Task.Delay(500);

                    var url = await commonDataService_.RetrieveClientUrl();
                    await commonDataService_.HasInternetConnection(url);

                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.PerformanceEvaluation}/save-po-details"
                    };
                    /*
                    var list = new List<APIM.Models.PerformanceObjectiveDetailDto>(
                            holder.Objectives.Select(x => new APIM.Models.PerformanceObjectiveDetailDto()
                            {
                                Actual = x.Actual,
                                BaseLine = x.BaseLine,
                                Criteria = string.Empty, //for verification
                                CustomKPI = x.CustomKPI,
                                EmployeeRating = x.EmployeeRating,
                                EmployeeReview = x.EmployeeReview,
                                IsRetrievedFromTemplate = x.IsRetrievedFromTemplate,
                                KeyPerformanceIndicatorId = x.KPIId,
                                KPI = x.CustomKPI,
                                ManagerActual = x.ManagerActual,
                                ManagerReview = x.ManagerReview,
                                ManagerReviewRating = x.ManagerReviewRating,
                                ManagerReviewRemarks = x.ManagerReview, //for verification
                                Measure = x.MeasureName, //for verification
                                MeasureId = x.MeasureId,
                                Objectives = x.ObjectiveDescription,
                                OrganizationGoal = x.ObjectiveDetail,
                                OrganizationGoalId = x.GoalId,
                                PerformanceObjectiveDetailId = x.PODetailId,
                                OrgGoalDescription = string.Empty, //for verification
                                ParentGoal = x.ParentObjective,
                                PerformanceObjectiveHeaderId = x.POHeaderId,
                                Rating = x.EmployeeRating,
                                RetrievedType = x.RetrievedType,
                                TargetGoal = Convert.ToDecimal(x.Target),
                                TempRowId = x.TempRowId,
                                RetrievedPATemplateId = x.RetrievedPATemplateId,
                                txtUnitOfMeasure = x.UnitOfMeasure,
                                UnitOfMeasure = x.UnitOfMeasure,
                                Weight = Convert.ToDecimal(x.Weight),
                                WeightedRating = 0, //for verification
                                RetrievedCompKPIId = x.RetrievedCompKPIId,
                            })
                        );
                    */
                    var param = new APIM.Requests.SubmitPORequest()
                    {
                        POHeaderId = holder.Model.POHeaderId,
                        StatusId = holder.Model.POHeaderStatusId,
                        PODetailList = holder.PODetails,
                        ApproverId = 0,
                    };

                    var response = await genericRepository_.PostAsync<APIM.Requests.SubmitPORequest, APIM.Responses.SubmitPOResponse>(builder.ToString(), param);

                    if (response.IsSuccess)
                    {
                        holder.IsSuccess = true;
                        holder.PODetails = response.PODetailList;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " - " + ex.Message}");
                throw ex;
            }

            return holder;
        }

        public async Task<PEFormHolder> SavePAQuestionnaire(PEFormHolder holder)
        {
            try
            {
                using (UserDialogs.Instance.Loading())
                {
                    await Task.Delay(500);
                    var url = await commonDataService_.RetrieveClientUrl();
                    await commonDataService_.HasInternetConnection(url);

                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.PerformanceEvaluation}/save-pa-questionnaire"
                    };

                    var questionCount = holder.CombinedCriteriaList.Count() + holder.NSQuestionList.Count(p => p.IsRequired && !p.IsHidden);
                    var answerCount = holder.CurrentPACombinedAnswerList.Count(x => x.Remarks != string.Empty && x.Recommendations != string.Empty);

                    var combinedAnswer = holder.CurrentPACombinedAnswerList;
                    var nsAnswerList = holder.NSAnswerList;
                    var uploadedCompFiles = holder.CompetencyUploadedFileList;
                    var uploadedKpiFiles = holder.KPIUploadedFileList;

                    var param = new APIM.Requests.SubmitPAQuestionnaireRequest()
                    {
                        CombineAnswerList = combinedAnswer,
                        CompetencyUploadedFileList = uploadedCompFiles,
                        KPIUploadedFileList = uploadedKpiFiles,
                        QuestionCount = questionCount,
                        AnswerCount = answerCount,
                        NSQuestionList = holder.NSQuestionList,
                        NSAnswerList = nsAnswerList,
                    };

                    var response = await genericRepository_.PostAsync<APIM.Requests.SubmitPAQuestionnaireRequest, APIM.Responses.SubmitPAQuestionnaireResponse>(builder.ToString(), param);

                    if (response.IsSuccess)
                    {
                        holder.Progress = response.EvaluationProgress;
                        holder.CurrentPACombinedAnswerList = response.CombinedAnswerList;
                        holder.CombinedAnswerList = response.CombinedAnswerList;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " - " + ex.Message}");
                throw ex;
            }

            return holder;
        }

        public async Task<PEFormHolder> SavePANarrativeSection(PEFormHolder holder)
        {
            holder.IsSuccess = false;

            try
            {
                using (UserDialogs.Instance.Loading())
                {
                    await Task.Delay(500);

                    var url = await commonDataService_.RetrieveClientUrl();
                    await commonDataService_.HasInternetConnection(url);

                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.PerformanceEvaluation}/save-pa-narrativesection"
                    };

                    var questionCount = holder.CombinedCriteriaList.Count() + holder.NSQuestionList.Count(p => p.IsRequired && !p.IsHidden);
                    var answerCount = holder.CurrentPACombinedAnswerList.Count(x => x.Remarks != string.Empty && x.Recommendations != string.Empty);

                    var answers = new List<APIM.Models.NarrativeSectionModel>(
                            holder.Narratives.Where(x => !x.IsHidden).Select(x => new APIM.Models.NarrativeSectionModel()
                            {
                                Answer = x.Answer,
                                EvaluationId = x.EvaluationId,
                                NarrativeSectionId = x.ActualNarrativeSectionId,
                                QuestionId = x.QuestionId,
                            })
                        );

                    var param = new APIM.Requests.SubmitPANarrativeSectionRequest()
                    {
                        QuestionCount = questionCount,
                        AnswerCount = answerCount,
                        NSQuestionList = holder.NSQuestionList,
                        NSAnswerList = holder.NSAnswerList,
                        PANSAnswerList = answers,
                    };

                    var response = await genericRepository_.PostAsync<APIM.Requests.SubmitPANarrativeSectionRequest, APIM.Responses.SubmitPANarrativeSectionResponse>(builder.ToString(), param);

                    if (response.IsSuccess)
                    {
                        holder.Progress = response.Progress;
                        holder.IsSuccess = true;
                        holder.ForSubmission = (response.StatusId == RequestStatusValue.Completed);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " - " + ex.Message}");
                throw ex;
            }

            return holder;
        }

        public async Task<PEFormHolder> ValidatePANarrativeSection(PEFormHolder holder)
        {
            var error = new List<int>();
            try
            {
                foreach (var item in holder.Narratives.ToList().OrderBy(x => x.SortOrder))
                {
                    item.HasError = false;

                    if (item.IsRequired && string.IsNullOrWhiteSpace(item.Answer))
                    {
                        item.HasError = true;
                        error.Add(1);
                    }

                    var exist = holder.Narratives.FirstOrDefault(x => x.QuestionId == item.QuestionId);

                    holder.Narratives.Remove(exist);
                    holder.Narratives.Add(item);
                }

                holder.HasError = (error.Count() > 1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " - " + ex.Message}");
                throw ex;
            }

            return await Task.FromResult(holder);
        }

        public async Task<PEFormHolder> ManageUploadFile(Models.DataObjects.FileUploadResponse file, PEFormHolder holder, bool isDelete = false)
        {
            try
            {
                var criteria = holder.CurrentCombinedCriteria;
                var answered = holder.CombinedAnswerList.Where(x => x.QuestionnaireCriteriaId == criteria.PrimaryId && x.QuestionnaireCriteriaTypeId == criteria.CriteriaType).FirstOrDefault();

                if (criteria.CriteriaType == (short)APIM.Models.CriteriaType.Competency)
                {
                    if (!isDelete)
                    {
                        var exst = holder.CompetencyUploadedFileList.Where(x => x.QuestionnaireCriteriaId == criteria.PrimaryId).FirstOrDefault(); ;
                        if (exst != null)
                            holder.CompetencyUploadedFileList.Remove(exst);

                        holder.CompetencyUploadedFileList.Add(new APIM.Models.PAFileUploadParamModel()
                        {
                            QuestionnaireCriteriaTypeId = criteria.CriteriaType,
                            QuestionnaireCriteriaId = (answered != null ? answered.QuestionnaireCriteriaId : criteria.PrimaryId),
                            QuestionnaireAnswerId = (answered != null ? answered.QuestionnaireAnswerId : 0),
                            EvaluationId = (answered != null ? answered.EvaluationId : holder.ActualEvaluationModel.EvaluationId),
                            AttachmentName = file.FileName,
                            AttachmentFile = Convert.ToBase64String(file.FileDataArray),
                            IsChanged = true,
                        });
                    }
                }
                else
                {
                    if (!isDelete)
                    {
                        var exst = holder.KPIUploadedFileList.Where(x => x.QuestionnaireCriteriaId == criteria.PrimaryId).FirstOrDefault(); ;
                        if (exst != null)
                            holder.KPIUploadedFileList.Remove(exst);

                        holder.KPIUploadedFileList.Add(new APIM.Models.PAFileUploadParamModel()
                        {
                            QuestionnaireCriteriaTypeId = criteria.CriteriaType,
                            QuestionnaireCriteriaId = (answered != null ? answered.QuestionnaireCriteriaId : 0),
                            QuestionnaireAnswerId = (answered != null ? answered.QuestionnaireAnswerId : 0),
                            EvaluationId = (answered != null ? answered.EvaluationId : holder.ActualEvaluationModel.EvaluationId),
                            AttachmentName = file.FileName,
                            AttachmentFile = Convert.ToBase64String(file.FileDataArray),
                            IsChanged = false,
                        });
                    }
                }

                if (isDelete)
                {
                    var exist = holder.UploadedFilesDisplay.FirstOrDefault(x => x.FileName == file.FileName);
                    holder.UploadedFilesDisplay.Remove(exist);

                    var currentAnswer = holder.CurrentPACombinedAnswerList.Where(x => x.QuestionnaireCriteriaId == criteria.PrimaryId && x.QuestionnaireCriteriaTypeId == criteria.CriteriaType).FirstOrDefault();
                    currentAnswer.FileUpload = string.Empty;
                    currentAnswer.AttachmentName = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} - {ex.Message}");
                throw ex;
            }

            return await Task.FromResult(holder);
        }

        public async Task<string> ViewAttachment(string fileUpload, string fileName, string fileType)
        {
            var retVal = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(fileUpload))
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);

                        var url = await commonDataService_.RetrieveClientUrl();
                        await commonDataService_.HasInternetConnection(url);

                        var builder = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.PerformanceEvaluation}/view-attachment"
                        };

                        var ATTACHMENT_PARAM = new APIM.Requests.ViewAttachmentFileRequest()
                        {
                            FileUpload = fileUpload,
                        };

                        var ATTACHMENT_URL = string_.CreateUrl<APIM.Requests.ViewAttachmentFileRequest>(builder.ToString(), ATTACHMENT_PARAM);

                        var ATTACHMENT_RESPONSE = await genericRepository_.GetAsync<string>(ATTACHMENT_URL.ToString());

                        if (string.IsNullOrWhiteSpace(ATTACHMENT_RESPONSE))
                            throw new Exception("Unable to view file.");

                        retVal = ATTACHMENT_RESPONSE;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " - " + ex.Message}");
                throw ex;
            }

            return retVal;
        }

        public async Task<PEFormHolder> SubmitPerformanceEvaluation(PEFormHolder holder)
        {
            try
            {
                using (UserDialogs.Instance.Loading())
                {
                    await Task.Delay(500);

                    var url = await commonDataService_.RetrieveClientUrl();
                    await commonDataService_.HasInternetConnection(url);

                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.PerformanceEvaluation}/pa-workflow"
                    };

                    var param = new APIM.Requests.SubmitPAWorkFlowRequest()
                    {
                        EmployeeName = holder.EmployeeName,
                        EvaluationId = holder.ActualEvaluationModel.EvaluationId,
                        PeriodCovered = holder.Period,
                        Reason = String.Empty,
                        ReviewerName = holder.ReviewerName,
                        StatusId = RequestStatusValue.Submitted /*holder.ActualEvaluationModel.EvaluationStatusId*/ ,
                        Data = String.Empty,
                        ActualEvaluationList = holder.ActualEvaluationList,
                    };

                    var response = await genericRepository_.PostAsync<APIM.Requests.SubmitPAWorkFlowRequest, APIM.Responses.SubmitPAWorkFlowResponse>(builder.ToString(), param);

                    holder.IsSuccess = response.IsSuccess;
                    FormSession.IsSubmitted = response.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw ex;
            }

            return holder;
        }
    }
}