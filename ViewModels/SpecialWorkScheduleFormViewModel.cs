using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels
{
    public class SpecialWorkScheduleFormViewModel : BaseViewModel
    {
        private readonly ISpecialWorkScheduleDataService _swsService;
        private readonly NavigationManager _navigationManager;

        public SpecialWorkScheduleFormViewModel(ISpecialWorkScheduleDataService swsService, NavigationManager navigationManager)
        {
            _swsService = swsService;
            _navigationManager = navigationManager;
            
            SubmitCommand = new Command(async () => await SubmitAsync());
            CancelCommand = new Command(GoBack);
        }

        // Form Fields
        private DateTime _workDate = DateTime.Today;
        public DateTime WorkDate
        {
            get => _workDate;
            set => SetProperty(ref _workDate, value);
        }

        private ShiftModel _selectedShift;
        public ShiftModel SelectedShift
        {
            get => _selectedShift;
            set 
            {
                SetProperty(ref _selectedShift, value);
                OnShiftChanged();
            }
        }

        private string _reason;
        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        private bool _isCustomSchedule;
        public bool IsCustomSchedule
        {
            get => _isCustomSchedule;
            set => SetProperty(ref _isCustomSchedule, value);
        }

        // Custom Schedule Fields
        private DateTime _startTime = DateTime.Today.Add(new TimeSpan(9, 0, 0));
        public DateTime StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }

        private DateTime _endTime = DateTime.Today.Add(new TimeSpan(18, 0, 0));
        public DateTime EndTime
        {
            get => _endTime;
            set => SetProperty(ref _endTime, value);
        }

        private DateTime _lunchStart = DateTime.Today.Add(new TimeSpan(12, 0, 0));
        public DateTime LunchStart
        {
            get => _lunchStart;
            set => SetProperty(ref _lunchStart, value);
        }

        private DateTime _lunchEnd = DateTime.Today.Add(new TimeSpan(13, 0, 0));
        public DateTime LunchEnd
        {
            get => _lunchEnd;
            set => SetProperty(ref _lunchEnd, value);
        }

        private bool _forOffsetting;
        public bool ForOffsetting
        {
            get => _forOffsetting;
            set => SetProperty(ref _forOffsetting, value);
        }

        private DateTime _offsetExpirationDate = DateTime.Today.AddMonths(1);
        public DateTime OffsetExpirationDate
        {
            get => _offsetExpirationDate;
            set => SetProperty(ref _offsetExpirationDate, value);
        }

        // Dropdowns
        public ObservableCollection<ShiftModel> Shifts { get; private set; } = new();

        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }

        public override async Task InitializeAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                var shifts = await _swsService.GetShiftsAsync();
                Shifts = new ObservableCollection<ShiftModel>(shifts);
            }, "Loading form...");
        }

        private void OnShiftChanged()
        {
            if (SelectedShift != null && SelectedShift.ShiftId == -1) // Others
            {
                IsCustomSchedule = true;
            }
            else if (SelectedShift != null)
            {
                IsCustomSchedule = false;
                // Auto-fill times from shift if available
                if (SelectedShift.StartTime.HasValue) StartTime = DateTime.Today.Add(SelectedShift.StartTime.Value.TimeOfDay);
                if (SelectedShift.EndTime.HasValue) EndTime = DateTime.Today.Add(SelectedShift.EndTime.Value.TimeOfDay);
                if (SelectedShift.LunchBreakStartTime.HasValue) LunchStart = DateTime.Today.Add(SelectedShift.LunchBreakStartTime.Value.TimeOfDay);
                if (SelectedShift.LunchBreakEndTime.HasValue) LunchEnd = DateTime.Today.Add(SelectedShift.LunchBreakEndTime.Value.TimeOfDay);
            }
        }

        private async Task SubmitAsync()
        {
            if (SelectedShift == null)
            {
                ErrorMessage = "Please select a shift.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Reason))
            {
                ErrorMessage = "Reason is required.";
                return;
            }

            await ExecuteBusyAsync(async () =>
            {
                var request = new SpecialWorkScheduleRequestModel
                {
                    WorkDate = WorkDate,
                    ShiftId = SelectedShift.ShiftId,
                    Reason = Reason,
                    ForOffsetting = ForOffsetting,
                    OffsettingExpirationDate = ForOffsetting ? OffsetExpirationDate : (DateTime?)null
                };

                if (IsCustomSchedule)
                {
                    request.StartTime = WorkDate.Date + StartTime.TimeOfDay;
                    request.EndTime = WorkDate.Date + EndTime.TimeOfDay;
                    request.LunchBreakStartTime = WorkDate.Date + LunchStart.TimeOfDay;
                    request.LunchBreakEndTime = WorkDate.Date + LunchEnd.TimeOfDay;
                    
                    // Basic duration calc
                    request.WorkingHours = (EndTime - StartTime).TotalHours - (LunchEnd - LunchStart).TotalHours;
                    request.LunchDuration = (LunchEnd - LunchStart).TotalHours;
                }
                else
                {
                    // If not custom, backend might handle it or we use shift times
                    // For now, let's send the times from the selected shift if we have them
                    if (SelectedShift.StartTime.HasValue)
                    {
                         request.StartTime = WorkDate.Date + SelectedShift.StartTime.Value.TimeOfDay;
                         request.EndTime = WorkDate.Date + SelectedShift.EndTime.Value.TimeOfDay;
                         request.LunchBreakStartTime = WorkDate.Date + SelectedShift.LunchBreakStartTime.Value.TimeOfDay;
                         request.LunchBreakEndTime = WorkDate.Date + SelectedShift.LunchBreakEndTime.Value.TimeOfDay;
                    }
                }

                var success = await _swsService.SubmitSpecialWorkScheduleRequestAsync(request);

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
            _navigationManager.NavigateTo("/specialworkschedule");
        }
    }
}
