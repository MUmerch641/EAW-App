using System;
using System.Collections.Generic;

namespace MauiHybridApp.Models
{
    public class UndertimeRequestModel
    {
        public long UndertimeId { get; set; }
        public long? ProfileId { get; set; }
        public DateTime? UndertimeDate { get; set; }
        public short? UndertimeTypeId { get; set; }
        public string Reason { get; set; }
        public DateTime? DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public double UTHrs { get; set; }
        public long? StatusId { get; set; }
        public short? SourceId { get; set; }
    }

    public class UndertimeRequestListModel
    {
        public long UndertimeId { get; set; }
        public long? ProfileId { get; set; }
        public string RequestNo { get; set; }
        public DateTime UndertimeDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public long? StatusId { get; set; }
        public double UTHrs { get; set; }
        
        // Display properties
        public string UndertimeDateDisplay => UndertimeDate.ToString("MMM dd, yyyy");
        public string UTHrsDisplay => $"{UTHrs:F2} hrs";
    }

    public class UndertimeTypeModel
    {
        public long Id { get; set; }
        public string Value { get; set; }
    }

    public class UndertimeListResponseWrapper
    {
        public List<UndertimeRequestListModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
        public int TotalListCount { get; set; }
    }
    
    public class UndertimeTypeListResponseWrapper
    {
        // Depending on API, this might be a list of Enums or a specific wrapper
        // Based on Xamarin code: genericRepository_.GetAsync<List<R.Models.Enums>>
        // So we might not need a wrapper if we deserialize directly to List<UndertimeTypeModel>
        // But let's keep it flexible or use List<UndertimeTypeModel> directly in service
    }
}
