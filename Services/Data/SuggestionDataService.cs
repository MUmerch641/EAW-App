using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class SuggestionDataService : ISuggestionDataService
    {
        private readonly IGenericRepository _repository;

        public SuggestionDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SuggestionListModel>> GetSuggestionsAsync()
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);

                var url = $"{ApiEndpoints.BaseApiUrl}/api/suggestion/list?ProfileId={pid}&Page=1&Rows=100&SortOrder=0";
                var response = await _repository.GetAsync<SuggestionListResponseWrapper>(url);
                return response?.ListData ?? new List<SuggestionListModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetSuggestionsAsync Error: {ex.Message}");
                return new List<SuggestionListModel>();
            }
        }

        public async Task<List<SuggestionCategoryModel>> GetCategoriesAsync()
        {
            try
            {
                var url = $"{ApiEndpoints.BaseApiUrl}/api/suggestioncategory/list";
                
                // Try Wrapper first
                try 
                {
                    var response = await _repository.GetAsync<PaginatedResponse<SuggestionCategoryModel>>(url);
                    if (response != null && response.Data != null && response.Data.Any())
                        return response.Data;
                }
                catch { }

                var listResponse = await _repository.GetAsync<List<SuggestionCategoryModel>>(url);
                return listResponse ?? new List<SuggestionCategoryModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCategoriesAsync Error: {ex.Message}");
                return new List<SuggestionCategoryModel>();
            }
        }

        public async Task<bool> SubmitSuggestionAsync(SuggestionModel suggestion)
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);
                suggestion.ProfileId = pid;
                suggestion.SourceId = 2; // Mobile

                var payload = new { data = suggestion };
                var response = await _repository.PostAsync<object, LeaveApiResponse>($"{ApiEndpoints.BaseApiUrl}/api/suggestion", payload);

                return response != null && (response.IsSuccess || response.Model != null || string.IsNullOrEmpty(response.ValidationMessage));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SubmitSuggestionAsync Error: {ex.Message}");
                return false;
            }
        }
    }

    public class SuggestionListResponseWrapper
    {
        public List<SuggestionListModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
    }
}
