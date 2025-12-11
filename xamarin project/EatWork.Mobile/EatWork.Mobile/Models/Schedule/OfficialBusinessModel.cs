using EatWork.Mobile.Contants;
using System;

namespace EatWork.Mobile.Models
{
    public class OfficialBusinessModel
    {
        public OfficialBusinessModel()
        {
            /*DateFiled = DateTime.UtcNow.Date;*/
            DateFiled = DateTime.Now.Date;
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            StartTimePreviousDay = false;
            IncludeRestdays = false;
            IncludeHolidays = false;
            EndTimeNextDay = false;
            NoOfHours = (decimal)0;
            StatusId = RequestStatusValue.Draft;
            OfficialBusinessDate = DateTime.UtcNow.Date;
            SourceId = (short)SourceEnum.Mobile;
        }

        public long OfficialBusinessId { get; set; }
        public byte? TypeId { get; set; }
        public long? ProfileId { get; set; }
        public DateTime? OfficialBusinessDate { get; set; }
        public DateTime? DateFiled { get; set; }
        public string ChargeCode { get; set; }
        public decimal? NoOfHours { get; set; }
        public long? OBTypeId { get; set; }
        public string Remarks { get; set; }
        public byte? ApplyTo { get; set; }
        public long? StatusId { get; set; }
        public string ApproverRemarks { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public long? TransactionId { get; set; }
        public short? IsOvertime { get; set; }
        public long? ParentId { get; set; }
        public short? SourceId { get; set; }
        public bool? StartTimePreviousDay { get; set; }
        public bool? EndTimeNextDay { get; set; }
        public bool? IncludeHolidays { get; set; }
        public bool? IncludeRestdays { get; set; }
    }
}