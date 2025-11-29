namespace MauiHybridApp.Models;

public class DashboardResponse
{
    // Inner class jo har property mein repeat ho rahi hai
    public class DashboardInfoBox
    {
        public string InfoboxDetail { get; set; } = string.Empty;
        public decimal InfoboxValue { get; set; }
    }

    // Properties (JSON key names must match case-insensitive mapping)
    public DashboardInfoBox TardinessWTD { get; set; } = new();
    public DashboardInfoBox TardinessMTD { get; set; } = new(); // Late arrivals (Month)
    
    public DashboardInfoBox AbsencesMTD { get; set; } = new(); // Absences (Month)
    
    public DashboardInfoBox TotalOvertimeMTD { get; set; } = new(); // Overtime (Month)
    
    public DashboardInfoBox VacationLeaveBalance { get; set; } = new();
    public DashboardInfoBox SickLeaveBalance { get; set; } = new();
}