using EatWork.Mobile.Models.FormHolder.Questionnaire;
using EatWork.Mobile.Models.Questionnaire;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface ISurveyService
    {
        Task<ObservableCollection<BaseQControlDto>> RetrievePulseSurvey(ObservableCollection<BaseQControlDto> list);

        Task<SurveyHolder> SubmitAnswer(SurveyHolder holder);
    }
}