namespace MauiHybridApp.Models;

public class MyApprovalRequest
{
    public long ProfileId { get; set; }
    public int Page { get; set; }
    public int Rows { get; set; }
    public int SortOrder { get; set; }
    public string Keyword { get; set; } = string.Empty;
    public string TransactionTypes { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
}
