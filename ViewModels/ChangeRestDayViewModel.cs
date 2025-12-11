using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.Navigation;
using System;
using System.Threading.Tasks;

namespace MauiHybridApp.ViewModels
{
    public partial class ChangeRestDayViewModel : BaseViewModel
    {
        private readonly IChangeRestdayScheduleDataService _dataService;
        private readonly INavigationService _navigationService;

        private ChangeRestdayHolder _holder;
        public ChangeRestdayHolder Holder
        {
            get => _holder;
            set => SetProperty(ref _holder, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public ChangeRestDayViewModel(
            IChangeRestdayScheduleDataService dataService,
            INavigationService navigationService)
        {
            _dataService = dataService;
            _navigationService = navigationService;
            _holder = new ChangeRestdayHolder();
        }

        public override async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // Initialize form with today's date or defaults
                Holder = await _dataService.InitForm(0, DateTime.Now);
            }
            catch (Exception ex)
            {
                HandleError(ex, "Failed to initialize Change Rest Day form");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SubmitAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(Holder.ChangeRestdayModel.Reason))
                {
                    // Show error (using a dialog service if available, or just setting message)
                    ErrorMessage = "Reason is required.";
                    return;
                }

                Holder = await _dataService.SubmitRequest(Holder);

                if (Holder.Success)
                {
                    await _navigationService.NavigateBackAsync();
                }
                else
                {
                    ErrorMessage = Holder.Msg;
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "Failed to submit request");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await _navigationService.NavigateBackAsync();
        }

        [RelayCommand]
        private async Task SelectEmployeeAsync()
        {
            // Placeholder for Employee Selection (Directory feature needed)
            // await _navigationService.NavigateToAsync("/employee-list-modal");
            ErrorMessage = "Employee Directory feature coming soon."; 
            await Task.CompletedTask;
        }
    }
}
