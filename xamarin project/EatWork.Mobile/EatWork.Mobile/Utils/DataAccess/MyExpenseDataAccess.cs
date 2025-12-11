using EatWork.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EatWork.Mobile.Utils.DataAccess
{
    public class MyExpenseDataAccess : SqliteDataAccess<ExpenseReportDetailListDataModel>
    {
        public MyExpenseDataAccess() : base()
        {
        }

        public async Task DeleteAllData()
        {
            await this.Database.DeleteAllAsync<ExpenseReportDetailListDataModel>();
        }

        public async Task<List<ExpenseReportDetailListDataModel>> RetrieveExpenses()
        {
            return await this.Database.Table<ExpenseReportDetailListDataModel>().ToListAsync();
        }

        public async Task RemoveItem(ExpenseReportDetailListDataModel item)
        {
            await this.Database.Table<ExpenseReportDetailListDataModel>().DeleteAsync(p => p.ID == item.ID);
        }
    }
}