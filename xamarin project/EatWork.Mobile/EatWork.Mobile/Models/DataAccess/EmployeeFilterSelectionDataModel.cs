using SQLite;

namespace EatWork.Mobile.Models.DataAccess
{
    [Table("EmployeeFilterSelection")]
    public class EmployeeFilterSelectionDataModel : EmployeeFilterSelection
    {
        [PrimaryKey, AutoIncrement]
        public long ID { get; set; }
    }
}