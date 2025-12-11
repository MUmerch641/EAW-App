using EatWork.Mobile.Models;
using EatWork.Mobile.Models.Attendance;
using EatWork.Mobile.Models.DataObjects;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IAttendanceViewTemplate2DataService
    {
        long TotalListItem { get; set; }
        long TotalListItemDetail { get; set; }

        Task<ObservableCollection<DetailedAttendanceListModel>> GetListAsync(ObservableCollection<DetailedAttendanceListModel> list, ListParam args);

        Task<ObservableCollection<IndividualAttendance>> InitFormAsync(ObservableCollection<IndividualAttendance> list, ListParamsRecordId args);
    }
}