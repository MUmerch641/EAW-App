using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels
{
    public class UndertimeRequestFormViewModel : BaseViewModel
    {
        private readonly IUndertimeDataService _undertimeService;
        private readonly NavigationManager _navigationManager;

        public UndertimeRequestFormViewModel(IUndertimeDataService undertimeService, NavigationManager navigationManager)
        {
            _undertimeService = undertimeService;
            _navigationManager = navigationManager;
            
            SubmitCommand = new Command(async () => await SubmitAsync());
            CancelCommand = new Command(GoBack);
        }

        // Form Fields
        private DateTime _undertimeDate = DateTime.Today;
        public DateTime UndertimeDate
        {
            get => _undertimeDate;
            set => SetProperty(ref _undertimeDate, value);
        }

        private UndertimeTypeModel _selectedUndertimeType;
        public UndertimeTypeModel SelectedUndertimeType
        {
            get => _selectedUndertimeType;
            set => SetProperty(ref _selectedUndertimeType, value);
        }

        private string _reason;
        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        private DateTime _departureTime = DateTime.Today.Add(DateTime.Now.TimeOfDay);
        public DateTime DepartureTime
        {
            get => _departureTime;
            set 
            {
                SetProperty(ref _departureTime, value);
                CalculateDuration();
            }
        }

        private DateTime _arrivalTime = DateTime.Today.Add(DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(1)));
        public DateTime ArrivalTime
        {
            get => _arrivalTime;
            set 
            {
                SetProperty(ref _arrivalTime, value);
                CalculateDuration();
            }
        }

        private double _utHrs;
        public double UTHrs
        {
            get => _utHrs;
            set => SetProperty(ref _utHrs, value);
        }

        // Dropdowns
        public ObservableCollection<UndertimeTypeModel> UndertimeTypes { get; private set; } = new();

        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }

        public override async Task InitializeAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                var types = await _undertimeService.GetUndertimeTypesAsync();
                UndertimeTypes = new ObservableCollection<UndertimeTypeModel>(types);
                CalculateDuration();
            }, "Loading form...");
        }

        private void CalculateDuration()
        {
            if (ArrivalTime > DepartureTime)
            {
                UTHrs = (ArrivalTime - DepartureTime).TotalHours;
            }
            else
            {
                UTHrs = 0;
            }
        }

        private async Task SubmitAsync()
        {
            if (SelectedUndertimeType == null)
            {
                ErrorMessage = "Please select an undertime type.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Reason))
            {
                ErrorMessage = "Reason is required.";
                return;
            }

            if (UTHrs <= 0)
            {
                ErrorMessage = "Invalid duration. Arrival time must be after departure time.";
                return;
            }

            await ExecuteBusyAsync(async () =>
            {
                var request = new UndertimeRequestModel
                {
                    UndertimeDate = UndertimeDate,
                    UndertimeTypeId = (short)SelectedUndertimeType.Id,
                    Reason = Reason,
                    DepartureTime = UndertimeDate.Date + DepartureTime.TimeOfDay,
                    ArrivalTime = UndertimeDate.Date + ArrivalTime.TimeOfDay,
                    UTHrs = UTHrs
                };

                var success = await _undertimeService.SubmitUndertimeRequestAsync(request);

                if (success)
                {
                    SuccessMessage = "Request submitted successfully.";
                    await Task.Delay(1500);
                    GoBack();
                }
                else
                {
                    ErrorMessage = "Failed to submit request.";
                }
            }, "Submitting...");
        }

        private void GoBack()
        {
            _navigationManager.NavigateTo("/undertime");
        }
    }
}
