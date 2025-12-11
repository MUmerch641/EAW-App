using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Payslip;
using EatWork.Mobile.Models.Payslip;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IPayslipDataService
    {
        long TotalListItem { get; set; }

        Task<ObservableCollection<MyPayslipListModel>> GetMyPayslipListAsync(ObservableCollection<MyPayslipListModel> list, ListParam args);

        Task<PayslipDetailHolder> GetPayslipDetailAsync(long profileId, long id);

        Task<YTDPayslipDetailHolder> GetPayslipYTDTemplateAsync(long profileId, long id);

        Task PrintPayslip(long profileId, long id);
    }
}