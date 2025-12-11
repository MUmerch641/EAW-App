using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.Navigation;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels
{
    public class MyScheduleViewModel : BaseViewModel
    {
        private readonly IScheduleDataService _scheduleService;
        private readonly INavigationService _navigationService;

        public MyScheduleViewModel(IScheduleDataService scheduleService, INavigationService navigationService)
        {
            _scheduleService = scheduleService;
            _navigationService = navigationService;
            
            // Default to current month
            var now = DateTime.Now;
            StartDate = new DateTime(now.Year, now.Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
            
            LoadScheduleCommand = new AsyncRelayCommand(LoadScheduleAsync);
            PreviousMonthCommand = new AsyncRelayCommand(async () => await NavigateMonthAsync(-1));
            NextMonthCommand = new AsyncRelayCommand(async () => await NavigateMonthAsync(1));
            NavigateToRequestCommand = new AsyncRelayCommand<string>(NavigateToRequestAsync);
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                SetProperty(ref _startDate, value);
                OnPropertyChanged(nameof(CurrentMonthDisplay));
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public string CurrentMonthDisplay => StartDate.ToString("MMMM yyyy");

        private ObservableCollection<MyScheduleListModel> _schedules;
        public ObservableCollection<MyScheduleListModel> Schedules
        {
            get => _schedules;
            set => SetProperty(ref _schedules, value);
        }

        public ICommand LoadScheduleCommand { get; }
        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand NavigateToRequestCommand { get; }

        public override async Task InitializeAsync()
        {
            await LoadScheduleAsync();
        }

        private async Task LoadScheduleAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                var list = await _scheduleService.RetrieveMyScheduleListAsync(StartDate, EndDate);
                Schedules = new ObservableCollection<MyScheduleListModel>(list);
            }, "Loading schedule...");
        }

        private async Task NavigateMonthAsync(int months)
        {
            StartDate = StartDate.AddMonths(months);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
            await LoadScheduleAsync();
        }

        private async Task NavigateToRequestAsync(string type)
        {
            if (string.IsNullOrEmpty(type)) return;
            await _navigationService.NavigateToAsync($"/schedule/request/{type}");
        }
    }
}
