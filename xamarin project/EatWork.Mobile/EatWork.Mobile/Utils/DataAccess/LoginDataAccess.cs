using EatWork.Mobile.Models.DataAccess;
using System.Threading.Tasks;

namespace EatWork.Mobile.Utils.DataAccess
{
    public class LoginDataAccess : SqliteDataAccess<LoginDataModel>
    {
        public LoginDataAccess() : base()
        {
        }

        public async Task DeleteAllData()
        {
            await this.Database.DeleteAllAsync<LoginDataModel>();
        }

        public async Task<LoginDataModel> RetrieveLoginCredential()
        {
            return await this.Database.Table<LoginDataModel>().Where(p => p.ID > 0).FirstOrDefaultAsync();
        }
    }
}