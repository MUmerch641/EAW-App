using EatWork.Mobile.Models.FormHolder.PerformanceEvaluation;
using System.Threading.Tasks;
using APIM = EAW.API.DataContracts;

namespace EatWork.Mobile.Contracts
{
    public interface IPEFormDataService
    {
        Task<PEFormHolder> InitForm(long id);

        Task<PEFormHolder> GetViolations(PEFormHolder holder, long competencyId);

        Task<PEFormHolder> GetJotters(PEFormHolder holder, long competencyId);

        Task<PEFormHolder> CollectQuestionAnswer(PEFormHolder holder);

        Task<PEFormHolder> OnCompetencyChanged(PEFormHolder holder, APIM.Models.CombinedCriteriaDto criteria);

        Task<PEFormHolder> SavePODetails(PEFormHolder holder);

        Task<PEFormHolder> SavePAQuestionnaire(PEFormHolder holder);

        Task<PEFormHolder> SavePANarrativeSection(PEFormHolder holder);

        Task<PEFormHolder> ValidatePANarrativeSection(PEFormHolder holder);

        Task<PEFormHolder> ManageUploadFile(Models.DataObjects.FileUploadResponse file, PEFormHolder holder, bool isDelete = false);

        Task<string> ViewAttachment(string fileUpload, string fileName, string fileType);

        Task<PEFormHolder> SubmitPerformanceEvaluation(PEFormHolder holder);
    }
}