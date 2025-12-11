using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
using System;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface ITimeEntryRequestDataService
    {
        Task<TimeEntryHolder> SubmitRequest(TimeEntryHolder form);

        Task<TimeEntryHolder> InitForm(long recordId, DateTime? selectedDate);

        Task<TimeEntryHolder> WorkflowTransactionRequest(TimeEntryHolder form);

        Task<TimeEntryApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId);

        Task<TimeEntryApprovalHolder> WorkflowTransaction(TimeEntryApprovalHolder form);
    }
}