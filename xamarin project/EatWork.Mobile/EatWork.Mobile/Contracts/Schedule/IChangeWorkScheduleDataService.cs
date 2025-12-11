using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
using System;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IChangeWorkScheduleDataService
    {
        Task<ChangeWorkScheduleHolder> InitForm(long recordId, DateTime? selectedDate);

        Task<ChangeWorkScheduleHolder> SubmitRequest(ChangeWorkScheduleHolder form);

        Task<ChangeWorkScheduleHolder> GetEmployeeSchedule(ChangeWorkScheduleHolder form, int option);

        Task<ChangeWorkScheduleApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId);

        Task<ChangeWorkScheduleApprovalHolder> WorkflowTransaction(ChangeWorkScheduleApprovalHolder form);
        Task<ChangeWorkScheduleHolder> WorkflowTransactionRequest(ChangeWorkScheduleHolder form);
    }
}