namespace MauiHybridApp.Models;
using MauiHybridApp.Models.Employee; // ProfileModel is here


public class EmployeeProfileResponse
{
    // This is the property Auth service is looking for

    public long ProfileId { get; set; }
        public ProfileModel? Model { get; set; } 

    
    // If API sends something else, we can add it,
    // but only ProfileId is necessary for Auth.
    public string? EmployeeNo { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}