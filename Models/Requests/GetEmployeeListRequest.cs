using System;

namespace MauiHybridApp.Models.Requests
{
    public class GetEmployeeListRequest
    {
        public int Page { get; set; }
        public int Rows { get; set; }
        public int SortOrder { get; set; }
        public string Keyword { get; set; } = string.Empty;
        public long BranchId { get; set; }
        public long DepartmentId { get; set; }
        public long TeamId { get; set; }
    }
}
