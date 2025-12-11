using System;

namespace MauiHybridApp.Models
{
    public class ChangeRestdayModel
    {
        public ChangeRestdayModel()
        {
            StatusId = 0; // Draft
            DateFiled = DateTime.Now.Date;
            SwapWithProfileId = 0;
            RestDayDate = DateTime.Now.Date;
            RequestDate = DateTime.Now.Date;
            SourceId = 2; // Mobile
        }

        public long ChangeRestDayId { get; set; }
        public long? ProfileId { get; set; }
        public DateTime? DateFiled { get; set; }
        public DateTime? RestDayDate { get; set; }
        public DateTime? RequestDate { get; set; }
        public long? SwapWithProfileId { get; set; }
        public string? Reason { get; set; }
        public string? ApproverRemarks { get; set; }
        public long? StatusId { get; set; }
        public string? RestDayId { get; set; }
        public long? CreateId { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? LastUpdateId { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public short? SourceId { get; set; }
    }
}
