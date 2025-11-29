using MauiHybridApp.Utils;

namespace MauiHybridApp.Models;

public class UserModel
{
    public UserModel()
    {
        BirthDate = Constants.NullDate;
        HireDate = Constants.NullDate;
    }

    public long ProfileId { get; set; }
    public long UserSecurityId { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeNo { get; set; }
    public string? Department { get; set; }
    public string? Company { get; set; }
    public string? Branch { get; set; }
    public string? Position { get; set; }
    public string? EmailAddress { get; set; }
    public long RoleId { get; set; }
    public long UserTypeId { get; set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public long DepartmentId { get; set; }
    public long PositionId { get; set; }
    public string? AccessId { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime HireDate { get; set; }
    public long OfficeId { get; set; }
    public long TeamId { get; set; }
    public long JobRankId { get; set; }
    public long JobGradeId { get; set; }
    public long JobLevelId { get; set; }
    public string? ProfilePicture { get; set; }
    
    public string FullName 
    { 
        get 
        {
            var name = $"{FirstName} {MiddleName} {LastName}".Trim();
            return !string.IsNullOrEmpty(name) ? name : (EmployeeName ?? Username ?? "User");
        }
    }
}

