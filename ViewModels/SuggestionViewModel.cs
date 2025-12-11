using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels
{
    public class SuggestionViewModel : BaseViewModel
    {
        private readonly ISuggestionDataService _suggestionService;
        private readonly NavigationManager _navigationManager;

        public SuggestionViewModel(ISuggestionDataService suggestionService, NavigationManager navigationManager)
        {
            _suggestionService = suggestionService;
            _navigationManager = navigationManager;
            
            CreateNewCommand = new Command(CreateNew);
            RefreshCommand = new Command(async () => await LoadDataAsync());
        }

        private ObservableCollection<SuggestionListModel> _suggestions;
        public ObservableCollection<SuggestionListModel> Suggestions
        {
            get => _suggestions;
            set => SetProperty(ref _suggestions, value);
        }

        public ICommand CreateNewCommand { get; }
        public ICommand RefreshCommand { get; }

        public override async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                var list = await _suggestionService.GetSuggestionsAsync();
                Suggestions = new ObservableCollection<SuggestionListModel>(list);
            }, "Loading suggestions...");
        }

        private void CreateNew()
        {
            _navigationManager.NavigateTo("/suggestion/new");
        }
    }
}
