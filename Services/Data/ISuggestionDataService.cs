using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MauiHybridApp.Models;

namespace MauiHybridApp.Services.Data
{
    public interface ISuggestionDataService
    {
        Task<List<SuggestionListModel>> GetSuggestionsAsync();
        Task<List<SuggestionCategoryModel>> GetCategoriesAsync();
        Task<bool> SubmitSuggestionAsync(SuggestionModel suggestion);
    }
}
