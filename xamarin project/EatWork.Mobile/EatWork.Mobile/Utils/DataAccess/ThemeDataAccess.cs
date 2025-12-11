using EatWork.Mobile.Models.DataAccess;
using System.Threading.Tasks;

namespace EatWork.Mobile.Utils.DataAccess
{
    public class ThemeDataAccess : SqliteDataAccess<ThemeConfigDataModel>
    {
        public ThemeDataAccess() : base()
        {
        }

        public async Task DeleteSetup()
        {
            await this.Database.DeleteAllAsync<ThemeConfigDataModel>();
        }

        public async Task<ThemeConfigDataModel> RetrieveThemeSetup()
        {
            return await this.Database.Table<ThemeConfigDataModel>().Where(p => p.ID > 0).FirstOrDefaultAsync();
        }
    }

    public class UserDeviceInfoDataAccess : SqliteDataAccess<UserDeviceInfoModel>
    {
        public UserDeviceInfoDataAccess() : base()
        {
        }

        public async Task DeleteSetup()
        {
            await this.Database.DeleteAllAsync<UserDeviceInfoModel>();
        }

        public async Task<bool> IsDeviceRegistered(string deviceId)
        {
            var record = await this.Database.Table<UserDeviceInfoModel>().Where(p => p.DeviceId.Equals(deviceId)).FirstOrDefaultAsync();
            return record?.IsRegistered ?? false;
        }
    }
}