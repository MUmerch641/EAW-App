using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.Models.FormHolder.PerformanceEvaluation;
using EAW.API.DataContracts.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EatWork.Mobile.Services.TestServices
{
    public class PEFormDataService : IPEFormDataService
    {
        public PEFormDataService()
        {
        }

        public async Task<PEFormHolder> CollectQuestionAnswer(PEFormHolder holder)
        {
            throw new System.NotImplementedException();
        }

        public async Task<PEFormHolder> GetJotters(PEFormHolder holder, long competencyId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<PEFormHolder> GetViolations(PEFormHolder holder, long competencyId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<PEFormHolder> InitForm(long id)
        {
            var retVal = new PEFormHolder()
            {
                EmployeeName = "Robles, Bryan",
                EmployeeNumber = "UP00026",
                Department = "BUSDEV Team",
                Position = "Business System Analyst",
                HireDate = "Since: 01/01/2020",
                Status = "Approved",
                AppraisalType = "Annual",
                EvaluatorType = "Self Appraisal",
                EvaluatorName = "Robles, Bryan",
                Period = "01/01/2022 - 03/01/2022",
                Progress = 20,
            };

            retVal.Objectives = new ObservableCollection<ObjectiveDetailDto>()
            {
                new ObjectiveDetailDto()
                {
                    ObjectiveDetail = "Reach 10M total premium",
                    ObjectiveDescription = "Target quota for Brand #1",
                    BaseLine = "75",
                    Target = "100",
                    EmployeeReview = "",
                    Actual = "",
                    EmployeeRating = 0,
                    PODetailId = 1,
                    ParentObjective = "Organization Goals",
                },
                new ObjectiveDetailDto()
                {
                    ObjectiveDetail = "Reach 5M total premium",
                    ObjectiveDescription = "Target quota for Brand #1",
                    BaseLine = "75",
                    Target = "100",
                    EmployeeReview = "",
                    Actual = "",
                    EmployeeRating = 0,
                    PODetailId = 2,
                    ParentObjective = "Organization Goals",
                },
                new ObjectiveDetailDto()
                {
                    ObjectiveDetail = "Reach 10M total premium",
                    ObjectiveDescription = "Target quota for Brand #1",
                    BaseLine = "75",
                    Target = "100",
                    EmployeeReview = "",
                    Actual = "",
                    EmployeeRating = 0,
                    PODetailId = 3,
                    ParentObjective = "ultrices gravida dictum fusce ut",
                },
                new ObjectiveDetailDto()
                {
                    ObjectiveDetail = "Reach 5M total premium",
                    ObjectiveDescription = "Target quota for Brand #1",
                    BaseLine = "75",
                    Target = "100",
                    EmployeeReview = "",
                    Actual = "",
                    EmployeeRating = 0,
                    PODetailId = 4,
                    ParentObjective = "ultrices gravida dictum fusce ut",
                },
                new ObjectiveDetailDto()
                {
                    ObjectiveDetail = "Reach 10M total premium",
                    ObjectiveDescription = "Target quota for Brand #1",
                    BaseLine = "75",
                    Target = "100",
                    EmployeeReview = "",
                    Actual = "",
                    EmployeeRating = 0,
                    PODetailId = 5,
                    ParentObjective = "proin nibh nisl condimentum id",
                },
                new ObjectiveDetailDto()
                {
                    ObjectiveDetail = "Reach 5M total premium",
                    ObjectiveDescription = "Target quota for Brand #1",
                    BaseLine = "75",
                    Target = "100",
                    EmployeeReview = "",
                    Actual = "",
                    EmployeeRating = 0,
                    PODetailId = 6,
                    ParentObjective = "proin nibh nisl condimentum id",
                },
            };

            /*
            var grouped = retVal.Objectives.GroupBy(g => g.ParentObjective);
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

            */

            /*retVal.ObjectivesDisplay = new ObservableCollection<ObjectiveDetailHeaderDto>(items);*/
            retVal.ObjectivesDisplay = new ObservableCollection<MainObjectiveDto>();

            retVal.Violations = new ObservableCollection<ViolationDto>()
            {
                new ViolationDto()
                {
                    OffenseCount = 1,
                    ViolationDate_String = "02/09/2020",
                    ViolationNo = "1020210311",
                    ViolationDetail = $"Against Property of the Company - Use of company resources for personal reasons.",
                    ViolationId = 1,
                },
                new ViolationDto()
                {
                    OffenseCount = 1,
                    ViolationDate_String = "01/05/2020",
                    ViolationNo = "1120210310",
                    ViolationDetail = $"A-I SECTION 1 - V4 - Disrespect and discourtesy to customers and officers",
                    ViolationId = 2,
                }
            };

            retVal.JotterSource = new ObservableCollection<JotterDto>()
            {
                new JotterDto()
                {
                    RecordId = 1 ,
                    ReportedBy = "Christopher Bautista from Finance Department is &#x1F604; with you.",
                    Message = "Great job on the training plan for the company! Looking forward to its execution soon!",
                    TimeStamp_String = "Sun, 02/28/2020 11:30 PM",
                },
                new JotterDto()
                {
                    RecordId = 2 ,
                    ReportedBy = "Marie Lopez from Operation Department is &#x1F60E; with you.",
                    Message = "Great job on the training plan for the company! Looking forward to its execution soon!",
                    TimeStamp_String = "Sun, 02/15/2020 11:20 PM",
                },
            };

            retVal.Narratives = new ObservableCollection<NarrativeSectionDto>()
            {
                new NarrativeSectionDto()
                {
                    Question = "Rater's Recommendation",
                    Answer = "",
                    NarrativeSectionId = "1",
                    EvaluationId = 1,
                    IsOpened = true,
                    QuestionId = 1,
                },
                new NarrativeSectionDto()
                {
                    Question = "Employee's Comments on Appraisal",
                    Answer = "",
                    NarrativeSectionId = "2",
                    EvaluationId = 2,
                    QuestionId = 2,
                },
                new NarrativeSectionDto()
                {
                    Question = "Team Member's Strength",
                    Answer = "",
                    NarrativeSectionId = "3",
                    EvaluationId = 3,
                    QuestionId = 3,
                },
                new NarrativeSectionDto()
                {
                    Question = "Team Member's Areas for Improvement",
                    Answer = "",
                    NarrativeSectionId = "4",
                    EvaluationId = 4,
                    QuestionId = 4,
                },
                new NarrativeSectionDto()
                {
                    Question = "Team Member's Strength",
                    Answer = "",
                    NarrativeSectionId = "6",
                    EvaluationId = 5,
                    QuestionId = 5,
                },
                new NarrativeSectionDto()
                {
                    Question = "Team Member's Development Areas",
                    Answer = "",
                    NarrativeSectionId = "7",
                    EvaluationId = 6,
                    QuestionId = 6,
                },
            };

            return await Task.FromResult(retVal);
        }

        public async Task<PEFormHolder> OnCompetencyChanged(PEFormHolder holder, CombinedCriteriaDto criteria)
        {
            throw new System.NotImplementedException();
        }

        public async Task<PEFormHolder> SavePODetails(PEFormHolder holder)
        {
            try
            {
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
            try
            {
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
            throw new NotImplementedException();
        }

        public async Task<PEFormHolder> ManageUploadFile(FileUploadResponse file, PEFormHolder holder, bool isDelete = false)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ViewAttachment(string fileUpload, string fileName, string fileType)
        {
            throw new NotImplementedException();
        }

        public async Task<PEFormHolder> SubmitPerformanceEvaluation(PEFormHolder holder)
        {
            throw new NotImplementedException();
        }
    }
}