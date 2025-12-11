using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using Syncfusion.ListView.XForms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IMyTimeLogsDataService
    {
        long TotalListItem { get; set; }

        Task<SfListView> InitListView(SfListView listview);

        Task<ObservableCollection<MyTimeLogsListModel>> RetrieveMyRequestList(ObservableCollection<Models.MyTimeLogsListModel> list, ListParam obj);

        Task<ObservableCollection<SelectableListModel>> RetrieveStatus();
    }
}