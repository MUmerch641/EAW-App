using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EAW.API.DataContracts.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IWorkflowDataService
    {
        Task<GetWorkflowDetailsResponse> GetWorkflowDetails(long transactionId, long TransactionTypeId);

        Task<WFTransactionResponse> ProcessWorkflowByRecordId(long TransactionId, long TransactionTypeId, long actionType, string remarks = "", long stageId = 0, string formData = "");

        Task<WFTransactionResponse> CancelWorkFlowByRecordId(long TransactionId, long TransactionTypeId, long actionType, string remarks = "", long stageId = 0, string formData = "", bool hasApproverId = true);

        Task<WFResponseMessage> ConfirmationMessage(long actionType, string msg);

        Task<List<TransactionHistory>> GetTransactionHistory(long TransactionTypeId, long transactionId);

        Task<bool> UpdateWFSourceId(string url, long transactionId);
    }
}