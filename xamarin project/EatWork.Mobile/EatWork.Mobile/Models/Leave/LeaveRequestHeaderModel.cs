using EatWork.Mobile.Contants;
using System;
using System.Collections.Generic;

namespace EatWork.Mobile.Models.Leave
{
    public class LeaveRequestHeaderModel
    {
        public LeaveRequestHeaderModel()
        {
            WorkScheduleModelList = new List<EmployeeWorkScheduleModel>();
            LeaveRequestModel = new LeaveRequestEngineModel();
            LeaveDocumentModel = new List<LeaveRequestDocumentModel>();
            LeaveDetail = new List<LeaveRequestEngineModel>();
            config = new LeaveCompanyConfiguration();
            SourceId = (short)SourceEnum.Mobile;
        }

        public List<EmployeeWorkScheduleModel> WorkScheduleModelList { get; set; }
        public LeaveRequestEngineModel LeaveRequestModel { get; set; }
        public List<LeaveRequestDocumentModel> LeaveDocumentModel { get; set; }
        public List<LeaveRequestEngineModel> LeaveDetail { get; set; }
        public LeaveCompanyConfiguration config { get; set; }

        public List<long> ForDeletionIds { get; set; }
        public string LeaveRequestIds { get; set; }
        public bool IsBatchLeave { get; set; }
        public long RowLeaveRequestDocumentId { get; set; }
        public decimal AvailableHours { get; set; }
        public short SourceId { get; set; }
        public decimal TotalApproveHours { get; set; }
        public decimal NoOfDays { get; set; }
        public long StatusId { get; set; }
        public string txtReason { get; set; }
        public short cmbPartialDayApplyTo { get; set; }
        public short optPartialDayLeave { get; set; }
        public decimal txtRemainingHours { get; set; }
        public decimal TotalNoOfHours { get; set; }
        public DateTime dtpInclusiveEndDate { get; set; }
        public DateTime dtpInclusiveStartDate { get; set; }
        public DateTime dtpDateFiled { get; set; }
        public long cmbLeaveTypeId { get; set; }
        public long ProfileId { get; set; }
        public long LeaveRequestHeaderId { get; set; }
        public bool FollowStandardWorkingHoursPerDayforAbsencesandLeaves { get; set; }
        public decimal StandardWorkingHoursPerDay { get; set; }
    }
}