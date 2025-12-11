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
    public partial class ChangeWorkScheduleViewModel : BaseViewModel
    {
        private readonly IChangeWorkScheduleDataService _dataService;
        private readonly INavigationService _navigationService;

        private ChangeWorkScheduleHolder _holder;
        public ChangeWorkScheduleHolder Holder
        {
            get => _holder;
            set => SetProperty(ref _holder, value);
        }

        public ChangeWorkScheduleViewModel(
            IChangeWorkScheduleDataService dataService,
            INavigationService navigationService)
        {
            _dataService = dataService;
            _navigationService = navigationService;
            Holder = new ChangeWorkScheduleHolder();
        }

        public override async Task InitializeAsync()
        {
            IsBusy = true;
            try
            {
                Holder = await _dataService.InitForm(0, null);
            }
            catch (Exception ex)
            {
                HandleError(ex, "Failed to initialize form");
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

            // Basic Validation
            if (Holder.WorkDate == DateTime.MinValue)
            {
                await Application.Current.MainPage.DisplayAlert("Validation", "Please select a valid work date.", "OK");
                return;
            }

            if (Holder.ShiftSelectedItem == null || Holder.ShiftSelectedItem.ShiftId <= 0)
            {
                // If Custom Schedule is not enabled, shift is required
                if (!Holder.EnableCustomSched)
                {
                     await Application.Current.MainPage.DisplayAlert("Validation", "Please select a shift.", "OK");
                     return;
                }
            }

            IsBusy = true;
            try
            {
                Holder = await _dataService.SubmitRequest(Holder);
                if (Holder.Success)
                {
                     await Application.Current.MainPage.DisplayAlert("Success", "Request submitted successfully.", "OK");
                     await _navigationService.NavigateBackAsync();
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
        private async Task SelectShiftAsync(ShiftDto shift)
        {
            if (shift == null) return;
            
            Holder.ShiftSelectedItem = shift;
            
            // Auto-populate times if standard shift
            if (shift.StartTime.HasValue && shift.EndTime.HasValue)
            {
                 // Logic to set start/end times based on shift definition
                 // For now relying on user manual verification or advanced logic if needed
            }
        }
    }
}
