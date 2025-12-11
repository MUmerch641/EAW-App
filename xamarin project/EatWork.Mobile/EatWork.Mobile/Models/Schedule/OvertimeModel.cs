using EatWork.Mobile.Contants;
using System;

namespace EatWork.Mobile.Models
{
    public class OvertimeModel
    {
        public OvertimeModel()
        {
            OvertimeDate = DateTime.Now.Date;
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            DateFiled = DateTime.Now.Date;
            OROTHrs = (decimal)0;
            NSOTHrs = (decimal)0;
            StatusId = RequestStatusValue.Draft;
            ForOffsetting = false;
            ApprovedOROTHrs = 0;
            ApprovedNSOTHrs = 0;
            OffsettingExpirationDate = Constants.NullDate;
            SourceId = (short)SourceEnum.Mobile;
        }

        public long OvertimeId { get; set; }
        public long? ProfileId { get; set; }
        public DateTime? DateFiled { get; set; }
        public DateTime? OvertimeDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? OROTHrs { get; set; }
        public decimal? NSOTHrs { get; set; }
        public string Reason { get; set; }
        public string Remarks { get; set; }
        public decimal? ApprovedOROTHrs { get; set; }
        public decimal? ApprovedNSOTHrs { get; set; }
        public string ApproverRemarks { get; set; }
        public short? ComputeHour { get; set; }
        public short? PreShiftOT { get; set; }
        public long? StatusId { get; set; }
        public short? SourceId { get; set; }
        public bool? ForOffsetting { get; set; }
        public DateTime? OffsettingExpirationDate { get; set; }
    }
}