namespace MauiHybridApp.Models;

public class ListResponse<T>
{
    public List<T> ListData { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}
