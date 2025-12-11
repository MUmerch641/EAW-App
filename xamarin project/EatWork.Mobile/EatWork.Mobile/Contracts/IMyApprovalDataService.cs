using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using Syncfusion.ListView.XForms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IMyApprovalDataService
    {
        long TotalListItem { get; set; }

        Task<SfListView> InitListView(SfListView listview, bool isAscending = false);

        Task<ObservableCollection<MyApprovalListModel>> RetrieveApprovalList(ObservableCollection<MyApprovalListModel> list, ListParam obj);
    }
}