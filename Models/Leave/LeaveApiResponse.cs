using System.Collections.Generic;

namespace MauiHybridApp.Models;

public class LeaveApiResponse
{
    public bool IsSuccess { get; set; }
    public string? ValidationMessage { get; set; }
    public List<string>? ValidationMessages { get; set; }
    
    // 🔥 YE ADD KARO: Agar ye null nahi hai, matlab Success hai
    public object? Model { get; set; } 
}