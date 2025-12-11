using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class FinancialDataService : IFinancialDataService
    {
        private readonly IGenericRepository _repository;

        public FinancialDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        // CASH ADVANCE
        public async Task<List<CashAdvanceModel>> GetCashAdvancesAsync()
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);

                var url = $"{ApiEndpoints.BaseApiUrl}/api/cash-advance/list?ProfileId={pid}&Page=1&Rows=100&SortOrder=0";
                var response = await _repository.GetAsync<FinancialListResponseWrapper<CashAdvanceModel>>(url);
                return response?.ListData ?? new List<CashAdvanceModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCashAdvancesAsync Error: {ex.Message}");
                return new List<CashAdvanceModel>();
            }
        }

        public async Task<CashAdvanceModel> GetCashAdvanceByIdAsync(long id)
        {
            // Stub for now, can implement detail fetch if needed
            return new CashAdvanceModel();
        }

        public async Task<bool> SubmitCashAdvanceAsync(CashAdvanceModel request)
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);
                request.ProfileId = pid;
                request.RequestedDate = DateTime.Now;

                var payload = new { data = request };
                var response = await _repository.PostAsync<object, LeaveApiResponse>($"{ApiEndpoints.BaseApiUrl}/api/cash-advance", payload);

                return response != null && (response.IsSuccess || response.Model != null || string.IsNullOrEmpty(response.ValidationMessage));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SubmitCashAdvanceAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<CostCenterModel>> GetCostCentersAsync()
        {
            try
            {
                var url = $"{ApiEndpoints.BaseApiUrl}/api/costcenter/list";
                
                // Try Wrapper first
                try 
                {
                    // Reusing FinancialListResponseWrapper since it is defined in this file
                    var response = await _repository.GetAsync<FinancialListResponseWrapper<CostCenterModel>>(url);
                    if (response != null && response.ListData != null)
                        return response.ListData;
                }
                catch { }

                var listResponse = await _repository.GetAsync<List<CostCenterModel>>(url);
                return listResponse ?? new List<CostCenterModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCostCentersAsync Error: {ex.Message}");
                return new List<CostCenterModel>();
            }
        }

        // LOANS
        public async Task<List<LoanRequestModel>> GetLoansAsync()
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);

                // Using the endpoint structure from Xamarin: api/loanrequest/list (inferred) or similar
                // Xamarin uses generic list request, let's try standard pattern
                var url = $"{ApiEndpoints.BaseApiUrl}/api/loanrequest/list?ProfileId={pid}&Page=1&Rows=100&SortOrder=0";
                
                // Note: Xamarin uses generic ListResponse, we'll try our wrapper
                var response = await _repository.GetAsync<FinancialListResponseWrapper<LoanRequestModel>>(url);
                return response?.ListData ?? new List<LoanRequestModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetLoansAsync Error: {ex.Message}");
                return new List<LoanRequestModel>();
            }
        }

        public async Task<LoanRequestModel> GetLoanByIdAsync(long id)
        {
            return new LoanRequestModel();
        }

        public async Task<bool> SubmitLoanAsync(LoanRequestModel request)
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);
                request.ProfileId = pid;
                request.DateRequest = DateTime.Now;

                // Xamarin sends a complex payload with Attachment, let's start simple
                var payload = new 
                { 
                    data = request,
                    Attachment = new { } // Empty attachment for now
                };
                
                var response = await _repository.PostAsync<object, LeaveApiResponse>($"{ApiEndpoints.BaseApiUrl}/api/loanrequest", payload);

                return response != null && (response.IsSuccess || response.Model != null || string.IsNullOrEmpty(response.ValidationMessage));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SubmitLoanAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<LoanTypeModel>> GetLoanTypesAsync()
        {
            try
            {
                var url = $"{ApiEndpoints.BaseApiUrl}/api/loantype/list";
                var response = await _repository.GetAsync<FinancialListResponseWrapper<LoanTypeModel>>(url);
                return response?.ListData ?? new List<LoanTypeModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetLoanTypesAsync Error: {ex.Message}");
                return new List<LoanTypeModel>();
            }
        }
    }

    public class FinancialListResponseWrapper<T>
    {
        public List<T> ListData { get; set; }
        public bool IsSuccess { get; set; }
    }
}
