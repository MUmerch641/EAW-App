using EatWork.Mobile.Models.FormHolder;
using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.TravelRequest;
using System;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface ITravelRequestDataService
    {
        Task<TravelRequestHolder> InitForm(long id, DateTime? selectedDate);

        Task<TravelRequestHolder> SubmitRecord(TravelRequestHolder holder);

        Task<TravelRequestHolder> RequestCancelRequest(TravelRequestHolder holder);

        Task<TravelRequestApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId);

        Task<TravelRequestApprovalHolder> WorkflowTransaction(TravelRequestApprovalHolder form);
    }
}