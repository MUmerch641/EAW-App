using Acr.UserDialogs;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder.Questionnaire;
using EatWork.Mobile.Models.Questionnaire;
using EatWork.Mobile.Utils;
using EAW.API.DataContracts.Models;
using Mobile.Utils.ControlGenerator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services.Questionnaire
{
    public class SurveyService : ISurveyDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IDialogService dialogService_;

        public SurveyService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IDialogService dialogService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            dialogService_ = dialogService;
        }

        #region details

        public async Task<ObservableCollection<BaseQControlDto>> RetrievePulseSurvey(ObservableCollection<BaseQControlDto> list, long id, long answerId)
        {
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var user = PreferenceHelper.UserInfo();

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.Questionnaire}/{id}/{answerId}/retrieve-pulse-survey"
                };

                var response = await genericRepository_.GetAsync<R.Responses.QuestionnaireFormResponse>(builder.ToString());

                if (response.ControlList.Count > 0)
                {
                    int ctr = 1;
                    foreach (var p in response.ControlList.OrderBy(p => p.BaseQuestion.QuestionSortOrder))
                    {
                        var answer = response.AnswerList.Where(x => x.FormQuestionId == p.BaseQuestion.FormQuestionId).FirstOrDefault();

                        list.Add(new BaseQControlDto()
                        {
                            BaseQuestion = new BaseQuestion()
                            {
                                Comment = p.BaseQuestion.Comment,
                                ControlTypeId = p.BaseQuestion.ControlTypeId,
                                DisplayPage = p.BaseQuestion.DisplayPage,
                                FormQuestionId = p.BaseQuestion.FormQuestionId,
                                IfAnswer = p.BaseQuestion.IfAnswer,
                                IsRequired = p.BaseQuestion.IsRequired,
                                IsRequiredComment = p.BaseQuestion.IsRequiredComment,
                                MaxLength = p.BaseQuestion.MaxLength,
                                Options = p.BaseQuestion.Options,
                                Question = p.BaseQuestion.Question,
                                QuestionSortOrder = p.BaseQuestion.QuestionSortOrder,
                                Required = p.BaseQuestion.Required,
                                Section = p.BaseQuestion.Section,
                                SectionSortOrder = p.BaseQuestion.SectionSortOrder,
                            },
                            DisplayPage = p.DisplayPage,
                            GeneratedQuestion = p.GeneratedQuestion,
                            ID = p.BaseQuestion.FormQuestionId.ToString(),
                            Name = p.QuestionName,
                            Question = p.Question,
                            QuestionName = p.QuestionName,
                            QuestionNumber = p.QuestionNumber,
                            QuestionSection = p.QuestionSection,
                            QuestionSortOrder = p.QuestionSortOrder,
                            SectionSortOrder = p.SectionSortOrder,
                            ControlName = Enum.GetName(typeof(ControlTypeEnum), p.BaseQuestion.ControlTypeId),
                            QuestionHeader = string.Format("{0}. Question #{1} {2}", ctr, p.QuestionNumber,
                                            (!string.IsNullOrEmpty(p.BaseQuestion.Comment) ? " - " + p.BaseQuestion.Comment : "")),
                            QuestionDetail = p.BaseQuestion.Question,
                            FormHeaderId = response.FormHeaderId,
                            FormSurveyHistoryId = response.FormSurveyHistoryId,
                            Answer = (answer != null ? answer.Answer : ""),
                            HasAnswer = (answer != null),
                        });

                        ctr++;
                    }
                    /*
                    retVal.QuestionList = new ObservableCollection<BaseQuestionDto>(
                        response.ControlList.Select(p => new BaseQuestionDto
                        {
                            Comment = p.BaseQuestion.Comment,
                            ControlTypeId = p.BaseQuestion.ControlTypeId,
                            DisplayPage = p.DisplayPage,
                            FormQuestionId = p.BaseQuestion.FormQuestionId,
                            IfAnswer = p.BaseQuestion.IfAnswer,
                            IsRequired = p.BaseQuestion.IsRequired,
                            IsRequiredComment = p.BaseQuestion.IsRequiredComment,
                            MaxLength = p.BaseQuestion.MaxLength,
                            Options = p.BaseQuestion.Options,
                            Question = p.Question,
                            QuestionSortOrder = p.QuestionSortOrder,
                            Section = p.BaseQuestion.Section,
                            SectionSortOrder = p.SectionSortOrder,
                        })
                    );
                    */
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return list;
        }

        public async Task<SurveyHolder> SubmitAnswer(SurveyHolder holder)
        {
            try
            {
                if (await dialogService_.ConfirmDialogAsync(Messages.Submit))
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);

                        var url = await commonDataService_.RetrieveClientUrl();
                        await commonDataService_.HasInternetConnection(url);
                        var builder = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.FormAnswer}"
                        };

                        var answers = new List<R.Requests.SurveyAnswerDto>(
                            holder.Answers.Select(p => new R.Requests.SurveyAnswerDto
                            {
                                Answer = p.Value,
                                Comment = string.Empty,
                                Points = string.Empty,
                                FormQuestionId = p.FormQuestionId,
                            })
                        );

                        var data = new R.Requests.SubmitSurveryAnswerRequest()
                        {
                            FormHeaderId = holder.FormHeaderId,
                            FormSurveyHistoryId = holder.FormSurveyHistoryId,
                            AnswerList = answers,
                            IsSubmitAnonymously = holder.SubmitAnonimously,
                        };

                        var response = await genericRepository_.PostAsync<R.Requests.SubmitSurveryAnswerRequest, R.Responses.BaseResponse<R.Models.FormAnswer>>(builder.ToString(), data);

                        if (response.Model.FormAnswerId > 0)
                        {
                            holder.IsSuccess = true;
                            FormSession.IsSubmitted = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return holder;
        }

        #endregion details

        #region list

        public async Task<ObservableCollection<PulseSurveyList>> RetrieveSurveys()
        {
            var list = new ObservableCollection<PulseSurveyList>();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var user = PreferenceHelper.UserInfo();

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.Questionnaire}/{user.ProfileId}/retrieve-pulse-surveys"
                };

                var response = await genericRepository_.GetAsync<R.Responses.UserQuestionnaireFormResponse>(builder.ToString());

                if (response.SurveyList.Count > 0)
                    list = new ObservableCollection<PulseSurveyList>(response.SurveyList);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw ex;
            }

            return list;
        }

        #endregion list

        #region charts

        public async Task<SurveyChartHolder> RetrieveChartSurvey(long formHeaderId)
        {
            var holder = new SurveyChartHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.Questionnaire}/{formHeaderId}/get-chart-data"
                };

                var response = await genericRepository_.GetAsync<R.Responses.GetSurveyChartResponse>(builder.ToString());

                if (response != null)
                {
                    holder.ChartTitle = string.Empty;
                    holder.PrimaryAxisTitle = string.Empty;

                    var answerlist = response.BarChart.AnswerDetailsFormQuestionIds.Split('|').ToList();
                    var questions = response.BarChart.Questions.Split(',').ToList();
                    var endDates = response.BarChart.EndDates.Split('|').ToList();

                    foreach (var answer in answerlist)
                    {
                    }

                    /*
                    holder.BarData = new ObservableCollection<ChartDataModel>()
                    {
                        new ChartDataModel("Colors", 5, ""),
                    };
                    */
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw ex;
            }

            return holder;
        }

        #endregion charts
    }
}