using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
using System;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface ILoanRequestDataService
    {
        Task<LoanRequestHolder> InitForm(long recordId, DateTime? selectedDate);

        Task<LoanRequestHolder> SubmitRequest(LoanRequestHolder form);

        Task<LoanRequestApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId);

        Task<LoanRequestApprovalHolder> WorkflowTransaction(LoanRequestApprovalHolder form);

        Task<LoanRequestHolder> WorkflowTransactionRequest(LoanRequestHolder form);
    }
}