using EatWork.Mobile.Models.Accountability;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.CashAdvance;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface ICashAdvanceRequestDataService
    {
        #region LIST

        long TotalListItem { get; set; }

        Task<ObservableCollection<CashAdvanceRequestList>> RetrieveList(ObservableCollection<CashAdvanceRequestList> list, ListParam args);

        #endregion LIST

        Task<CashAdvanceRequestHolder> InitForm(long recordId);

        Task<CashAdvanceRequestHolder> SubmitAsync(CashAdvanceRequestHolder holder);
        Task<CashAdvanceRequestHolder> CancelRequestAsync(CashAdvanceRequestHolder holder);
    }
}