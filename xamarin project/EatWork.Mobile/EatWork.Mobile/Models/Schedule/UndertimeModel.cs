using EatWork.Mobile.Contants;
using System;

namespace EatWork.Mobile.Models
{
    public class UndertimeModel
    {
        public UndertimeModel()
        {
            DateFiled = DateTime.Now.Date;
            UndertimeDate = DateTime.Now.Date;
            UTHrs = 0;
            StatusId = RequestStatusValue.Draft;
            SourceId = (short)SourceEnum.Mobile;
        }

        public long UndertimeId { get; set; }
        public long? ProfileId { get; set; }
        public DateTime? DateFiled { get; set; }
        public DateTime? UndertimeDate { get; set; }
        public decimal? UTHrs { get; set; }
        public DateTime? DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public short? UndertimeTypeId { get; set; }
        public string Reason { get; set; }
        public string ApproverRemarks { get; set; }
        public long? StatusId { get; set; }
        public short? SourceId { get; set; }
    }
}