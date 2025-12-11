using MauiHybridApp.Models;
using MauiHybridApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class WorkflowDataService : IWorkflowDataService
    {
        private readonly IGenericRepository _repository;

        public WorkflowDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TransactionHistory>> GetTransactionHistoryAsync(long transactionTypeId, long transactionId)
        {
            // Simulate API
            await Task.Delay(300);

            // Mock History
            return new List<TransactionHistory>
            {
                new TransactionHistory 
                { 
                    ActionPastTense = "Filed", 
                    CreatedBy = "Muhammad Umer", 
                    LogDate = DateTime.Now.AddDays(-2), 
                    MessageTemplate = "<b>Muhammad Umer</b> filed the request.",
                    HistoryTypeId = 1, // Filed
                    ActionTypeId = 1
                },
                new TransactionHistory 
                { 
                    ActionPastTense = "Approved", 
                    CreatedBy = "Manager Name", 
                    LogDate = DateTime.Now.AddDays(-1), 
                    MessageTemplate = "<b>Manager Name</b> approved the request.",
                    HistoryTypeId = 2, // Approved
                    ActionTypeId = 2
                }
            };
        }
    }
}
