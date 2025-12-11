namespace MauiHybridApp.Models.DataObjects;

public class ListParam
{
    public int ListCount { get; set; }
    public int Count { get; set; }
    public bool IsAscending { get; set; }
    public string KeyWord { get; set; }
    public string FilterTypes { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string Status { get; set; }
}
