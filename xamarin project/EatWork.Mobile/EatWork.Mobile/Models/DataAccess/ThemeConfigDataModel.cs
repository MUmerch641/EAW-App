using SQLite;

namespace EatWork.Mobile.Models.DataAccess
{
    [Table("ThemeConfig")]
    public class ThemeConfigDataModel : ThemeConfigModel
    {
        [PrimaryKey, AutoIncrement]
        public long ID { get; set; }
    }
}