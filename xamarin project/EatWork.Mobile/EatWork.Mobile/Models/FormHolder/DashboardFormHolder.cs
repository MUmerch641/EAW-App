using EatWork.Mobile.Models.Dashboard;

namespace EatWork.Mobile.Models.FormHolder
{
    public class DashboardFormHolder
    {
        public DashboardFormHolder()
        {
            TardinessWTD = new DashboardModel();
            TardinessMTD = new DashboardModel();
            TardinessYTD = new DashboardModel();
            AbsencesWTD = new DashboardModel();
            AbsencesMTD = new DashboardModel();
            AbsencesYTD = new DashboardModel();
            TotalOvertimeWTD = new DashboardModel();
            TotalOvertimeMTD = new DashboardModel();
            TotalOvertimeYTD = new DashboardModel();
            VacationLeaveBalance = new DashboardModel();
            SickLeaveBalance = new DashboardModel();
        }

        public DashboardModel TardinessWTD { get; set; }
        public DashboardModel TardinessMTD { get; set; }
        public DashboardModel TardinessYTD { get; set; }
        public DashboardModel AbsencesWTD { get; set; }
        public DashboardModel AbsencesMTD { get; set; }
        public DashboardModel AbsencesYTD { get; set; }

        public DashboardModel TotalOvertimeWTD { get; set; }
        public DashboardModel TotalOvertimeMTD { get; set; }
        public DashboardModel TotalOvertimeYTD { get; set; }

        public DashboardModel VacationLeaveBalance { get; set; }
        public DashboardModel SickLeaveBalance { get; set; }
    }
}