using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MauiHybridApp.ViewModels
{
    public partial class MyRequestViewModel : BaseViewModel
    {
        private readonly IMyRequestDataService _dataService;
        private readonly INavigationService _navigationService;

        private ObservableCollection<MyRequestListModel> _myRequests;
        public ObservableCollection<MyRequestListModel> MyRequests
        {
            get => _myRequests;
            set => SetProperty(ref _myRequests, value);
        }

        private string _keyword = string.Empty;
        public string Keyword
        {
            get => _keyword;
            set => SetProperty(ref _keyword, value);
        }

        private int _totalItems;
        public int TotalItems
        {
            get => _totalItems;
            set => SetProperty(ref _totalItems, value);
        }

        private bool _isNewRequestModalVisible;
        public bool IsNewRequestModalVisible
        {
            get => _isNewRequestModalVisible;
            set => SetProperty(ref _isNewRequestModalVisible, value);
        }

        private bool _isFilterModalVisible;
        public bool IsFilterModalVisible
        {
            get => _isFilterModalVisible;
            set => SetProperty(ref _isFilterModalVisible, value);
        }

        public ObservableCollection<RequestTypeModel> AvailableRequestTypes { get; } = new();
        public ObservableCollection<SelectionItem> FilterTypes { get; } = new();

        public MyRequestViewModel(
            IMyRequestDataService dataService,
            INavigationService navigationService)
        {
            _dataService = dataService;
            _navigationService = navigationService;
            MyRequests = new ObservableCollection<MyRequestListModel>();
            
            InitializeRequestTypes();
            InitializeFilterTypes();
        }

        private void InitializeRequestTypes()
        {
            AvailableRequestTypes.Clear();
            AvailableRequestTypes.Add(new RequestTypeModel { RequestTypeId = 1, Title = "Leave Request", Icon = "oi oi-calendar", Route = "/leave" });
            AvailableRequestTypes.Add(new RequestTypeModel { RequestTypeId = 3, Title = "Overtime Request", Icon = "oi oi-clock", Route = "/overtime" });
            AvailableRequestTypes.Add(new RequestTypeModel { RequestTypeId = 5, Title = "Official Business", Icon = "oi oi-briefcase", Route = "/official-business" });
            AvailableRequestTypes.Add(new RequestTypeModel { RequestTypeId = 4, Title = "Time Off Request", Icon = "oi oi-timer", Route = "/time-off-request" });
            AvailableRequestTypes.Add(new RequestTypeModel { RequestTypeId = 6, Title = "Change Schedule", Icon = "oi oi-calendar", Route = "/change-work-schedule" });
            AvailableRequestTypes.Add(new RequestTypeModel { RequestTypeId = 22, Title = "Change Rest Day", Icon = "oi oi-loop-circular", Route = "/change-rest-day" });
            // Add other types as needed or as "Coming Soon" placeholders
        }

        private void InitializeFilterTypes()
        {
            FilterTypes.Clear();
            foreach(var type in AvailableRequestTypes)
            {
                FilterTypes.Add(new SelectionItem { Value = type.RequestTypeId.ToString(), Name = type.Title, IsSelected = false });
            }
        }

        public override async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                 // Reset list for reload
                if (MyRequests == null) MyRequests = new ObservableCollection<MyRequestListModel>();
                
                var selectedFilters = FilterTypes.Where(x => x.IsSelected).Select(x => x.Value).ToList();
                var filterString = selectedFilters.Any() ? string.Join(",", selectedFilters) : string.Empty;

                var param = new ListParam { KeyWord = Keyword, FilterTypes = filterString };
                MyRequests = await _dataService.RetrieveMyRequestList(MyRequests, param);
                TotalItems = MyRequests.Count;
            }
            catch (Exception ex)
            {
                HandleError(ex, "Failed to load requests");
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        [RelayCommand]
        private async Task SearchAsync()
        {
             MyRequests.Clear();
             await LoadDataAsync();
        }

        [RelayCommand]
        private void OpenNewRequestModal() => IsNewRequestModalVisible = true;

        [RelayCommand]
        private void CloseNewRequestModal() => IsNewRequestModalVisible = false;

        [RelayCommand]
        private void OpenFilterModal() => IsFilterModalVisible = true;

        [RelayCommand]
        private void CloseFilterModal() => IsFilterModalVisible = false;

        [RelayCommand]
        private async Task ApplyFilterAsync()
        {
            CloseFilterModal();
            MyRequests.Clear();
            await LoadDataAsync();
        }
        
        [RelayCommand]
        private async Task NavigateToRequestAsync(RequestTypeModel requestType)
        {
            CloseNewRequestModal();
            if(!string.IsNullOrEmpty(requestType.Route))
            {
                await _navigationService.NavigateToAsync(requestType.Route);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Coming Soon", $"{requestType.Title} is not yet implemented.", "OK");
            }
        }

        [RelayCommand]
        private async Task ViewDetailsAsync(MyRequestListModel item)
        {
            if (item == null) return;

            string route = GetRouteForTransaction(item.TransactionTypeId, item.TransactionId);
            if (!string.IsNullOrEmpty(route))
            {
                // Navigate to detail page
                 await _navigationService.NavigateToAsync(route);
            }
            else 
            {
                 // Fallback or Coming Soon
                await Application.Current.MainPage.DisplayAlert("Info", $"Details for {item.TransactionType} coming soon.", "OK");
            }
        }
        
        [RelayCommand]
        private async Task ViewHistoryAsync(MyRequestListModel item)
        {
             if (item == null) return;
             await _navigationService.NavigateToAsync($"/transaction-history/{item.TransactionTypeId}/{item.TransactionId}");
        }

        private string GetRouteForTransaction(long typeId, long id)
        {
            return typeId switch
            {
                1 => $"/leave-history", // Leave
                3 => $"/overtime", // Overtime 
                4 => $"/time-off-request", // Time Off (Generic view or specific ignored for now as it's usage, maybe edit?)
                5 => $"/official-business", // OB (Similarly, might need ID passing)
                6 => $"/change-work-schedule", // Change Sched
                22 => $"/change-rest-day", // Change Rest Day (Corrected ID)
                64 => $"/change-rest-day", // Legacy ID support just in case
                 _ => string.Empty
            };
        }
    }

    public class RequestTypeModel
    {
        public long RequestTypeId { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Route { get; set; }
    }
}
