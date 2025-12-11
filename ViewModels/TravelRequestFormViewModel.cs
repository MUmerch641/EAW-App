using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels
{
    public class TravelRequestFormViewModel : BaseViewModel
    {
        private readonly ITravelDataService _travelService;
        private readonly NavigationManager _navigationManager;

        public TravelRequestFormViewModel(ITravelDataService travelService, NavigationManager navigationManager)
        {
            _travelService = travelService;
            _navigationManager = navigationManager;
            
            SubmitCommand = new Command(async () => await SubmitAsync());
            CancelCommand = new Command(GoBack);
        }

        // Form Fields
        private DateTime _requestDate = DateTime.Today;
        public DateTime RequestDate
        {
            get => _requestDate;
            set => SetProperty(ref _requestDate, value);
        }

        private TripTypeModel _selectedTripType;
        public TripTypeModel SelectedTripType
        {
            get => _selectedTripType;
            set => SetProperty(ref _selectedTripType, value);
        }

        private string _details;
        public string Details
        {
            get => _details;
            set => SetProperty(ref _details, value);
        }

        private string _reason;
        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        // Trip 1
        private string _firstOrigin;
        public string FirstOrigin
        {
            get => _firstOrigin;
            set => SetProperty(ref _firstOrigin, value);
        }

        private string _firstDestination;
        public string FirstDestination
        {
            get => _firstDestination;
            set => SetProperty(ref _firstDestination, value);
        }

        private DateTime _firstDepartureDate = DateTime.Today;
        public DateTime FirstDepartureDate
        {
            get => _firstDepartureDate;
            set => SetProperty(ref _firstDepartureDate, value);
        }

        private DateTime _firstDepartureTime = DateTime.Today.Add(new TimeSpan(8, 0, 0));
        public DateTime FirstDepartureTime
        {
            get => _firstDepartureTime;
            set => SetProperty(ref _firstDepartureTime, value);
        }

        // Trip 2
        private string _secondOrigin;
        public string SecondOrigin
        {
            get => _secondOrigin;
            set => SetProperty(ref _secondOrigin, value);
        }

        private string _secondDestination;
        public string SecondDestination
        {
            get => _secondDestination;
            set => SetProperty(ref _secondDestination, value);
        }

        private DateTime _secondDepartureDate = DateTime.Today;
        public DateTime SecondDepartureDate
        {
            get => _secondDepartureDate;
            set => SetProperty(ref _secondDepartureDate, value);
        }

        private DateTime _secondDepartureTime = DateTime.Today.Add(new TimeSpan(17, 0, 0));
        public DateTime SecondDepartureTime
        {
            get => _secondDepartureTime;
            set => SetProperty(ref _secondDepartureTime, value);
        }

        // Dropdowns
        public ObservableCollection<TripTypeModel> TripTypes { get; private set; } = new();
        public ObservableCollection<string> Origins { get; private set; } = new();
        public ObservableCollection<string> Destinations { get; private set; } = new();

        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }

        public override async Task InitializeAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                var types = await _travelService.GetTripTypesAsync();
                TripTypes = new ObservableCollection<TripTypeModel>(types);

                var initData = await _travelService.GetInitDataAsync();
                Origins = new ObservableCollection<string>(initData.Origins);
                Destinations = new ObservableCollection<string>(initData.Destinations);
            }, "Loading form...");
        }

        private async Task SubmitAsync()
        {
            if (SelectedTripType == null)
            {
                ErrorMessage = "Please select a trip type.";
                return;
            }

            if (string.IsNullOrWhiteSpace(FirstOrigin) || string.IsNullOrWhiteSpace(FirstDestination))
            {
                ErrorMessage = "Origin and Destination are required.";
                return;
            }

            await ExecuteBusyAsync(async () =>
            {
                var request = new TravelRequestModel
                {
                    RequestDate = RequestDate,
                    TypeOfBusinessTrip = SelectedTripType.Id,
                    Details = Details,
                    Reason = Reason,
                    FirstOrigin = FirstOrigin,
                    FirstDestination = FirstDestination,
                    FirstDepartureDate = FirstDepartureDate,
                    FirstDepartureTime = FirstDepartureDate.Date + FirstDepartureTime.TimeOfDay,
                    SecondOrigin = SecondOrigin,
                    SecondDestination = SecondDestination,
                    SecondDepartureDate = SecondDepartureDate,
                    SecondDepartureTime = SecondDepartureDate.Date + SecondDepartureTime.TimeOfDay
                };

                var success = await _travelService.SubmitTravelRequestAsync(request);

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
            _navigationManager.NavigateTo("/travel");
        }
    }
}
