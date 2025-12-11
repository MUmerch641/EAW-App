using EatWork.Mobile.Models.DataAccess;
using System.Threading.Tasks;

namespace EatWork.Mobile.Utils.DataAccess
{
    public class ClientSetupDataAccess : SqliteDataAccess<ClientSetupModel>
    {
        public ClientSetupDataAccess() : base()
        {
        }

        public async Task DeleteAllData()
        {
            await this.Database.DeleteAllAsync<ClientSetupModel>();
        }

        public async Task<ClientSetupModel> RetrieveClientSetup()
        {
            return await this.Database.Table<ClientSetupModel>().Where(p => p.ID > 0).FirstOrDefaultAsync();
        }
    }
}