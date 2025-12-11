using EatWork.Mobile.Models.DataObjects;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface ISettingsDataService
    {
        Task<ObservableCollection<SelectableListModel>> EmployeeFilterConfig();

        Task UpdateEmployeeFilterSetup(SelectableListModel item);
    }
}