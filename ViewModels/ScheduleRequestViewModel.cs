using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels
{
    public class ScheduleRequestViewModel : BaseViewModel
    {
        private readonly IScheduleDataService _scheduleService;
        private readonly NavigationManager _navigationManager;

        public ScheduleRequestViewModel(IScheduleDataService scheduleService, NavigationManager navigationManager)
        {
            _scheduleService = scheduleService;
            _navigationManager = navigationManager;
            
            SubmitCommand = new Command(async () => await SubmitAsync());
            CancelCommand = new Command(GoBack);
        }

        private string _requestType; // "work-schedule" or "rest-day"
        public string RequestType
        {
            get => _requestType;
            set
            {
                SetProperty(ref _requestType, value);
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Title => RequestType == "rest-day" ? "Change Rest Day" : "Change Work Schedule";

        // Form Fields
        private DateTime _startDate = DateTime.Today;
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private DateTime _endDate = DateTime.Today;
        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        private string _reason;
        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        private string _newSchedule; // For Work Schedule
        public string NewSchedule
        {
            get => _newSchedule;
            set => SetProperty(ref _newSchedule, value);
        }

        private DateTime _newRestDay = DateTime.Today; // For Rest Day
        public DateTime NewRestDay
        {
            get => _newRestDay;
            set => SetProperty(ref _newRestDay, value);
        }

        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }

        public void Initialize(string type)
        {
            RequestType = type;
            ClearError();
            SuccessMessage = string.Empty;
        }

        private async Task SubmitAsync()
        {
            if (string.IsNullOrWhiteSpace(Reason))
            {
                ErrorMessage = "Reason is required.";
                return;
            }

            await ExecuteBusyAsync(async () =>
            {
                bool success = false;
                if (RequestType == "work-schedule")
                {
                    var request = new ChangeWorkScheduleRequest
                    {
                        StartDate = StartDate,
                        EndDate = EndDate,
                        Reason = Reason,
                        NewSchedule = NewSchedule
                    };
                    success = await _scheduleService.SubmitWorkScheduleChangeAsync(request);
                }
                else
                {
                    var request = new ChangeRestDayRequest
                    {
                        StartDate = StartDate,
                        EndDate = EndDate,
                        Reason = Reason,
                        NewRestDay = NewRestDay
                    };
                    success = await _scheduleService.SubmitRestDayChangeAsync(request);
                }

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
            }, "Submitting request...");
        }

        private void GoBack()
        {
            _navigationManager.NavigateTo("/schedule");
        }
    }
}
