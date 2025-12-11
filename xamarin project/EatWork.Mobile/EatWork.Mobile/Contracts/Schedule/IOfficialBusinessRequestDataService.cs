using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
using System;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Contracts
{
    public interface IOfficialBusinessRequestDataService
    {
        Task<OfficialBusinessHolder> SubmitRequest(OfficialBusinessHolder form);

        Task<OfficialBusinessHolder> InitForm(long recordId, int obTypeId, DateTime? selectedDate);

        Task<OfficialBusinessHolder> WorkflowTransactionRequest(OfficialBusinessHolder form);

        Task<OfficialBusinessApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId);

        Task<OfficialBusinessApprovalHolder> WorkflowTransaction(OfficialBusinessApprovalHolder form);

        Task<OfficialBusinessHolder> SaveRecord(OfficialBusinessHolder form, R.Requests.SubmitOfficialBusinessRequest request);
    }
}