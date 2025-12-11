using EatWork.Mobile.Contants;
using System;

namespace EatWork.Mobile.Models
{
    public class DocumentRequestModel
    {
        public DocumentRequestModel()
        {
            StatusId = RequestStatusValue.Submitted;
            DateStart = DateTime.Now.Date;
            DateEnd = DateTime.Now.Date;
            DateAvailable = Constants.NullDate;
            DateClaimed = Constants.NullDate;
            ProfileId = 0;
            ParameterId = 0;
            SourceId = (short)SourceEnum.Mobile;
        }

        public long DocumentRequestId { get; set; }
        public long? ProfileId { get; set; }
        public DateTime? DateRequested { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public DateTime? DateAvailable { get; set; }
        public string Remark { get; set; }
        public DateTime? DateClaimed { get; set; }
        public string RecievedBy { get; set; }
        public string Details { get; set; }
        public string Reason { get; set; }
        public long? DocumentId { get; set; }
        public long? StatusId { get; set; }
        public long? ParameterId { get; set; }
        public string Signatory1 { get; set; }
        public string Designation1 { get; set; }
        public string Signatory2 { get; set; }
        public string Designation2 { get; set; }
        public short? SourceId { get; set; }
    }
}