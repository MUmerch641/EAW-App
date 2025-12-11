using System.Collections.Generic;

namespace MauiHybridApp.Models;

public class LeaveApiResponse
{
    public bool IsSuccess { get; set; }
    public string? ValidationMessage { get; set; }
    public List<string>? ValidationMessages { get; set; }
    
    // ADDED: If this is not null, it means Success
    public object? Model { get; set; } 
}