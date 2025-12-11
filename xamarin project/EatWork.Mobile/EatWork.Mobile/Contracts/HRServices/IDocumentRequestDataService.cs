using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
using System;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IDocumentRequestDataService
    {
        Task<DocumentRequestHolder> InitForm(long recordId, DateTime? selectedDate);

        Task<DocumentRequestHolder> SubmitRequest(DocumentRequestHolder form);

        Task<DocumentRequestApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId);

        Task<DocumentRequestApprovalHolder> WorkflowTransaction(DocumentRequestApprovalHolder form);

        Task<DocumentRequestHolder> WorkflowTransactionRequest(DocumentRequestHolder form);
    }
}