using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.Expense;
using EatWork.Mobile.Models.FormHolder.Expenses;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IExpenseDataService
    {
        long TotalListItem { get; set; }

        Task<ObservableCollection<MyExpensesListDto>> GetLisyAsync(ObservableCollection<MyExpensesListDto> list, ListParam args);

        Task<ObservableCollection<SelectableListModel>> GetExpenseTypes();

        Task<AppExpenseReportDetail> GetRecordAsync(long id);

        Task<NewExpenseHolder> InitExpenseForm();

        Task<NewExpenseHolder> SubmitRecord(NewExpenseHolder holder);

        Task<int> DeleteAsync(ObservableCollection<MyExpensesListDto> list);
    }
}