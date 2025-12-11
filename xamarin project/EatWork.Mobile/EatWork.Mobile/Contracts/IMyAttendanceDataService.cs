using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IMyAttendanceDataService
    {
        long TotalListItem { get; set; }

        Task<ObservableCollection<MyAttendanceListModel>> GetListAsync(ObservableCollection<MyAttendanceListModel> list, ListParam args);

        Task<ObservableCollection<IndividualAttendance>> GetIndividualAttendanceAsync(ObservableCollection<IndividualAttendance> list, ListParam args);
    }
}