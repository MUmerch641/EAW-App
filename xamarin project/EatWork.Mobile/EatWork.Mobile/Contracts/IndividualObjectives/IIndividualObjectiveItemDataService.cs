using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IIndividualObjectiveItemDataService
    {
        long TotalListItem { get; set; }

        Task<IndividualObjectiveItemHolder> InitForm(long id);

        Task<IndividualObjectiveItemHolder> SubmitRequest(IndividualObjectiveItemHolder holder);

        Task<ObjectiveDetailHolder> InitObjectiveDetailForm(long id, short effectiveYear);

        /*Task<ObjectiveDetailHolder> SaveObjectiveDetail(ObjectiveDetailHolder holder);*/

        Task<KPISelectionResponse> RetrieveKPICriteria(long id, ObservableCollection<RateScaleDto> list);

        Task<ObjectiveGroupingResponse> RetrieveStandardObjectives(string effectiveYear);

        Task<ObjectiveDetailHolder> SetValueObjectiveDetailForm(ObjectiveDetailDto item, ObjectiveDetailHolder holder);

        Task<IndividualObjectiveItemHolder> CancelRequest(IndividualObjectiveItemHolder holder);

        Task<ObjectiveGroupingResponse> GroupObjectives(ObservableCollection<ObjectiveDetailDto> items);
    }
}