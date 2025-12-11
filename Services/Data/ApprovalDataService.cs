using MauiHybridApp.Models;
using MauiHybridApp.Models.Workflow;
using MauiHybridApp.Utils;
using MauiHybridApp.Services.Data;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class ApprovalDataService : IApprovalDataService
    {
        private readonly IGenericRepository _repository;

        public ApprovalDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        // 1. LIST MANGWANA
        public async Task<List<MyApprovalListModel>> GetMyApprovalsAsync()
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);

                var request = new MyApprovalRequest
                {
                    ProfileId = pid,
                    Page = 1,
                    Rows = 100,
                    SortOrder = 0,
                    Status = "",
                    Keyword = "",
                    TransactionTypes = ""
                };

                // API Call
                var response = await _repository.PostAsync<MyApprovalRequest, ApprovalApiResponse>(ApiEndpoints.GetMyApprovals, request);

                if (response != null && response.ListData != null)
                {
                    // 🔥 MAPPING: API Data -> UI Model
                    return response.ListData.Select(x => new MyApprovalListModel
                    {
                        // IDs
                        RequestId = x.TransactionId, // UI uses RequestId
                        TransactionId = x.TransactionId,
                        TransactionTypeId = x.TransactionTypeId,
                        ProfileId = x.ProfileId,

                        // Text Info
                        RequestType = x.TransactionType, // UI uses RequestType
                        RequesterName = x.EmployeeName,  // UI uses RequesterName
                        Details = x.Details,
                        Status = x.Status,
                        
                        // Dates
                        DateFiled = x.DateFiled,
                        RequestDate = x.DateFiled,
                        
                        // Extra Parsing
                        StartDate = DateTime.TryParse(x.RequestedDate, out var d) ? d : null,
                        Reason = x.Details // Details ko hi Reason bana diya
                    }).ToList();
                }

                return new List<MyApprovalListModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Approvals Error: {ex.Message}");
                return new List<MyApprovalListModel>();
            }
        }

        // 2. APPROVE
        public async Task<SaveResult> ApproveRequestAsync(long requestId, string comments)
        {
            // ActionTriggeredId = 1 (Usually Approve)
            return await ProcessTransaction(requestId, 1, comments);
        }

        // 3. DISAPPROVE
        public async Task<SaveResult> DisapproveRequestAsync(long requestId, string comments)
        {
            // ActionTriggeredId = 3 (Usually Disapprove/Reject)
            return await ProcessTransaction(requestId, 3, comments);
        }

        // --- Helper: Process Workflow ---
        private async Task<SaveResult> ProcessTransaction(long transactionId, int actionId, string remarks)
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long approverId);

                // Swagger Payload for Process
                var payload = new
                {
                    transactionId = transactionId,
                    actionTriggeredId = actionId, // 1=Approve, 3=Disapprove
                    approverId = approverId,
                    remarks = remarks,
                    
                    // Defaults required by API
                    transactionTypeId = 0, // Server usually finds this by ID
                    stageId = 0,
                    userAccessId = 0
                };

                // API Call
                var response = await _repository.PostAsync<object, LeaveApiResponse>(ApiEndpoints.ProcessWorkflow, payload);

                if (response != null)
                {
                    if (response.IsSuccess)
                        return new SaveResult { Success = true };
                    else
                        return new SaveResult { Success = false, ErrorMessage = response.ValidationMessage ?? "Action failed." };
                }

                return new SaveResult { Success = false, ErrorMessage = "Server returned no response" };
            }
            catch (Exception ex)
            {
                return new SaveResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        // --- Interface Stub (Not used but required by Interface) ---
        public Task<List<MyApprovalListModel>> GetPendingApprovalsAsync() => GetMyApprovalsAsync();
        public Task<MyApprovalListModel?> GetApprovalByIdAsync(long id) => Task.FromResult<MyApprovalListModel?>(null);
    }

    // --- Helper Class for JSON Response ---
    public class ApprovalApiResponse
    {
        public List<MyApprovalListModel>? ListData { get; set; }
        public bool IsSuccess { get; set; }
    }
}
