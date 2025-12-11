using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels
{
    public class SuggestionFormViewModel : BaseViewModel
    {
        private readonly ISuggestionDataService _suggestionService;
        private readonly NavigationManager _navigationManager;

        public SuggestionFormViewModel(ISuggestionDataService suggestionService, NavigationManager navigationManager)
        {
            _suggestionService = suggestionService;
            _navigationManager = navigationManager;
            
            SubmitCommand = new Command(async () => await SubmitAsync());
            CancelCommand = new Command(GoBack);
        }

        // Form Fields
        private SuggestionCategoryModel _selectedCategory;
        public SuggestionCategoryModel SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        private string _detail;
        public string Detail
        {
            get => _detail;
            set => SetProperty(ref _detail, value);
        }

        // Dropdowns
        public ObservableCollection<SuggestionCategoryModel> Categories { get; private set; } = new();

        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }

        public override async Task InitializeAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                var categories = await _suggestionService.GetCategoriesAsync();
                Categories = new ObservableCollection<SuggestionCategoryModel>(categories);
            }, "Loading form...");
        }

        private async Task SubmitAsync()
        {
            if (SelectedCategory == null)
            {
                ErrorMessage = "Please select a category.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Detail))
            {
                ErrorMessage = "Suggestion detail is required.";
                return;
            }

            await ExecuteBusyAsync(async () =>
            {
                var suggestion = new SuggestionModel
                {
                    SuggestionCategoryId = SelectedCategory.SuggestionCategoryId,
                    Detail = Detail
                };

                var success = await _suggestionService.SubmitSuggestionAsync(suggestion);

                if (success)
                {
                    SuccessMessage = "Suggestion submitted successfully.";
                    await Task.Delay(1500);
                    GoBack();
                }
                else
                {
                    ErrorMessage = "Failed to submit suggestion.";
                }
            }, "Submitting...");
        }

        private void GoBack()
        {
            _navigationManager.NavigateTo("/suggestion");
        }
    }
}
