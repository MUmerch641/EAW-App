using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels
{
    public class UndertimeRequestViewModel : BaseViewModel
    {
        private readonly IUndertimeDataService _undertimeService;
        private readonly NavigationManager _navigationManager;

        public UndertimeRequestViewModel(IUndertimeDataService undertimeService, NavigationManager navigationManager)
        {
            _undertimeService = undertimeService;
            _navigationManager = navigationManager;
            
            CreateNewCommand = new Command(CreateNew);
            RefreshCommand = new Command(async () => await LoadDataAsync());
        }

        private ObservableCollection<UndertimeRequestListModel> _undertimeRequests;
        public ObservableCollection<UndertimeRequestListModel> UndertimeRequests
        {
            get => _undertimeRequests;
            set => SetProperty(ref _undertimeRequests, value);
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
                var list = await _undertimeService.GetUndertimeRequestsAsync();
                UndertimeRequests = new ObservableCollection<UndertimeRequestListModel>(list);
            }, "Loading requests...");
        }

        private void CreateNew()
        {
            _navigationManager.NavigateTo("/undertime/request/new");
        }
    }
}
