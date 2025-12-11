using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
using System;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IChangeRestdayScheduleDataService
    {
        Task<ChangeRestdayHolder> InitForm(long recordId, DateTime? selectedDate);

        Task<ChangeRestdayHolder> SubmitRequest(ChangeRestdayHolder form);

        Task<ChangeRestdayHolder> GetEmployeeSchedule(ChangeRestdayHolder form);

        Task<ChangeRestdayScheduleApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId);

        Task<ChangeRestdayScheduleApprovalHolder> WorkflowTransaction(ChangeRestdayScheduleApprovalHolder form);

        Task<ChangeRestdayHolder> WorkflowTransactionRequest(ChangeRestdayHolder form);
    }
}