using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
using System;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface ISpecialWorkScheduleDataService
    {
        Task<SpecialWorkScheduleHolder> InitForm(long recordId, DateTime? selectedDate);

        Task<SpecialWorkScheduleHolder> SubmitRequest(SpecialWorkScheduleHolder form);

        Task<SpecialWorkScheduleHolder> GetShiftSchedule(SpecialWorkScheduleHolder form);

        Task<SpecialWorkScheduleApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId);

        Task<SpecialWorkScheduleApprovalHolder> WorkflowTransaction(SpecialWorkScheduleApprovalHolder form);

        Task<SpecialWorkScheduleHolder> WorkflowTransactionRequest(SpecialWorkScheduleHolder form);

        Task<SpecialWorkScheduleHolder> GetDateSchedule(SpecialWorkScheduleHolder form);
    }
}