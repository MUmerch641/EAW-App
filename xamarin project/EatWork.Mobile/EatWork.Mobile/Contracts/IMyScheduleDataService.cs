using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using Syncfusion.ListView.XForms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IMyScheduleDataService
    {
        long TotalListItem { get; set; }

        Task<SfListView> InitListView(SfListView listview);

        Task<ObservableCollection<MyScheduleListModel>> RetrieveMyScheduleList(ObservableCollection<Models.MyScheduleListModel> list, ListParam obj);

        Task<MyScheduleListModel> RetrieveCurrentSchedule(ListParam obj);
    }
}