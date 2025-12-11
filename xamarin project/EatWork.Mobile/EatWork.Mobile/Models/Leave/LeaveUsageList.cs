using EatWork.Mobile.ViewModels;
using System;

namespace EatWork.Mobile.Models
{
    public class LeaveUsageList : BaseViewModel
    {
        public string LeaveTypeSetup { get; set; }
        public DateTime DateFiled { get; set; }
        public decimal? NoOfHours { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string InclusiveDate { get; set; }
        public bool? DisplayInDays { get; set; }
        public int? NoOfHoursPerDay { get; set; }
        public string LeaveRequestDisplay { get; set; }
    }
}