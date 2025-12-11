using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
using System;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IUndertimeRequestDataService
    {
        Task<UndertimeHolder> InitForm(long recordId, DateTime? selectedDate);

        Task<UndertimeHolder> SubmitRequest(UndertimeHolder form);

        Task<UndertimeApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId);

        Task<UndertimeApprovalHolder> WorkflowTransaction(UndertimeApprovalHolder form);

        Task<UndertimeHolder> WorkflowTransactionRequest(UndertimeHolder form);
    }
}