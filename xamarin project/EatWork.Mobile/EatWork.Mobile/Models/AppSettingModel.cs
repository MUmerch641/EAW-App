namespace EatWork.Mobile.Models
{
    public class EmployeeFilterSelection
    {
        public EmployeeFilterSelection()
        {
            ByBranch = false;
            ByDepartment = false;
            ByTeam = false;
        }

        public bool ByBranch { get; set; }
        public bool ByDepartment { get; set; }
        public bool ByTeam { get; set; }
    }
}