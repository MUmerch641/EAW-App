using MauiHybridApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public interface IWorkflowDataService
    {
        Task<List<TransactionHistory>> GetTransactionHistoryAsync(long transactionTypeId, long transactionId);
    }
}
