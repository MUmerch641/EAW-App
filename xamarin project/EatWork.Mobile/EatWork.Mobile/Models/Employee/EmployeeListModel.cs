using System;

namespace EatWork.Mobile.Models
{
    public class EmployeeListModel
    {
        public EmployeeListModel()
        {
            ImageSource = string.Empty;
        }

        public long ProfileId { get; set; }
        public string EmployeeNo { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string EmployeeName { get; set; }
        public string FullAddress { get; set; }
        public string Department { get; set; }
        public string Branch { get; set; }
        public string Position { get; set; }
        public long DepartmentId { get; set; }
        public long BranchId { get; set; }
        public long PositionId { get; set; }
        public long UserAccountId { get; set; }
        public string Username { get; set; }
        public DateTime HiredDate { get; set; }
        public DateTime RegularizationDate { get; set; }
        public string EmailAddress { get; set; }
        public DateTime BirthDate { get; set; }
        public long EmploymentTypeId { get; set; }
        public string EmploymentType { get; set; }
        public long JobLevelId { get; set; }
        public string JobLevel { get; set; }
        public long CompanyId { get; set; }
        public string Company { get; set; }
        public long JobGradeId { get; set; }
        public string JobGrade { get; set; }
        public string ImageSource { get; set; }
    }
}