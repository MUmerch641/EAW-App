using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataAccess;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils.DataAccess;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Services
{
    public class SettingsDataService : ISettingsDataService
    {
        private readonly EmployeeFilterSelectionDataAccess employeeFilterSelectionDataAccess_;

        public SettingsDataService()
        {
            employeeFilterSelectionDataAccess_ = AppContainer.Resolve<EmployeeFilterSelectionDataAccess>();
        }

        public async Task<ObservableCollection<SelectableListModel>> EmployeeFilterConfig()
        {
            var config = new EmployeeFilterSelectionDataModel();

            var setup = await employeeFilterSelectionDataAccess_.RetrieveSetup();

            if (setup != null)
                config = setup;

            var retValue = new ObservableCollection<SelectableListModel>()
            {
                new SelectableListModel{IsChecked = config.ByBranch, DisplayText = "Limit to Branch", Id = 1 },
                new SelectableListModel{IsChecked = config.ByDepartment, DisplayText = "Limit to Department", Id = 2 },
                new SelectableListModel{IsChecked = config.ByTeam, DisplayText = "Limit to Team", Id = 3},
            };

            return await Task.FromResult(retValue);
        }

        public async Task UpdateEmployeeFilterSetup(SelectableListModel item)
        {
            var data = await employeeFilterSelectionDataAccess_.RetrieveSetup();

            if (data == null)
                data = new EmployeeFilterSelectionDataModel();

            switch (item.Id)
            {
                case 1:
                    data.ByBranch = item.IsChecked;
                    break;

                case 2:
                    data.ByDepartment = item.IsChecked;
                    break;

                case 3:
                    data.ByTeam = item.IsChecked;
                    break;

                default:
                    break;
            }

            if (data.ID != 0)
                await employeeFilterSelectionDataAccess_.UpdateRecord(data);
            else
                await employeeFilterSelectionDataAccess_.SaveRecord(data);
        }
    }
}