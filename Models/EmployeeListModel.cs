using System;

namespace MauiHybridApp.Models
{
    public class EmployeeListModel
    {
        public EmployeeListModel()
        {
            ImageSource = string.Empty;
        }

        public long ProfileId { get; set; }
        public string EmployeeNo { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public long? DepartmentId { get; set; }
        public long? BranchId { get; set; }
        public long? PositionId { get; set; }
        public long? UserAccountId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime? HiredDate { get; set; }
        public DateTime? RegularizationDate { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public long? EmploymentTypeId { get; set; }
        public string EmploymentType { get; set; } = string.Empty;
        public long? JobLevelId { get; set; }
        public string JobLevel { get; set; } = string.Empty;
        public long? CompanyId { get; set; }
        public string Company { get; set; } = string.Empty;
        public long? JobGradeId { get; set; }
        public string JobGrade { get; set; } = string.Empty;
        public string ImageSource { get; set; }
    }
}
