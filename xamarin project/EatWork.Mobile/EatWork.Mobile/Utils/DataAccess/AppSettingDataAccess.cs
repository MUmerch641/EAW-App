using EatWork.Mobile.Models.DataAccess;
using System.Threading.Tasks;

namespace EatWork.Mobile.Utils.DataAccess
{
    public class EmployeeFilterSelectionDataAccess : SqliteDataAccess<EmployeeFilterSelectionDataModel>
    {
        public EmployeeFilterSelectionDataAccess() : base()
        {
        }

        public async Task DeleteSetup()
        {
            await this.Database.DeleteAllAsync<EmployeeFilterSelectionDataModel>();
        }

        public async Task<EmployeeFilterSelectionDataModel> RetrieveSetup()
        {
            var data = await this.Database.Table<EmployeeFilterSelectionDataModel>().Where(p => p.ID > 0).FirstOrDefaultAsync();

            if (data == null)
                data = new EmployeeFilterSelectionDataModel();

            return data;
        }
    }
}