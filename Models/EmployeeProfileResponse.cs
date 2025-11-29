namespace MauiHybridApp.Models;
using MauiHybridApp.Models.Employee; // ProfileModel yahan hai


public class EmployeeProfileResponse
{
    // Ye wohi property hai jo Auth service dhoond rahi hai

    public long ProfileId { get; set; }
        public ProfileModel? Model { get; set; } 

    
    // Agar API kuch aur bhi bhejti hai to add kar sakte hain, 
    // lekin Auth ke liye sirf ProfileId zaroori hai.
    public string? EmployeeNo { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}