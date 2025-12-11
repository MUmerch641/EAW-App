using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels
{
    public class SpecialWorkScheduleViewModel : BaseViewModel
    {
        private readonly ISpecialWorkScheduleDataService _swsService;
        private readonly NavigationManager _navigationManager;

        public SpecialWorkScheduleViewModel(ISpecialWorkScheduleDataService swsService, NavigationManager navigationManager)
        {
            _swsService = swsService;
            _navigationManager = navigationManager;
            
            CreateNewCommand = new Command(CreateNew);
            RefreshCommand = new Command(async () => await LoadDataAsync());
        }

        private ObservableCollection<SpecialWorkScheduleListModel> _requests;
        public ObservableCollection<SpecialWorkScheduleListModel> Requests
        {
            get => _requests;
            set => SetProperty(ref _requests, value);
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
                var list = await _swsService.GetSpecialWorkScheduleRequestsAsync();
                Requests = new ObservableCollection<SpecialWorkScheduleListModel>(list);
            }, "Loading requests...");
        }

        private void CreateNew()
        {
            _navigationManager.NavigateTo("/specialworkschedule/new");
        }
    }
}
