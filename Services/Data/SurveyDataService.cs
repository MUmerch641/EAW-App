using MauiHybridApp.Models;
using MauiHybridApp.Models.Questionnaire;
using MauiHybridApp.Utils;

using MauiHybridApp.Services.Data;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class SurveyDataService : ISurveyDataService
    {
        private readonly IGenericRepository _repository;

        public SurveyDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<ObservableCollection<PulseSurveyList>> RetrieveSurveysAsync()
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                if (string.IsNullOrEmpty(profileIdStr)) return new ObservableCollection<PulseSurveyList>();

                var url = $"{ApiEndpoints.Questionnaire}/{profileIdStr}/retrieve-pulse-surveys";
                var response = await _repository.GetAsync<UserQuestionnaireFormResponse>(url);

                return response?.SurveyList != null 
                    ? new ObservableCollection<PulseSurveyList>(response.SurveyList) 
                    : new ObservableCollection<PulseSurveyList>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RetrieveSurveys Error: {ex.Message}");
                return new ObservableCollection<PulseSurveyList>();
            }
        }

        public async Task<ObservableCollection<BaseQControlDto>> RetrievePulseSurveyAsync(ObservableCollection<BaseQControlDto> list, long id, long answerId)
        {
            try
            {
                var url = $"{ApiEndpoints.Questionnaire}/{id}/{answerId}/retrieve-pulse-survey";
                var response = await _repository.GetAsync<QuestionnaireFormResponse>(url);

                if (response?.ControlList != null && response.ControlList.Count > 0)
                {
                    int ctr = 1;
                    foreach (var p in response.ControlList.OrderBy(p => p.BaseQuestion.QuestionSortOrder))
                    {
                        var answer = response.AnswerList?.FirstOrDefault(x => x.FormQuestionId == p.BaseQuestion.FormQuestionId);

                        p.QuestionHeader = $"{ctr}. Question #{p.QuestionNumber} {(!string.IsNullOrEmpty(p.BaseQuestion.Comment) ? " - " + p.BaseQuestion.Comment : "")}";
                        p.Answer = answer?.Value ?? "";
                        p.HasAnswer = answer != null;
                        p.FormHeaderId = response.FormHeaderId;
                        p.FormSurveyHistoryId = response.FormSurveyHistoryId;

                        // Map Value for UI binding
                        p.Value = p.Answer;

                        list.Add(p);
                        ctr++;
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RetrievePulseSurvey Error: {ex.Message}");
                return list;
            }
        }

        public async Task<SurveyHolder> SubmitAnswerAsync(SurveyHolder holder)
        {
            try
            {
                var answers = holder.Answers.Select(p => new AnswerDto
                {
                    Value = p.Value,
                    Comment = string.Empty,
                    Points = string.Empty,
                    FormQuestionId = p.FormQuestionId,
                }).ToList();

                var data = new SubmitSurveyAnswerRequest
                {
                    FormHeaderId = holder.FormHeaderId,
                    FormSurveyHistoryId = holder.FormSurveyHistoryId,
                    AnswerList = answers,
                    IsSubmitAnonymously = holder.SubmitAnonimously,
                };

                var response = await _repository.PostAsync<SubmitSurveyAnswerRequest, BaseResponse<object>>(ApiEndpoints.FormAnswer, data);

                if (response != null && (response.IsSuccess || response.Model != null || string.IsNullOrEmpty(response.Message)))
                {
                    holder.IsSuccess = true;
                }
                else
                {
                    holder.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SubmitAnswer Error: {ex.Message}");
                holder.IsSuccess = false;
            }

            return holder;
        }

        public async Task<SurveyChartHolder> RetrieveChartSurveyAsync(long formHeaderId)
        {
            try
            {
                var url = $"{ApiEndpoints.Questionnaire}/{formHeaderId}/get-chart-data";
                var response = await _repository.GetAsync<GetSurveyChartResponse>(url);
                
                // Basic holder for now, can expand later
                return new SurveyChartHolder();
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"RetrieveChartSurvey Error: {ex.Message}");
                 return new SurveyChartHolder();
            }
        }
    }
}
