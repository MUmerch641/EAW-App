using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IIndividualObjectivesDataService
    {
        long TotalListItem { get; set; }

        Task<ObservableCollection<IndividualObjectivesDto>> GetListAsync(ObservableCollection<IndividualObjectivesDto> list, ListParam args);
    }
}