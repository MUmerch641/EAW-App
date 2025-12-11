using System;
using System.Collections.Generic;

namespace MauiHybridApp.Models
{
    public class TravelRequestModel
    {
        public long TravelRequestId { get; set; }
        public long? ProfileId { get; set; }
        public DateTime? RequestDate { get; set; }
        public long? TypeOfBusinessTrip { get; set; }
        public string Details { get; set; }
        public string FirstOrigin { get; set; }
        public string FirstDestination { get; set; }
        public DateTime? FirstDepartureDate { get; set; }
        public DateTime? FirstDepartureTime { get; set; }
        public string SecondOrigin { get; set; }
        public string SecondDestination { get; set; }
        public DateTime? SecondDepartureDate { get; set; }
        public DateTime? SecondDepartureTime { get; set; }
        public string Reason { get; set; }
        public long? StatusId { get; set; }
        public short? SourceId { get; set; }
    }

    public class TravelRequestListModel
    {
        public long TravelRequestId { get; set; }
        public long? ProfileId { get; set; }
        public string RequestNo { get; set; }
        public DateTime RequestDate { get; set; }
        public string TripType { get; set; }
        public string Destination { get; set; }
        public string Status { get; set; }
        public long? StatusId { get; set; }
        
        // Display properties
        public string RequestDateDisplay => RequestDate.ToString("MMM dd, yyyy");
    }

    public class TravelInitModel
    {
        public List<string> Origins { get; set; }
        public List<string> Destinations { get; set; }
        public long FARModuleFormId { get; set; }
    }

    public class TripTypeModel
    {
        public long Id { get; set; }
        public string Value { get; set; }
    }

    public class TravelListResponseWrapper
    {
        public List<TravelRequestListModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
        public int TotalListCount { get; set; }
    }
}
