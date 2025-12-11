using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class PerformanceDataService : IPerformanceDataService
    {
        private readonly IGenericRepository _repository;

        public PerformanceDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<object>> GetPerformanceEvaluationsAsync()
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);

                // Using MyApprovalRequest structure as it seems standard for lists
                var request = new MyApprovalRequest
                {
                    ProfileId = pid,
                    Page = 1,
                    Rows = 50,
                    SortOrder = 1,
                    Status = "",
                    Keyword = "",
                    TransactionTypes = ""
                };

                // Construct URL manually or use PostAsync if it's a POST
                // Based on other services, lists are often POST with a payload or GET with query params
                // Assuming GET with query params for now based on Payroll service pattern
                var queryString = $"?ProfileId={request.ProfileId}&Page={request.Page}&Rows={request.Rows}&SortOrder={request.SortOrder}";
                var url = $"{ApiEndpoints.PerformanceEvaluation}/list{queryString}"; // Guessing /list endpoint

                // If the base is just "api/performanceevaluation", it might need specific action
                // Let's try standard list pattern
                var response = await _repository.GetAsync<ListResponse<object>>(url);

                if (response != null && response.ListData != null)
                {
                    return response.ListData;
                }

                return new List<object>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PE List Error: {ex.Message}");
                return new List<object>();
            }
        }

        public async Task<object> GetIndividualObjectivesAsync()
        {
            await Task.Delay(100);
            return new { };
        }
    }
}
