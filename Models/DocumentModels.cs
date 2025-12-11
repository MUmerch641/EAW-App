using System;
using System.Collections.Generic;

namespace MauiHybridApp.Models
{
    public class DocumentRequestModel
    {
        public long DocumentRequestId { get; set; }
        public long? ProfileId { get; set; }
        public DateTime? RequestDate { get; set; }
        public long? DocumentId { get; set; }
        public string Reason { get; set; }
        public string Details { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public long? StatusId { get; set; }
        public short? SourceId { get; set; }
    }

    public class DocumentRequestListModel
    {
        public long DocumentRequestId { get; set; }
        public long? ProfileId { get; set; }
        public string RequestNo { get; set; }
        public DateTime RequestDate { get; set; }
        public string DocumentName { get; set; }
        public string Status { get; set; }
        public long? StatusId { get; set; }
        
        // Display properties
        public string RequestDateDisplay => RequestDate.ToString("MMM dd, yyyy");
    }

    public class DocumentTypeModel
    {
        public long DocumentTypeId { get; set; }
        public string DocumentName { get; set; }
        public int StatusId { get; set; }
        public int SourceTypeId { get; set; }
    }

    public class ReasonModel
    {
        public long ReasonPurposeId { get; set; }
        public string Text { get; set; }
    }

    public class DocumentListResponseWrapper
    {
        public List<DocumentRequestListModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
        public int TotalListCount { get; set; }
    }
    
    public class DocumentTypeListResponseWrapper
    {
        public List<DocumentTypeModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class ReasonListResponseWrapper
    {
        public List<ReasonModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
    }
}
