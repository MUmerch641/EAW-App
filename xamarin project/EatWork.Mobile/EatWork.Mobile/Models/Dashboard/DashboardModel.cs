namespace EatWork.Mobile.Models.Dashboard
{
    public class DashboardModel
    {
        public DashboardModel()
        {
            InfoboxDetail = string.Empty;
            InfoboxValue = 0;
        }

        public string InfoboxDetail { get; set; }
        public decimal InfoboxValue { get; set; }
    }
}