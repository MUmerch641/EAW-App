using System.Collections.ObjectModel;
using MauiHybridApp.Models.Questionnaire;

namespace MauiHybridApp.Services.Data;

public interface ISurveyDataService
{
    Task<ObservableCollection<PulseSurveyList>> RetrieveSurveysAsync();
    Task<ObservableCollection<BaseQControlDto>> RetrievePulseSurveyAsync(ObservableCollection<BaseQControlDto> list, long id, long answerId);
    Task<SurveyHolder> SubmitAnswerAsync(SurveyHolder holder);
    Task<SurveyChartHolder> RetrieveChartSurveyAsync(long formHeaderId);
}
