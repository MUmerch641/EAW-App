using System;
using System.Collections.Generic;
using System.Text;

namespace EatWork.Mobile.Models
{
    public class EmploymentInformationModel
    {
        public long EmploymentInformationId { get; set; }
        public long? ProfileId { get; set; }
        public long? CJI_EmployeeType { get; set; }
        public long? CJI_EmploymentStatus { get; set; }
        public long? CJI_DueTo { get; set; }
        public string CJI_EmployeeNo { get; set; }
        public string CJI_AccessId { get; set; }
        public long? CJI_JobLevel { get; set; }
        public long? CJI_JobGrade { get; set; }
        public long? CJI_JobRank { get; set; }
        public long? CJI_Position { get; set; }
        public long? CJI_ManpowerClassification { get; set; }
        public long? CJI_ManHourClassification { get; set; }
        public long? AOA_Company { get; set; }
        public long? AOA_Branch { get; set; }
        public long? AOA_Department { get; set; }
        public long? AOA_Office { get; set; }
        public long? AOA_Unit { get; set; }
        public long? AOA_District { get; set; }
        public long? AOA_Location { get; set; }
        public long? AOA_Project { get; set; }
        public long? AOA_CostCenter { get; set; }
        public long? AOA_Division { get; set; }
        public long? AOA_Groups { get; set; }
        public long? AOA_Team { get; set; }
        public long? AOA_Line { get; set; }
        public DateTime? RED_HireDate { get; set; }
        public DateTime? RED_ReglarizationDate { get; set; }
        public DateTime? RED_EndOfContractDate { get; set; }
        public DateTime? RED_SeparationDate { get; set; }
        public DateTime? RED_ClearanceDate { get; set; }
        public long? RED_Reason { get; set; }
        public string RED_Others { get; set; }
        public long? ChargeCodeId { get; set; }
        public bool? UnionMember { get; set; }
    }
}
