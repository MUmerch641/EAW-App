using MauiHybridApp.Models;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Utils;
using MauiHybridApp.Services.Data;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class ExpenseDataService : IExpenseDataService
    {
        private readonly IGenericRepository _repository;

        public ExpenseDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        // 1. LIST MANGWANA
        public async Task<List<ExpenseModel>> GetExpensesAsync()
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                
                // Swagger: GET api/v1/expense/list?ProfileId=...
                var url = $"{ApiEndpoints.GetExpenseList}?ProfileId={profileIdStr}&Page=1&Rows=50&SortOrder=1";
                
                // Response Wrapper (List + IsSuccess)
                var response = await _repository.GetAsync<ExpenseListResponseWrapper>(url);
                
                return response?.ListData ?? new List<ExpenseModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Expense List Error: {ex.Message}");
                return new List<ExpenseModel>();
            }
        }

        // 2. SUBMIT EXPENSE
        public async Task<SaveResult> SubmitExpenseAsync(ExpenseModel request)
        {
            try
            {
                // Profile ID Fix
                if (request.ProfileId == 0)
                {
                    var pid = await SecureStorage.GetAsync("profile_id");
                    if (long.TryParse(pid, out long id)) request.ProfileId = id;
                }

                request.DateFiled = DateTime.UtcNow;
                request.ExpenseDate = request.ExpenseDate.Date;

                // Construct payload matching Swagger: SubmitAppExpenseReportDetailRequest
                // The schema expects: { data: { profileId, expenseSetupId, expenseDate, amount, remarks, ... } }
                var payload = new 
                { 
                    data = new
                    {
                        profileId = request.ProfileId,
                        expenseSetupId = request.ExpenseSetupId,
                        expenseDate = request.ExpenseDate.ToString("yyyy-MM-dd"),
                        dateFiled = request.DateFiled.ToString("yyyy-MM-ddTHH:mm:ss"),
                        amount = request.Amount,
                        remarks = request.Remarks,
                        merchant = request.Merchant,
                        statusId = request.StatusId
                    }
                };

                // Use generic BaseResponse since actual response type varies
                var response = await _repository.PostAsync<object, ExpenseApiResponse>(ApiEndpoints.CreateExpense, payload);

                if (response != null)
                {
                    // Check IsSuccess or if Model exists
                    if (response.IsSuccess || response.Model != null || response.ExpenseReportDetailId > 0)
                        return new SaveResult { Success = true };
                    else
                        return new SaveResult { Success = false, ErrorMessage = response.ValidationMessage ?? response.ErrorMessage ?? "Submission failed." };
                }

                // Null response - might be HTTP error
                Console.WriteLine("Expense Submit: Response was null");
                return new SaveResult { Success = false, ErrorMessage = "Server returned no response. Please check connection." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Expense Submit Error: {ex.Message}");
                return new SaveResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        // Expense API Response matching AppExpenseReportDetailBaseResponse
        public class ExpenseApiResponse
        {
            public bool IsSuccess { get; set; }
            public ExpenseModel Model { get; set; }
            public long ExpenseReportDetailId { get; set; }
            public string ValidationMessage { get; set; }
            public string ErrorMessage { get; set; }
            public List<string> ValidationMessages { get; set; }
        }

        // 3. GET TYPES (Dropdown)
        public async Task<List<SelectableListModel>> GetExpenseTypesAsync()
        {
            try
            {
                // Swagger: GET api/v1/expense/expense-setup-list
                // Try Wrapper first
                try 
                {
                    var response = await _repository.GetAsync<PaginatedResponse<SelectableListModel>>(ApiEndpoints.GetExpenseSetup);
                    if (response != null && response.Data != null && response.Data.Any())
                        return response.Data;
                }
                catch { /* Ignore, duplicate list call below */ }

                var listResponse = await _repository.GetAsync<List<SelectableListModel>>(ApiEndpoints.GetExpenseSetup);
                return listResponse ?? new List<SelectableListModel>();
            }
            catch
            {
                return new List<SelectableListModel>();
            }
        }
    }

    // Helper Class for List Response
    public class ExpenseListResponseWrapper
    {
        public List<ExpenseModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
    }
}
