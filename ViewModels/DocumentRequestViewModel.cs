using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels
{
    public class DocumentRequestViewModel : BaseViewModel
    {
        private readonly IDocumentDataService _documentService;
        private readonly NavigationManager _navigationManager;

        public DocumentRequestViewModel(IDocumentDataService documentService, NavigationManager navigationManager)
        {
            _documentService = documentService;
            _navigationManager = navigationManager;
            
            CreateNewCommand = new Command(CreateNew);
            RefreshCommand = new Command(async () => await LoadDataAsync());
        }

        private ObservableCollection<DocumentRequestListModel> _documentRequests;
        public ObservableCollection<DocumentRequestListModel> DocumentRequests
        {
            get => _documentRequests;
            set => SetProperty(ref _documentRequests, value);
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
                var list = await _documentService.GetDocumentRequestsAsync();
                DocumentRequests = new ObservableCollection<DocumentRequestListModel>(list);
            }, "Loading requests...");
        }

        private void CreateNew()
        {
            _navigationManager.NavigateTo("/document/request/new");
        }
    }
}
