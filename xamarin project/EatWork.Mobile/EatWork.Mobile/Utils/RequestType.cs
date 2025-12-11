using EatWork.Mobile.Contants;
using EatWork.Mobile.Views.Approvals;
using EatWork.Mobile.Views.Requests;
using EatWork.Mobile.Views.Shared;
using EatWork.Mobile.Views.TravelRequest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EatWork.Mobile.Utils
{
    public class RequestType
    {
        private List<RequestTypeModel> requetTypeList_;

        public List<RequestTypeModel> RequetTypeList
        {
            get { return requetTypeList_; }
            set { requetTypeList_ = value; }
        }

        public RequestType()
        {
            RequestTypeList();
        }

        public async Task<RequestTypeModel> GetPageByRequestType(long requestTypeId)
        {
            var retValue = new RequestTypeModel();

            await Task.Run(() =>
            {
                retValue = RequetTypeList.Find(p => p.RequestTypeId == requestTypeId);
            });

            return retValue;
        }

        private void RequestTypeList()
        {
            requetTypeList_ = new List<RequestTypeModel>()
            {
                new RequestTypeModel(){RequestTypeId = TransactionType.Leave, RequestPage =  typeof(LeaveRequestPage), ApprovalPage = typeof(LeaveRequestApprovalPage), Title = "Leave Request", IsVisible = 1 },
                new RequestTypeModel(){RequestTypeId = TransactionType.Undertime, RequestPage =  typeof(UndertimeRequestPage), ApprovalPage = typeof(UndertimeApprovalPage), Title = "Undertime Request", IsVisible = 1 },
                new RequestTypeModel(){RequestTypeId = TransactionType.Overtime, RequestPage =  typeof(OvertimeRequestPage), ApprovalPage = typeof(OvertimeApprovalPage), Title = "Overtime Request", IsVisible = 1 },
                new RequestTypeModel(){RequestTypeId = TransactionType.TimeOff, RequestPage =  typeof(TimeOffRequestPage), ApprovalPage = typeof(TimeOffApprovalPage), Title = "Time-off Request", IsVisible = 1 },
                new RequestTypeModel(){RequestTypeId = TransactionType.OfficialBusiness, RequestPage =  typeof(OfficialBusinessRequestPage), ApprovalPage = typeof(OfficialBusinessApprovalPage), Title = "Official Business Request", IsVisible = 1 },
                new RequestTypeModel(){RequestTypeId = TransactionType.ChangeWorkSchedule, RequestPage =  typeof(ChangeWorkSchedulePage), ApprovalPage = typeof(ChangeWorkSchedApprovalPage), Title = "Change Work Schedule Request", IsVisible = 1 },
                new RequestTypeModel(){RequestTypeId = TransactionType.ExtendedTimeandOffset, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Extended Time and Offset Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.TimeLog, RequestPage =  typeof(TimeEntryRequestPage), ApprovalPage = typeof(TimeEntryApprovalPage), Title = "Time Entry Request", IsVisible = 1 },
                new RequestTypeModel(){RequestTypeId = TransactionType.Manpower, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Manpower Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.Training, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Training Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.Document, RequestPage =  typeof(DocumentRequestPage), ApprovalPage = typeof(DocumentApprovalPage), Title = "Document Request", IsVisible = 1 },
                new RequestTypeModel(){RequestTypeId = TransactionType.ProfileUpdate, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Profile Update Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.AttendanceReview, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Attendance Review Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.PayrollDeduction, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Payroll Deduction Request", IsVisible = 0},
                new RequestTypeModel(){RequestTypeId = TransactionType.Loan, RequestPage =  typeof(LoanRequestPage), ApprovalPage = typeof(LoanApprovalPage), Title = "Loan Request", IsVisible = 1 },
                new RequestTypeModel(){RequestTypeId = TransactionType.PayrollReview, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Payroll Review Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.CashAdvance, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Cash Advance Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.ExpenseReport, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Expense Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.Item, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Item Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.FlexibleBenefit, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Flexible Benefit Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.ProjectTimeEntry, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Project Time Entry Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.ChangeRestDay, RequestPage =  typeof(ChangeRestdayPage), ApprovalPage = typeof(ChangeRestdaySchedApprovalPage), Title = "Change Restday Request", IsVisible = 1 },
                new RequestTypeModel(){RequestTypeId = TransactionType.ProvidentFundEnrollee, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Provident Fund Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.Onboarding, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Onboarding Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.Offboarding, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Offboarding Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.LegalCases, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Legal Cases Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.Violation, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Violation Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.EmployeeAssigment, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Employee Assignment Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.PerformanceAppraisalReview, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Performance Appraisal Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.IndividualDevelopmentPlan, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage) , Title = "Individual Development Plan Request", IsVisible = 0},
                new RequestTypeModel(){RequestTypeId = TransactionType.MedicalConditionandClaimsReport, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Medical Condition and Claims Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.WorkScheduleRequest, RequestPage =  typeof(SpecialWorkSchedulePage), ApprovalPage = typeof(SpecialWorkScheduleApprovalPage), Title = "Special Work Schedule Request", IsVisible = 1 },
                new RequestTypeModel(){RequestTypeId = TransactionType.JobOffer, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Job Offer Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.AllowanceOtherEarnings, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Allowance and Other Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.OtherDeductions, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Other Deductions Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.NetPayBreakdown, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Netpay Breakdown Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.EmployeeOnhold, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Employee Onhold Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.BankFile, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Bankfile Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.Travel, RequestPage =  typeof(TravelRequestFormPage), ApprovalPage = typeof(TravelRequestApprovalPage), Title = "Travel Request", IsVisible = 1 },
                new RequestTypeModel(){RequestTypeId = TransactionType.SalaryBatchUpdate, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Salary Batch Update Request", IsVisible = 0 },
                new RequestTypeModel(){RequestTypeId = TransactionType.BenefitIssuance, RequestPage =  typeof(ComingSoonPage), ApprovalPage = typeof(ComingSoonPage), Title = "Benefit Issuance Request", IsVisible = 0 },
            };
        }
    }

    public class RequestTypeModel
    {
        public long RequestTypeId { get; set; }
        public string Title { get; set; }
        public Type RequestPage { get; set; }
        public Type ApprovalPage { get; set; }
        public int IsVisible { get; set; }
    }
}