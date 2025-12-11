using EatWork.Mobile.Contants;
using System;

namespace EatWork.Mobile.Models
{
    public class LeaveRequestModel
    {
        public LeaveRequestModel()
        {
            RemainingHours = 0;
            Reason = string.Empty;
            NoOfDays = 0;
            LeaveTypeId = 0;
            StatusId = RequestStatusValue.Submitted;
            /*DateFiled = DateTime.UtcNow.Date;*/
            DateFiled = DateTime.Now;
            NoOfHours = 0;
            TotalNoOfHours = 0;
            PartialDayLeave = 0;
            Planned = 0;
            ProfileId = 0;
            LeaveRequestHeaderId = 0;
            LeaveRequestId = 0;
            SourceId = (short)SourceEnum.Mobile;
        }

        public long LeaveRequestId { get; set; }
        public long? ProfileId { get; set; }
        public long? LeaveTypeId { get; set; }
        public long? CompanyId { get; set; }
        public decimal? RemainingHours { get; set; }
        public DateTime? InclusiveStartDate { get; set; }
        public DateTime? InclusiveEndDate { get; set; }
        public decimal? NoOfHours { get; set; }
        public short? PartialDayLeave { get; set; }
        public short? PartialDayApplyTo { get; set; }
        public string Reason { get; set; }
        public long? StatusId { get; set; }
        public DateTime? DateFiled { get; set; }
        public decimal? TotalNoOfHours { get; set; }
        public short? NoOfDays { get; set; }
        public short? Planned { get; set; }
        public long? LeaveRequestHeaderId { get; set; }
        public short? SourceId { get; set; }
    }
}