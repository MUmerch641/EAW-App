using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiHybridApp.ViewModels
{
    public partial class EmployeeListViewModel : BaseViewModel
    {
        private readonly IEmployeeDataService _dataService;
        private readonly INavigationService _navigationService;

        private ObservableCollection<EmployeeListModel> _employees;
        public ObservableCollection<EmployeeListModel> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }

        private string _keyword;
        public string Keyword
        {
            get => _keyword;
            set => SetProperty(ref _keyword, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private int _totalItems = 100; // Default or fetched from config?
        private int _itemsPerPage = 20;

        public EmployeeListViewModel(
            IEmployeeDataService dataService,
            INavigationService navigationService)
        {
            _dataService = dataService;
            _navigationService = navigationService;
            Employees = new ObservableCollection<EmployeeListModel>();
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
                var param = new ListParam
                {
                    ListCount = Employees.Count,
                    Count = _itemsPerPage,
                    IsAscending = true,
                    KeyWord = Keyword
                };

                Employees = await _dataService.RetrieveEmployeeList(Employees, param);
            }
            catch (Exception ex)
            {
                HandleError(ex, "Failed to load employees");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            Employees.Clear();
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task SelectEmployeeAsync(EmployeeListModel employee)
        {
            // If opened as a modal (e.g., from ChangeRestDay), return result
            // For now, just navigate back or show detail
            
            // TODO: Implement result return mechanism (MessagingCenter or Navigation Parameters)
            // Example:
            // WeakReferenceMessenger.Default.Send(new EmployeeSelectedMessage(employee));
            
            await _navigationService.NavigateBackAsync();
        }

        [RelayCommand]
        private async Task LoadMoreAsync()
        {
             await LoadDataAsync();
        }

        [RelayCommand]
        private async Task CloseAsync()
        {
            await _navigationService.NavigateBackAsync();
        }
    }
}
