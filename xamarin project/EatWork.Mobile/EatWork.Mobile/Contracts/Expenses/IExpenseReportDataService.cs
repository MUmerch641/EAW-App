using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Expenses;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IExpenseReportDataService
    {
        #region LIST

        long TotalListItem { get; set; }

        Task<ObservableCollection<MyExpenseReportsList>> RetrieveMyExpenses(ObservableCollection<MyExpenseReportsList> list, ListParam args);

        #endregion LIST

        #region DETAIL

        Task<ExpenseReportDetailHolder> InitForm(ObservableCollection<MyExpensesListDto> details = null);

        Task<ExpenseReportDetailHolder> RetrieveRecord(long id);

        Task<ExpenseReportDetailHolder> SubmitRecord(ExpenseReportDetailHolder holder);

        Task<ExpenseReportDetailHolder> WorkflowTransactionRequest(ExpenseReportDetailHolder holder);

        #endregion DETAIL
    }
}