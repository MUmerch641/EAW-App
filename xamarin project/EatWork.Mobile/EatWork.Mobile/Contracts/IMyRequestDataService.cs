using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using Syncfusion.ListView.XForms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IMyRequestDataService
    {
        long TotalListItem { get; set; }

        Task<SfListView> InitListView(SfListView listview);

        Task<ObservableCollection<MyRequestListModel>> RetrieveMyRequestList(ObservableCollection<Models.MyRequestListModel> list, ListParam obj);
    }
}