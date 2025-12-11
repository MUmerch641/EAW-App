using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels
{
    public class TravelRequestViewModel : BaseViewModel
    {
        private readonly ITravelDataService _travelService;
        private readonly NavigationManager _navigationManager;

        public TravelRequestViewModel(ITravelDataService travelService, NavigationManager navigationManager)
        {
            _travelService = travelService;
            _navigationManager = navigationManager;
            
            CreateNewCommand = new Command(CreateNew);
            RefreshCommand = new Command(async () => await LoadDataAsync());
        }

        private ObservableCollection<TravelRequestListModel> _travelRequests;
        public ObservableCollection<TravelRequestListModel> TravelRequests
        {
            get => _travelRequests;
            set => SetProperty(ref _travelRequests, value);
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
                var list = await _travelService.GetTravelRequestsAsync();
                TravelRequests = new ObservableCollection<TravelRequestListModel>(list);
            }, "Loading requests...");
        }

        private void CreateNew()
        {
            _navigationManager.NavigateTo("/travel/request/new");
        }
    }
}
