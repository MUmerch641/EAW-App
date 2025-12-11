using System;

namespace EatWork.Mobile.Models.Leave
{
    public class LeaveRequestEngineModel2
    {
        public long StatusId { get; set; }
        public DateTime dtpDateFiled { get; set; }
        public DateTime DateFiled { get; set; }
        public DateTime txtReasonForCancel { get; set; }
        public long UserAccessId { get; set; }
        public long WorkFlowButtonValue { get; set; }
        public bool hiddenContainWorkFlowRemarks { get; set; }
        public string txtWorkFlowRemarks { get; set; }
        public string txtReason { get; set; }
        public short NoOfDays { get; set; }
        public short chkPlanned { get; set; }
        public string chkPlanned_String { get; set; }
        public string txtWfRemarks { get; set; }
        public string LeaveRequestIds { get; set; }
        public decimal TotalApproveHours { get; set; }
        public decimal AvailableHours { get; set; }
        public bool DisplayInDays { get; set; }
        public string RandomId { get; set; }
        public decimal NoOfDaysDecimal { get; set; }
        public short cmbPartialDayApplyTo { get; set; }
        public string optPartialDayLeave_String { get; set; }
        public short optPartialDayLeave { get; set; }
        public long LeaveRequestId { get; set; }
        public long LeaveRequestHeaderId { get; set; }
        public long ProfileId { get; set; }
        public long CompanyId { get; set; }
        public string txtEmployeeName { get; set; }
        public string txtDepartment { get; set; }
        public string txtEmploymentStatus { get; set; }
        public string txtPosition { get; set; }
        public DateTime dtpHiredDate { get; set; }
        public DateTime dtpBirthDate { get; set; }
        public decimal txtRemainingHours { get; set; }
        public decimal NoOfHoursPerDay { get; set; }
        public decimal txtRemainingDays { get; set; }
        public long cmbLeaveTypeId { get; set; }
        public string LeaveType { get; set; }
        public DateTime dtpInclusiveStartDate { get; set; }
        public DateTime dtpInclusiveEndDate { get; set; }
        public decimal txtNoOfHours { get; set; }
        public decimal TotalNoOfHours { get; set; }
        public bool IsImport { get; set; }
        public decimal WorkingHoursPerDay { get; set; }
    }
}