using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.PerformanceEvaluation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IPEListDataService
    {
        long TotalListItem { get; set; }

        Task<ObservableCollection<PEListDto>> GetListAsync(ObservableCollection<PEListDto> list, ListParam args);
    }
}