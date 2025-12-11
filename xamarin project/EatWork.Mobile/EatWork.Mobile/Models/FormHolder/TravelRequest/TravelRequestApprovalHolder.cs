using EatWork.Mobile.Models.FormHolder.Approvals;
using API = EAW.API.DataContracts.Models;

namespace EatWork.Mobile.Models.FormHolder.TravelRequest
{
    public class TravelRequestApprovalHolder : ApprovalHolder
    {
        public TravelRequestApprovalHolder()
        {
            IsSuccess = false;
            Model = new API.TravelRequest();
        }

        public string DateFiled { get; set; }
        public string DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string TripType { get; set; }
        public string SpecialRequestNote { get; set; }
        public long ModuleFormId { get; set; }
        public API.TravelRequest Model { get; set; }
    }
}