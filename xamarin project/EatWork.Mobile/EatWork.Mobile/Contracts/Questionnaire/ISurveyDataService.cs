using EatWork.Mobile.Models.FormHolder.Questionnaire;
using EatWork.Mobile.Models.Questionnaire;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using APIM = EAW.API.DataContracts;

namespace EatWork.Mobile.Contracts
{
    public interface ISurveyDataService
    {
        Task<ObservableCollection<BaseQControlDto>> RetrievePulseSurvey(ObservableCollection<BaseQControlDto> list, long id, long answerId);

        Task<SurveyHolder> SubmitAnswer(SurveyHolder holder);

        Task<ObservableCollection<APIM.Models.PulseSurveyList>> RetrieveSurveys();

        Task<SurveyChartHolder> RetrieveChartSurvey(long formHeaderId);
    }
}