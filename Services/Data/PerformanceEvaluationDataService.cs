using MauiHybridApp.Models;
using MauiHybridApp.Models.PerformanceEvaluation;
using MauiHybridApp.Utils;
using MauiHybridApp.Services.Data;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class PerformanceEvaluationDataService : IPerformanceEvaluationDataService
    {
        private readonly IGenericRepository _repository;

        public PerformanceEvaluationDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PEListDto>> GetListAsync()
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
                var queryString = $"?ProfileId={request.ProfileId}&Page={request.Page}&Rows={request.Rows}&SortOrder={request.SortOrder}";
                // Reverting to assessment/list since list-eaw-app returns empty
                var url = $"{ApiEndpoints.PerformanceEvaluation}/assessment/list{queryString}";

                // If the base is just "api/performanceevaluation", it might need specific action
                // Let's try standard list pattern
                var response = await _repository.GetAsync<ListResponse<PEListDto>>(url);

                if (response != null && response.ListData != null)
                {
                    return response.ListData;
                }

                return new List<PEListDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PE List Error: {ex.Message}");
                return new List<PEListDto>();
            }
        }

        public async Task<PEFormHolder> InitFormAsync(long id)
        {
            try
            {
                // Xamarin uses: {ApiConstants.PerformanceEvaluation}/{id}
                var url = $"{ApiEndpoints.PerformanceEvaluation}/{id}";
                return await _repository.GetAsync<PEFormHolder>(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PE Init Form Error: {ex.Message}");
                return new PEFormHolder();
            }
        }

        public async Task<PEFormHolder> SavePODetailsAsync(PEFormHolder holder)
        {
            try
            {
                var url = $"{ApiEndpoints.PerformanceEvaluation}/save-po-details";
                return await _repository.PostAsync<PEFormHolder, PEFormHolder>(url, holder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PE Save Error: {ex.Message}");
                return holder;
            }
        }
    }
}
