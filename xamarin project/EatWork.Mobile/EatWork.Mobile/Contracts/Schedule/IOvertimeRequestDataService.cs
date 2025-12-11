using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
using System;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IOvertimeRequestDataService
    {
        Task<OvertimeRequestHolder> SubmitRequest(OvertimeRequestHolder form);

        Task<OvertimeRequestHolder> InitForm(long recordId, DateTime? selectedDate);

        Task<OvertimeRequestHolder> PreOTValidation(OvertimeRequestHolder form);

        Task<OvertimeRequestHolder> WorkflowTransactionRequest(OvertimeRequestHolder form);

        Task<OvertimeApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId);

        Task<OvertimeApprovalHolder> WorkflowTransaction(OvertimeApprovalHolder form);
    }
}