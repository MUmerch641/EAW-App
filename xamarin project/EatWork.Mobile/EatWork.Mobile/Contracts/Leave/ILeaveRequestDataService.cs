using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface ILeaveRequestDataService
    {
        Task<LeaveRequestHolder> SubmitRequest(LeaveRequestHolder form);

        Task<LeaveRequestHolder> GetLeaveBalance(LeaveRequestHolder form);

        Task<LeaveRequestHolder> InitFormHelpers(long recordId);

        Task<LeaveApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId);

        Task<LeaveApprovalHolder> WorkflowTransaction(LeaveApprovalHolder form);

        Task<ObservableCollection<LeaveUsageListHolder>> InitLeaveUsage(long profileId, long leaveTypeId);

        Task<LeaveRequestHolder> WorkflowTransactionRequest(LeaveRequestHolder form);

        Task<LeaveRequestHolder> InitLeaveRequestForm(long recordId, DateTime? selectedDate);

        Task<LeaveRequestHolder> ValidateLeaveRequestGeneration(LeaveRequestHolder form);

        Task<LeaveRequestHolder> SubmitRequestEngine(LeaveRequestHolder form);

        Task<LeaveRequestHolder> GenerateAndSubmitRequest(LeaveRequestHolder form);
    }
}