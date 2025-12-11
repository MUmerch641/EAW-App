using System;
using System.Collections.Generic;

namespace MauiHybridApp.Models
{
    public class SuggestionModel
    {
        public long SuggestionId { get; set; }
        public long? ProfileId { get; set; }
        public string Detail { get; set; }
        public long SuggestionCategoryId { get; set; }
        public short SourceId { get; set; }
    }

    public class SuggestionListModel
    {
        public long SuggestionId { get; set; }
        public long? ProfileId { get; set; }
        public string Category { get; set; }
        public string SuggestionDetail { get; set; }
        public DateTime? CreateDate { get; set; }
        public string EmployeeName { get; set; }
        
        // Display properties
        public string CreateDateDisplay => CreateDate?.ToString("MMM dd, yyyy") ?? "";
    }

    public class SuggestionCategoryModel
    {
        public long SuggestionCategoryId { get; set; }
        public string Name { get; set; }
    }

    public class SuggestionListResponseWrapper
    {
        public List<SuggestionListModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
        public int TotalListCount { get; set; }
    }
}
