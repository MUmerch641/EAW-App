using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class EmployeeRelationsDataService : IEmployeeRelationsDataService
    {
        private readonly IGenericRepository _repository;

        public EmployeeRelationsDataService(IGenericRepository repository)
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
                Console.WriteLine($"Suggestion List Error: {ex.Message}");
                return new List<SuggestionListModel>();
            }
        }

        public async Task<List<object>> GetSurveysAsync()
        {
            await Task.Delay(100);
            return new List<object>();
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
}
