using SQLite;

namespace EatWork.Mobile.Models.DataAccess
{
    [Table("Login")]
    public class LoginDataModel
    {
        [PrimaryKey, AutoIncrement]
        public long ID { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
    }
}