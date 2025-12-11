using SQLite;

namespace EatWork.Mobile.Models.DataAccess
{
    [Table("UserDeviceInfo")]
    public class UserDeviceInfoModel
    {
        [PrimaryKey, AutoIncrement]
        public long ID { get; set; }

        public string DeviceId { get; set; }
        public bool IsRegistered { get; set; }
    }
}