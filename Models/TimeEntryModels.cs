namespace MauiHybridApp.Models;

// 1. Jab hum List mangwate hain (GET Response)
public class TimeEntryListResponse
{
    public List<TimeEntryLogItem> ListData { get; set; } = new();
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}

// 2. List ke andar ka aik record
public class TimeEntryLogItem
{
    public long TimeEntryLogId { get; set; }
    public long ProfileId { get; set; }
    public string? EmployeeName { get; set; }
    
    // Asli Time (In ya Out wala)
    public DateTime TimeEntry { get; set; } 
    
    // Type batayega ke ye "Time-In" hai ya "Time-Out"
    public string? Type { get; set; } 
    
    public string? Location { get; set; }
    public string? Latitude { get; set; } // API response mein string tha
    public string? Longitude { get; set; }
    public string? Status { get; set; } // Present, Late etc.
}

// 3. Jab hum Clock In/Out karte hain (POST Request)
public class TimeEntryRequest
{
    public TimeEntryRequest()
    {
        // Defaults
        TimeEntryLogId = 0;
        StatusId = 0;
        Source = "Mobile";
        Remarks = "Mobile Punch";
        TransactionDate = DateTime.UtcNow;
    }

    public long TimeEntryLogId { get; set; }
    public long ProfileId { get; set; }
    public DateTime TransactionDate { get; set; }
    
    public string? Type { get; set; } // "Time-In" or "Time-Out"
    
    // 🔥 FIX: Lat/Long ko String rakho (Swagger ke hisab se)
    public string Latitude { get; set; } = "0";
    public string Longitude { get; set; } = "0";
    
    public string Source { get; set; }
    public string Remarks { get; set; }
    public long StatusId { get; set; }
}
