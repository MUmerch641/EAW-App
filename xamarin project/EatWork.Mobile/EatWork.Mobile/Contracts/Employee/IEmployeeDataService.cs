using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using Syncfusion.ListView.XForms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IEmployeeDataService
    {
        #region employee list

        Task<SfListView> InitListView(SfListView listview);

        Task<ObservableCollection<EmployeeListModel>> RetrieveEmployeeList(ObservableCollection<Models.EmployeeListModel> list, ListParam obj);

        #endregion employee list
    }
}