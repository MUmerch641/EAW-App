namespace MauiHybridApp.Models.Employee;

public class ProfileModel
{
    public long ProfileId { get; set; }

    
    // --- Missing Fields Added (That were giving errors) ---
    public string? EmployeeNo { get; set; } 
    public string? Position { get; set; } 
    public string? Department { get; set; }
    public string? EmploymentStatus { get; set; }
    public string? Location { get; set; }
    public DateTime? HireDate { get; set; }
    public string? Branch { get; set; } // Fallback for location
    
    // --- Standard Fields ---
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    
    public DateTime? Birthdate { get; set; }
    public string? EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
    public string? MobileNumber { get; set; }
    
    // Address
    public string? CityAddress1 { get; set; }
    public string? CityAddressCity { get; set; }
    
    // Emergency
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactRelationship { get; set; }
    public string? EmergencyContactContactNumber { get; set; }
    
    // Others
    public string? ProfilePhotoUrl { get; set; } // If API sends it
}