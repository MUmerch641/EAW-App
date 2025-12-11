using Newtonsoft.Json;

namespace MauiHybridApp.Models;

public class PaginatedResponse<T>
{
    [JsonProperty("data")]
    public List<T> Data { get; set; } = new List<T>();

    [JsonProperty("totalPages")] 
    public int TotalPages { get; set; }

    [JsonProperty("totalCount")]
    public int TotalCount { get; set; }

    [JsonProperty("pageNumber")]
    public int PageNumber { get; set; }

    [JsonProperty("pageSize")]
    public int PageSize { get; set; }
    
    // Fallback property mapping if needed
    public bool IsSuccess => Data != null;
}
